using uyg1.Models;

namespace uyg1.ViewModels
{
    public class QuestionModel
    {
        public int QuestionID { get; set; }

        public int Id { get; set; }
        public int SurveyID { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }

        public bool IsMultipleChoice { get; set; }

        public  List<AnswerModel> ?Answers { get; set; }
        public List<string> Options { get; internal set; }

        public AppUser AppUser { get; set; }
    }
}
