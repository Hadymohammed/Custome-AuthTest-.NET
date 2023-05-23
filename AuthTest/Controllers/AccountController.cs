using AuthTest.Data;
using AuthTest.Models;
using AuthTest.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace AuthTest.Controllers
{
    public class AccountController : Controller
    {
        private readonly Dbcontext _context;
        private IUserManager _userManager;

        private string? recipient;
        public string? APIKey { get; private set; }
        private string ValidOTP { get; set; }
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
            var stringfyUser = JsonConvert.SerializeObject(user);
            HttpContext.Session.SetString("User", stringfyUser);
            return RedirectToAction("OTP");
        }
        public IActionResult Logout()
        {
            _userManager.SignOut(HttpContext);
            return RedirectToAction("Login");
        }
        [Authorize(Roles =UserRoles.Admin)]
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

        //*OTP*//
        public IActionResult OTP()
        {
            int otpValue = new Random().Next(100000, 999999);
            HttpContext.Session.SetString("OTP", otpValue.ToString());
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> OTP(OTPVM receivedOTP)
        {
            bool result = false;
            var ValidOTP = HttpContext.Session.GetString("OTP");
            if (ValidOTP == null)
                return View(receivedOTP);

            if (receivedOTP.OTP == ValidOTP)
            {
                var stringfyUser = HttpContext.Session.GetString("User");
                if (stringfyUser == null)
                    return View(receivedOTP);

                RegisterVM _registeredUser = (RegisterVM)JsonConvert.DeserializeObject<RegisterVM>(stringfyUser);
                result = true;
                if (_registeredUser.Role == UserRoles.User)
                {
                    User dbUser = new User()
                    {
                        Name = _registeredUser.Name,
                        username = _registeredUser.Username,
                        password = _registeredUser.Password
                    };
                    await _context.Users.AddAsync(dbUser);
                    await _context.SaveChangesAsync();
                    await _userManager.SignIn(HttpContext, dbUser);
                    return RedirectToAction("User");

                }
                else if (_registeredUser.Role == UserRoles.Admin)
                {
                    Admin dbAdmin = new Admin()
                    {
                        Name = _registeredUser.Name,
                        username = _registeredUser.Username,
                        password = _registeredUser.Password
                    };
                    await _context.Admins.AddAsync(dbAdmin);
                    await _context.SaveChangesAsync();
                    await _userManager.SignIn(HttpContext, dbAdmin);
                    return RedirectToAction("Admin");
                }
            }
            
            result = false;
            receivedOTP.valid = false;
            return View(receivedOTP);
        }
    }
}
