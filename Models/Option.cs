namespace uyg1.Models
{
    public class Option
    {
        public int OptionID { get; set; }
        public int QuestionID { get; set; }
        public string Text { get; set; }

        public Question Question { get; set; }
    }
}
