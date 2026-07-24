namespace DotNetApiStarterKit.Business.Interfaces.Auths
{
    public interface ICurrentUserService
    {
        int? GetUserId();

        string? GetEmail();

        string? GetRole();
    }
}
