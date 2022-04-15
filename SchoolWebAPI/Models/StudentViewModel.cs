using System.ComponentModel.DataAnnotations;

namespace SchoolWebAPI.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name of student is required!")]
        public string Name { get; set; }
        public int? standardId { get; set; }
    }
}