namespace Shared.Common.Authentication;

public class UserSignInResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string IdToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string Error { get; set; }
    public string ErrorDescription { get; set; }
}
