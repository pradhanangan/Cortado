using System.Text;

namespace Shared.Common.Authentication;

public static class CognitoComputeSecretHash
{
    public static string ComputeSecretHash(string userPoolClientId, string userPoolClientSecret, string username)
    {
        var dataString = username + userPoolClientId;
        var data = Encoding.UTF8.GetBytes(dataString);
        var key = Encoding.UTF8.GetBytes(userPoolClientSecret);
        using (var hmac = new System.Security.Cryptography.HMACSHA256(key))
        {
            var hash = hmac.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }
    }
}
