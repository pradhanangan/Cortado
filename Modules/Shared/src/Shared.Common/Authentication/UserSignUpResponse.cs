namespace Shared.Common.Authentication;

public class UserSignUpResponse
{
    public bool UserConfirmed { get; set; }
    public Guid UserSub { get; set; }
}
