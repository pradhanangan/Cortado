using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Authentication;

namespace Shared.Common;

public static class ConfigureServices
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CognitoSettings>(configuration.GetSection("CognitoSettings"));

        services.AddSingleton<IUserManagement, CognitoUserManagement>();
        services.AddScoped<ITokenParser, TokenParser>();
        return services;
    }
}
