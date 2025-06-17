namespace Shared.Common.Authentication;

public sealed class CognitoSettings
{
    public string UserPoolId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
