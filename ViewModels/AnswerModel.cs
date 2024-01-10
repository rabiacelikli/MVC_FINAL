using System;
using System.ComponentModel.DataAnnotations;

namespace uyg1.ViewModels
{
    public class AnswerModel
    {
        public int AnswerID { get; set; }

        [Required(ErrorMessage = "Cevap metni zorunludur.")]
        public string ResponseText { get; set; }

        public int Id { get; set; }

        public int QuestionID { get; set; }
    }
}
