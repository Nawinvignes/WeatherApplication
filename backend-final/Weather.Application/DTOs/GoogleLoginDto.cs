namespace Weather.Application;

public class GoogleLoginDto
{
    /// <summary>
    /// The ID Token received from Google Sign-In on the frontend.
    /// </summary>
    public string IdToken { get; set; } = string.Empty;
}
