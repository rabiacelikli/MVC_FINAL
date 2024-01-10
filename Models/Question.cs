using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace uyg1.Models
{
    

    public class Question
    {
        public int QuestionID { get; set; }

        public int Id { get; set; }
        public int SurveyID { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public bool IsMultipleChoice { get; set; }

        public Survey Survey { get; set; }
        public List<Option> Options { get; set; }
        public List<Answer> Answers { get; set; }

        public AppUser AppUser { get; set; }
    }
}
