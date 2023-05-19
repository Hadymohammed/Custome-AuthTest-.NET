using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AuthTest.Data;

namespace AuthTest.Models
{
    public class UserManager : IUserManager
    {
        readonly Dbcontext _context;
        public UserManager(Dbcontext context)
        {
            _context = context;
        }
        public IAbstractUser GetCurrentUser(HttpContext httpContext)
        {
            int currentUserId = this.GetCurrentUserId(httpContext);
            string role= this.GetUserRole(httpContext);

            if (currentUserId == -1)
                return null;
            if (role == null)
                return null;
            IAbstractUser user;
            if (role == UserRoles.User)
                user = _context.Users.Where(user => user.Id == currentUserId).FirstOrDefault();
            else if (role == UserRoles.Admin)
                user = _context.Admins.Where(user => user.Id == currentUserId).FirstOrDefault();
            else
                user = null;
            return user;
        }

        public string GetUserRole(HttpContext httpContext)
        {
            var role = httpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;
            return role;
        }
        public int GetCurrentUserId(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return -1;

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
                return -1;

            int currentUserId;

            if (!int.TryParse(claim.Value, out currentUserId))
                return -1;

            return currentUserId;
        }

        private IEnumerable<Claim> GetUserClaims<T>(T user) where T : IAbstractUser
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim("Username", user.username));
            if (user is User)
                claims.Add(new Claim(ClaimTypes.Role, UserRoles.User));
            else if(user is Admin)
                claims.Add(new Claim(ClaimTypes.Role, UserRoles.Admin));
            return claims;
        }
        public async Task SignIn<T>(HttpContext httpContext, T user, bool isPersistent = false) where T : IAbstractUser
        {
            ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims<T>(user), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = isPersistent }
            );
        }

        public async Task SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
