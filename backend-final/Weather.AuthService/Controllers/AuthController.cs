using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Weather.Application;
using Weather.Domain;
using Weather.Infrastructure;
using Google.Apis.Auth;

namespace Weather.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // =====================================
    // REGISTER
    // =====================================
    [HttpPost("register")]
    public IActionResult Register(RegisterDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and Password are required.");
        }

        var existingUser = _context.Users
            .FirstOrDefault(x => x.Email == request.Email);

        if (existingUser != null)
            return BadRequest("User already exists.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            Password = hashedPassword,
            Role = request.Role ?? "User",
            LoginProvider = "Local"
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { user.Id, user.Email, user.Role });
    }

    // =====================================
    // LOGIN
    // =====================================
    [HttpPost("login")]
    public IActionResult Login(LoginDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and Password are required.");
        }

        var user = _context.Users
            .FirstOrDefault(x => x.Email == request.Email);

        if (user == null)
            return BadRequest("User not found.");

        // Google users cannot login with password
        if (user.LoginProvider == "Google")
            return BadRequest("This account uses Google Sign-In. Please login with Google.");

        if (string.IsNullOrEmpty(user.Password) ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return BadRequest("Wrong password.");
        }

        return Ok(new { token = GenerateJwtToken(user), expiresIn = 60 });
    }

    // =====================================
    // GOOGLE LOGIN
    // =====================================
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return BadRequest("Google ID token is required.");

        GoogleJsonWebSignature.Payload payload;

        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config["Google:ClientId"] }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch (InvalidJwtException ex)
        {
            return Unauthorized($"Invalid Google token: {ex.Message}");
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == payload.Email);

        if (user == null)
        {
            // First-time Google login — auto register
            user = new User
            {
                Email = payload.Email,
                GoogleId = payload.Subject,
                FullName = payload.Name,
                ProfilePicture = payload.Picture,
                Role = "User",
                LoginProvider = "Google",
                Password = null
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Link Google to existing local account
            user.GoogleId ??= payload.Subject;
            user.FullName ??= payload.Name;
            user.ProfilePicture ??= payload.Picture;
            await _context.SaveChangesAsync();
        }

        return Ok(new
        {
            token = GenerateJwtToken(user),
            expiresIn = 60,
            user = new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.ProfilePicture,
                user.Role,
                user.LoginProvider
            }
        });
    }

    // =====================================
    // HELPERS
    // =====================================
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
