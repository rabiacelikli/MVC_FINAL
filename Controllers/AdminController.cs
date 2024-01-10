using AspNetCoreHero.ToastNotification.Abstractions;
using uyg1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NuGet.Protocol;
using uyg1.ViewModels;
using System.Security.Claims;

namespace uyg1.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly IConfiguration _config;
        private readonly IFileProvider _fileProvider;

        public AdminController(AppDbContext appDbContext, IConfiguration config, IFileProvider fileProvider, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {

            _context = appDbContext;
            _config = config;
            _fileProvider = fileProvider;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AdminUser()
        {
            
            return View();
            
        }

        public async Task< IActionResult> GetRoleList()
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

        public IActionResult Category()
        {
            
            return View();
        }

        public IActionResult CategoryListAjax(object categoryViewModel)
        {
            var categoryViewModels = _context.Categories.Select(x => new  CategoryViewModel()
            {
                CategoryID = x.CategoryID,
                Name = x.Name,
                
            }).ToList();

            return Json(categoryViewModel);
        }
        public IActionResult CategoryByIdAjax(int CategoryID)
        {
            var categoryViewModel = _context.Categories.Where(s => s.CategoryID == CategoryID).Select(x => new CategoryViewModel()
            {
                CategoryID = x.CategoryID,
                Name = x.Name,
            }).SingleOrDefault();

            return Json(categoryViewModel);
        }

        [HttpPost]
        public IActionResult CategoryAddAjax(CategoryViewModel model)
        {
            var sonuc = new SonucModel();
            if (model.CategoryID == 0)
            {
                if (_context.Categories.Count(c => c. Name == model.Name) > 0)
                {
                    sonuc.Status = false;
                    sonuc.Message = "Girilen Kategori Kayıtlıdır!";
                    return Json(sonuc);
                }

                var category = new Category();
                category.CategoryID = model.CategoryID;
                category.Name = model.Name;
                
                _context.Categories.Add(category);
                _context.SaveChanges();
                sonuc.Status = true;
                sonuc.Message = "Kategori Eklendi";
            }
            else
            {
                var category = _context.Categories.FirstOrDefault(x => x.CategoryID == model.CategoryID);
                category.CategoryID = model.CategoryID;
                category.Name = model.Name;
                _context.SaveChanges();
                sonuc.Status = true;
                sonuc.Message = "İşlem Güncellendi";
            }

            return Json(sonuc);
        }

        public IActionResult LessonRemoveAjax(int CategoryID)
        { 
            var category = _context.Categories.FirstOrDefault(x => x.CategoryID == CategoryID);
            _context.Categories.Remove(category);
            _context.SaveChanges();

            var sonuc = new SonucModel();
            sonuc.Status = true;
            sonuc.Message = "İşlem Silindi";
            return Json(sonuc);
        }

        public IActionResult Survey()
        {
            return View();
        }



        [AllowAnonymous]
        public IActionResult SurveyListAjax()
        {
            var surveys = _context.Surveys.Include(s => s.AppUser).ToList();
            var surveyModels = surveys.Select(x => new SurveyModel()
            {
                SurveyID = x.SurveyID,
                UserId = x.AppUser.Id,
                Title = x.Title,
                Description = x.Description,
                CategoryID = x.CategoryID,
            }).ToList();

            return Json(surveyModels);
        }

        [AllowAnonymous]
        public IActionResult SurveyByIdAjax(int id)
        {
            var survey = _context.Surveys.Include(s => s.AppUser).FirstOrDefault(s => s.SurveyID == id);

            if (survey == null)
            {
                return Json(new { status = false, message = "Anket bulunamadı!" });
            }

            var surveyModel = new SurveyModel()
            {
                SurveyID = survey.SurveyID,
                UserId = survey.AppUser.Id,
                Title = survey.Title,
                Description = survey.Description,
                CategoryID = survey.CategoryID,
            };

            return Json(surveyModel);
        }

        [HttpPost]
        public async Task<IActionResult> SurveyAddEditAjax(SurveyModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var existingSurvey = await _context.Surveys.FirstOrDefaultAsync(s => s.SurveyID == model.SurveyID);
                if (existingSurvey == null)
                {
                    var survey = new Survey
                    {
                        Title = model.Title,
                        Description = model.Description,
                        CategoryID = model.CategoryID,
                        AppUser = user
                    };

                    _context.Surveys.Add(survey);
                }
                else
                {
                    existingSurvey.Title = model.Title;
                    existingSurvey.Description = model.Description;
                    existingSurvey.CategoryID = model.CategoryID;
                }

                await _context.SaveChangesAsync();
                return Json(new { status = true, message = "İşlem başarılı!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [AllowAnonymous]

        public async Task<IActionResult> SurveyRemoveAjax(int id)
        {
            try
            {
                var existingSurvey = await _context.Surveys.FirstOrDefaultAsync(s => s.SurveyID == id);
                if (existingSurvey != null)
                {
                    _context.Surveys.Remove(existingSurvey);
                    await _context.SaveChangesAsync();
                    return Json(new { status = true, message = "Anket silindi!" });
                }
                else
                {
                    return Json(new { status = false, message = "Anket bulunamadı!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }






    }
}
