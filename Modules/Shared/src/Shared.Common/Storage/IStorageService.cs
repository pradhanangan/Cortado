namespace Shared.Common.Storage;

public interface IStorageService
{
    Task UploadFileAsync(string bucketName, string key, Stream fileStream);
    Task<string> GeneratePreSignedUrlAsync(string bucketName, string key, TimeSpan expiration);
}
