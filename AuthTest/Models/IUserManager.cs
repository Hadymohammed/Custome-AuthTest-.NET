using System.Data;

namespace AuthTest.Models
{
    public interface IUserManager
    {
        Task SignIn<T>(HttpContext httpContext, T user, bool isPersistent = false) where T : IAbstractUser;
        Task SignOut(HttpContext httpContext);
        int GetCurrentUserId(HttpContext httpContext);
        IAbstractUser GetCurrentUser(HttpContext httpContext);
    }
}
