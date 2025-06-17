using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Authentication;
using Shared.Common.Storage;

namespace Shared.Common;

public static class ConfigureServices
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure settings
        services.Configure<AwsSettings>(configuration.GetSection("AwsSettings"));
        services.Configure<CognitoSettings>(configuration.GetSection("CognitoSettings"));

        // Register the AmazonS3ClientFactory
        services.AddSingleton<AmazonS3ClientFactory>();

        // Use the factory to create the AmazonS3Client
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var factory = sp.GetRequiredService<AmazonS3ClientFactory>();
            return factory.CreateClient();
        });

        // Register other services
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<IUserManagement, CognitoUserManagement>();
        services.AddScoped<ITokenParser, TokenParser>();
        services.AddSingleton<IStorageService, S3StorageService>();

        return services;
    }

    
}
