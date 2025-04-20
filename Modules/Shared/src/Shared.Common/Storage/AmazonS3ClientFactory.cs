using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Options;
using Shared.Common.Authentication;

namespace Shared.Common.Storage;

public class AmazonS3ClientFactory
{
    private readonly AwsSettings _awsSettings;

    public AmazonS3ClientFactory(IOptions<AwsSettings> awsOptions)
    {
        _awsSettings = awsOptions.Value;
    }

    public IAmazonS3 CreateClient()
    {
        var awsCredentials = new BasicAWSCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey);
        var reg = RegionEndpoint.GetBySystemName(_awsSettings.Region);
        return new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(_awsSettings.Region));
    }
}