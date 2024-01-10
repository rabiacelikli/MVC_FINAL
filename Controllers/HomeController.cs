using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Security.Claims;
using uyg1.Models;
using uyg1.ViewModels;

namespace uyg1.Controllers
{
   
        [Authorize]
        public class HomeController : Controller
        {
            private readonly ILogger<HomeController> _logger;

            private readonly UserManager<AppUser> _userManager;
            private readonly RoleManager<AppRole> _roleManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IFileProvider _fileProvider;
            private readonly AppDbContext _appDbContext;


            public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider = null, AppDbContext appDbContext = null)
            {
                _logger = logger;
                _userManager = userManager;
                _roleManager = roleManager;
                _signInManager = signInManager;
                _fileProvider = fileProvider;
                _appDbContext = appDbContext;
            }

            public IActionResult Index()
            {
                return View();
            }
            [AllowAnonymous]
            public IActionResult Login()
            {
                return View();
            }


            [AllowAnonymous]
            [HttpPost]
            public async Task<IActionResult> Login(LoginModel model)
            {

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz Kullanıcı Adı veya Parola!");
                    return View();
                }
                var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                if (signInResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Kullanıcı Girişi " + user.LockoutEnd + " kadar kısıtlanmıştır!");
                    return View();
                }
                ModelState.AddModelError("", "Geçersiz Kullanıcı Adı veya Parola Başarısız Giriş Sayısı :" + await _userManager.GetAccessFailedCountAsync(user) + "/3");
                return View();
            }

            [AllowAnonymous]
            public IActionResult Register()
            {
                return View();
            }
            [AllowAnonymous]
            [HttpPost]
            public async Task<IActionResult> Register(RegisterModel model)
            {


                if (!ModelState.IsValid)
                {
                    return View(model);

                }
                var identityResult = await _userManager.CreateAsync(new() { UserName = model.UserName, Email = model.Email, FullName = model.FullName, }, model.Password);

                if (!identityResult.Succeeded)
                {
                    foreach (var item in identityResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }

                    return View(model);
                }

                // default olarak Uye rolü ekleme
                var user = await _userManager.FindByNameAsync(model.UserName);
                var roleExist = await _roleManager.RoleExistsAsync("Uye");
                if (!roleExist)
                {
                    var role = new AppRole { Name = "Uye" };
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(user, "Uye");

                return RedirectToAction("Login");
            }
            public IActionResult AccessDenied()
            {
                return View();
            }


            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync(); // Oturumu sonlandır

                // Çıkış yapıldıktan sonra yönlendirilecek sayfa veya işlemler
                return RedirectToAction("Login");
            }

            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> GetUserList()
            {
                return View();
            }
            public IActionResult GetRoleList()
            {
                return View();
            }

            public IActionResult RoleAdd()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> RoleAdd(AppRole model)
            {
                var role = await _roleManager.FindByNameAsync(model.Name);
                if (role == null)
                {

                    var newrole = new AppRole();
                    newrole.Name = model.Name; ;
                    await _roleManager.CreateAsync(newrole);
                }
                return RedirectToAction("GetRoleList");
            }





            [Authorize(Roles = "Admin, Uye")]
            public async Task<IActionResult> UserPage()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {

                    return NotFound();
                }

                return View(user);
            }








        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Privacy()
            {
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
