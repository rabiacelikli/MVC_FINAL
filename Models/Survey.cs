using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace uyg1.Models
{
    public class Survey
    {
        public int SurveyID { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int CategoryID { get; set; }

        public AppUser AppUser { get; set; }
        public List<Question> Questions { get; set; }

        public Category Category { get; set; }
    }
}
