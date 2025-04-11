namespace Shared.Common.Authentication;

public interface ITokenParser
{
    string? GetSubFromIdToken(string idToken);
    string? GetEmailFromIdToken(string idToken);
}
