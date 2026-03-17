namespace Weather.Domain;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    // Nullable - Google users won't have a password
    public string? Password { get; set; }

    public string Role { get; set; } = "User";

    // ============================================
    // Google OAuth Fields
    // ============================================
    public string? GoogleId { get; set; }

    public string? FullName { get; set; }

    public string? ProfilePicture { get; set; }

    public string LoginProvider { get; set; } = "Local"; // "Local" | "Google"
}