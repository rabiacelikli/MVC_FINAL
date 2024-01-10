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
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public QuestionController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult QuestionListAjax(int surveyId)
        {
            var questions = _context.Questions
                .Where(q => q.SurveyID == surveyId)
                .Include(q => q.Answers)
                .ToList();

            var questionModels = questions.Select(x => new QuestionModel()
            {
                QuestionID = x.QuestionID,
                SurveyID = x.SurveyID,
                Text = x.Text,
                Type = x.Type,
                IsMultipleChoice = x.IsMultipleChoice,
                Answers = x.Answers?.Select(a => new AnswerModel
                {
                    AnswerID = a.AnswerID,
                    ResponseText = a.ResponseText,
                    Id = a.Id,
                    QuestionID = a.QuestionID
                }).ToList(),
                Options = x.Options?.Select(o => o.Text).ToList()
            }).ToList();

            return Json(questionModels);
        }

        [AllowAnonymous]
        public IActionResult QuestionByIdAjax(int id)
        {
            var question = _context.Questions
                .Include(q => q.Options)
                .FirstOrDefault(q => q.QuestionID == id);

            if (question == null)
            {
                return Json(new { status = false, message = "Soru bulunamadı!" });
            }

            var questionModel = new QuestionModel()
            {
                QuestionID = question.QuestionID,
                SurveyID = question.SurveyID,
                Text = question.Text,
                Type = question.Type,
                IsMultipleChoice = question.IsMultipleChoice,
                Answers = question.Answers?.Select(a => new AnswerModel
                {
                    AnswerID = a.AnswerID,
                    ResponseText = a.ResponseText,
                    Id = a.Id,
                    QuestionID = a.QuestionID
                }).ToList(),
                Options = question.Options?.Select(o => o.Text).ToList()
            };

            return Json(questionModel);
        }

        [HttpPost]
        public async Task<IActionResult> QuestionAddEditAjax(QuestionModel model)
        {
            try
            {
                var existingSurvey = await _context.Surveys
                    .FirstOrDefaultAsync(s => s.SurveyID == model.SurveyID);

                if (existingSurvey == null)
                {
                    return Json(new { status = false, message = "Anket bulunamadı!" });
                }

                if (model.QuestionID == 0)
                {
                    var question = new Question
                    {
                        Text = model.Text,
                        Type = model.Type,
                        IsMultipleChoice = model.IsMultipleChoice,
                        Survey = existingSurvey
                    };

                    // Options ekleyebilirsiniz, gerekirse
                    if (model.Options != null)
                    {
                        question.Options = model.Options.Select(o => new Option { Text = o }).ToList();
                    }

                    _context.Questions.Add(question);
                }
                else
                {
                    var existingQuestion = _context.Questions
                        .Include(q => q.Options)
                        .FirstOrDefault(q => q.QuestionID == model.QuestionID);

                    if (existingQuestion != null)
                    {
                        existingQuestion.Text = model.Text;
                        existingQuestion.Type = model.Type;
                        existingQuestion.IsMultipleChoice = model.IsMultipleChoice;

                        // Options güncelleme
                        if (model.Options != null)
                        {
                            existingQuestion.Options = model.Options.Select(o => new Option { Text = o }).ToList();
                        }
                        else
                        {
                            existingQuestion.Options = null;
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Soru bulunamadı!" });
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
        public async Task<IActionResult> QuestionRemoveAjax(int id)
        {
            try
            {
                var existingQuestion = await _context.Questions
                    .Include(q => q.Options)
                    .FirstOrDefaultAsync(q => q.QuestionID == id);

                if (existingQuestion != null)
                {
                    _context.Questions.Remove(existingQuestion);
                    await _context.SaveChangesAsync();
                    return Json(new { status = true, message = "Soru silindi!" });
                }
                else
                {
                    return Json(new { status = false, message = "Soru bulunamadı!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
