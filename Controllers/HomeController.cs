using Blogie.Auth.Models;
using Blogie.Auth.Repository;
using Blogie.UI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Blogie.Home.Data.Repository;
using Blogie.Blogger.Data.Models;
using Blogie.Home.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Blogie.UI.Services;


namespace Blogie.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthRepository _AuthRepo;
        private readonly IAllHomeRepository _homeRepo;
        private readonly IUserInfoService _service;
        public HomeController(ILogger<HomeController> logger, IAuthRepository auth, IAllHomeRepository homeRepo, IUserInfoService service)
        {
            _logger = logger;
            _AuthRepo = auth;
            _homeRepo = homeRepo;
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<AllPost> posts = await _homeRepo.DisplayAllPost();
            return View(posts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            Login login = new Login();
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            
            string controller = "";
            if (ModelState.IsValid)
            {
                User user = await _AuthRepo.LoginUser(login);

                if (user == null)
                {
                    ViewBag.Message = "Invalid Username/Email or Password";
                    return View(login);
                }
                else
                {
                    if (user.Role == "Blogger")
                    {
                        login.ReturnUrl = "~/Blogger/Index";
                        
                        controller = "Blogger";
                    }
                    else
                    {
                        login.ReturnUrl = "~/Admin/Index";
                       
                        controller = "Admin";
                    }

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserId)),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                        new AuthenticationProperties()
                        {
                            IsPersistent = login.RememberMe
                        });


                    return LocalRedirect(login.ReturnUrl);
                }
            }
            return View(login);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register register, IFormFile file)
        {
            byte[] fileData = null;
            if (file != null && file.Length != 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileData = ms.ToArray();
                }

                register.Avatar = fileData;
            }
            else
            {
                ModelState.AddModelError("Avatar", "Please select an Picture for yourself");
            }

            if (ModelState.IsValid)
            {
                if (register.RepeatPassword == register.Password)
                {
                    await _AuthRepo.RegisterUser(register);
                    return RedirectToAction("Login"); 
                }
                else
                {
                    ModelState.AddModelError("RepeatPassword", "Passwords entered does not match");
                    return View(register);
                }

            }

            return View(register);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }

       
        public async Task<IActionResult> LikePost(int id)
        {
            int UserID = _service.GetLogUserId();
            await _homeRepo.LikePost(UserID, id);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> PostDetail(int id)
        {
            if (id != null || id != 0)
            {
                AllPost post = await _homeRepo.ViewPost(id);
                return View(post);
            }

            return RedirectToAction("Index");
        }


    }
}
