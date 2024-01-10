using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using uyg1.Models;
using uyg1.ViewModels;

namespace uyg1.Controllers
{
   
    public class SurveyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SurveyController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine($"Survey ID: {SurveyByIdAjax}");
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

        [HttpPost]
        public async Task<IActionResult> SurveyAddEditAjax(SurveyModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (model.SurveyID == 0)
                {
                    // Yeni anket eklemek için
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
                    // Mevcut anketi güncellemek için
                    var existingSurvey = await _context.Surveys.FirstOrDefaultAsync(s => s.SurveyID == model.SurveyID);
                    if (existingSurvey != null)
                    {
                        existingSurvey.Title = model.Title;
                        existingSurvey.Description = model.Description;
                        existingSurvey.CategoryID = model.CategoryID;
                    }
                    else
                    {
                        return Json(new { status = false, message = "Anket bulunamadı!" });
                    }
                }

                await _context.SaveChangesAsync();

                // Formu temizle
                ModelState.Clear();

                return Json(new { status = true, message = "İşlem başarılı!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
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
