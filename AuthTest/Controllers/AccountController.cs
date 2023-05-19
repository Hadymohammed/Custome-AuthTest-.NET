using AuthTest.Data;
using AuthTest.Models;
using AuthTest.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthTest.Controllers
{
    public class AccountController : Controller
    {
        private readonly Dbcontext _context;
        private IUserManager _userManager;
        public AccountController(Dbcontext context, IUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVM user)
        {
            if (!ModelState.IsValid)
                return View(user);
            if (user.Role == UserRoles.Admin)
            {
                Admin admin = _context.Admins.Where(ad => ad.username == user.Username && ad.password == user.Password).FirstOrDefault();
                if (admin != null)
                {
                    await _userManager.SignIn(HttpContext, admin);
                    return RedirectToAction("Admin");
                }
                return View(admin);
            }
            else if (user.Role == UserRoles.User)
            {
                User dbuser = _context.Users.Where(ad => ad.username == user.Username && ad.password == user.Password).FirstOrDefault();
                if (dbuser != null)
                {
                    await _userManager.SignIn(HttpContext, dbuser);
                    return RedirectToAction("User");

                }
                return View(dbuser);
            }


            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterVM user)
        {
            if (!ModelState.IsValid)
                return View(user);
            if (user.Role == UserRoles.User)
            {
                User dbUser = new User()
                {
                    Name = user.Name,
                    username = user.Username,
                    password = user.Password
                };
                _context.Users.Add(dbUser);
            }
            else if(user.Role == UserRoles.Admin)
            {
                Admin dbAdmin = new Admin()
                {
                    Name = user.Name,
                    username = user.Username,
                    password = user.Password
                };
                _context.Admins.Add(dbAdmin);

            }
            await _context.SaveChangesAsync();
            return Redirect("/Users");
        }
        public IActionResult Logout()
        {
            _userManager.SignOut(HttpContext);
            return RedirectToAction("Login");
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Admin()
        {
            return View();
        }
        [Authorize(Roles = "User")]
        public IActionResult User()
        {
            var user = _userManager.GetCurrentUser(HttpContext);
            return View(user);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
