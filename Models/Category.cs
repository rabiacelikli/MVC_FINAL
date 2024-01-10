namespace uyg1.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }

        public List<Survey> Surveys { get; set; }
    }
}
