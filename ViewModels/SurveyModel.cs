using uyg1.Models;

namespace uyg1.ViewModels
{
    public class SurveyModel
    {
        public int SurveyID { get; set; }
        public int Id { get; set; }  
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CategoryName { get; set; }

        public int CategoryID { get; set; }


        public AppUser AppUser { get; set; }
        public string UserId { get; internal set; }
    }
}
