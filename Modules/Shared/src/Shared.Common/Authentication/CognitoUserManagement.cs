using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace Shared.Common.Authentication;

public class CognitoUserManagement : IUserManagement
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoIdentityProviderClient;
    private readonly CognitoSettings _cognitoSettings;
    
    public CognitoUserManagement(IOptions<CognitoSettings> options)
    {
        _cognitoSettings = options.Value;

        BasicAWSCredentials awsCredentials = new (_cognitoSettings.AwsAccessKey, _cognitoSettings.AwsSecretKey);
        _cognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient(awsCredentials, RegionEndpoint.APSoutheast2);
    }

    public async Task<Guid> CreateUserAsync(string email, string password)
    {
        var request = new AdminCreateUserRequest
        {
            UserPoolId = _cognitoSettings.UserPoolId,
            Username = email,
            DesiredDeliveryMediums = new List<string> { "EMAIL" },
            TemporaryPassword = password,
            UserAttributes = new List<AttributeType>
            {
                new AttributeType
                {
                    Name = "email",
                    Value = email
                }
            }
        };

        try
        {
            var response = await _cognitoIdentityProviderClient.AdminCreateUserAsync(request);
            Guid userId = Guid.Parse(response.User.Username);
            return userId;        
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating user: {ex.Message}");
        }
    }

    public async Task AddUserToGroupAsync(string email, string group)
    {
        var request = new AdminAddUserToGroupRequest
        { 
            UserPoolId = _cognitoSettings.UserPoolId,
            Username = email,
            GroupName = group
        };

        try
        {
            await _cognitoIdentityProviderClient.AdminAddUserToGroupAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding user to a group: {ex.Message}");
        }
    }

    public async Task ChangePasswordAsync(string accessToken, string oldPassword, string newPassword)
    {
        ChangePasswordRequest request = new ()
        {
            PreviousPassword = oldPassword,
            ProposedPassword = newPassword,
            AccessToken = accessToken
        };

        try
        {
            await _cognitoIdentityProviderClient.ChangePasswordAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error changing password: {ex.Message}");
        }
    }

    public async Task<UserSignUpResponse> SignUpAsync(string email, string password)
    {
        var request = new SignUpRequest
        {
            ClientId = _cognitoSettings.ClientId,
            SecretHash = CognitoComputeSecretHash.ComputeSecretHash(_cognitoSettings.ClientId, _cognitoSettings.ClientSecret, email),
            Username = email,
            Password = password,
            UserAttributes = new List<AttributeType>
                {
                    new AttributeType
                    {
                        Name = "email",
                        Value = email
                    }
                }
        };

        try
        {
            var response = await _cognitoIdentityProviderClient.SignUpAsync(request);
            UserSignUpResponse signUpResponse = new UserSignUpResponse
            {
                UserConfirmed = response.UserConfirmed,
                UserSub = Guid.Parse(response.UserSub)
            };
            return signUpResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error signing up: {ex.Message}");
        }
    }

    public async Task ConfirmSignUpAsync(string email, string confirmationCode)
    {
        var request = new ConfirmSignUpRequest
        {
            ClientId = _cognitoSettings.ClientId,
            SecretHash = CognitoComputeSecretHash.ComputeSecretHash(_cognitoSettings.ClientId, _cognitoSettings.ClientSecret, email),
            Username = email,
            ConfirmationCode = confirmationCode
        };

        try
        {
            await _cognitoIdentityProviderClient.ConfirmSignUpAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error confirming sign up: {ex.Message}");
        }
    }

    public async Task<UserSignInResponse> SignInAsync(string email, string password)
    {
        var request = new InitiateAuthRequest
        {
            AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
            AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", email },
                { "PASSWORD", password },
                { "SECRET_HASH", CognitoComputeSecretHash.ComputeSecretHash(_cognitoSettings.ClientId, _cognitoSettings.ClientSecret, email) }
            },
            ClientId = _cognitoSettings.ClientId,
            
        };

        try
        {
            var response = await _cognitoIdentityProviderClient.InitiateAuthAsync(request);
            return new UserSignInResponse
            {
                AccessToken = response.AuthenticationResult.AccessToken,
                RefreshToken = response.AuthenticationResult.RefreshToken,
                IdToken = response.AuthenticationResult.IdToken,
                ExpiresIn = response.AuthenticationResult.ExpiresIn
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error signing in: {ex.Message}");
        }
    }

    public async Task SignOutAsync(string accessToken)
    {
        var request = new GlobalSignOutRequest
        {
            AccessToken = accessToken,
        };

        try
        {
            await _cognitoIdentityProviderClient.GlobalSignOutAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error signing out: {ex.Message}");
        }
    }
}
