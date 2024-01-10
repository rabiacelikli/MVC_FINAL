using System;

namespace uyg1.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }
        
        public int Id { get; set; }
        public int QuestionID { get; set; }
        public string ResponseText { get; set; }

        public AppUser AppUser { get; set; }
        public Question Question { get; set; }

        
    }
}
