namespace Shared.Common.Authentication;

public interface IUserManagement
{
    Task<Guid> CreateUserAsync(string email, string password);
    Task AddUserToGroupAsync(string email, string group);
    Task ChangePasswordAsync(string accessToken, string oldPassword, string newPassword);
    Task<UserSignUpResponse> SignUpAsync(string email, string password);
    Task ConfirmSignUpAsync(string email, string confirmationCode);
    Task<UserSignInResponse> SignInAsync(string email, string password);
    Task SignOutAsync(string accessToken);
}
