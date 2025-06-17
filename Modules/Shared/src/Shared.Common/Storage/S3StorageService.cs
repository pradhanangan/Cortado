using Amazon.S3;
using Amazon.S3.Model;

namespace Shared.Common.Storage;

public class S3StorageService(IAmazonS3 amazonS3Client) : IStorageService
{
    private readonly IAmazonS3 _amazonS3Client = amazonS3Client;

    public async Task UploadFileAsync(string bucketName, string key, Stream fileStream)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream
        };

        await _amazonS3Client.PutObjectAsync(request);
    }

    public async Task<string> GeneratePreSignedUrlAsync(string bucketName, string key, TimeSpan expiration)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.Add(expiration)
        };

        return await _amazonS3Client.GetPreSignedURLAsync(request);
    }
}
