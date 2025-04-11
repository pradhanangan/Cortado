namespace Shared.Common.Authentication;

public sealed class CognitoSettings
{
    public string AwsAccessKey { get; set; }
    public string AwsSecretKey { get; set; }
    public string Region { get; set; }
    public string UserPoolId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
