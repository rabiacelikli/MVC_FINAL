using System.Collections.Generic;
using uyg1.Models;

namespace uyg1.ViewModels
{
    public class QuestionListViewModel
    {
        public int SurveyID { get; set; }
        public List<Question> Questions { get; set; }
    }
}