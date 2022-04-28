using System.ComponentModel.DataAnnotations;

namespace DataAccess.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name of student is required!")]
        public string Name { get; set; }
        [RegularExpression(@"^(1[0-2]|[1-9])$", ErrorMessage = "Invalid standard")]
        public int? standardId { get; set; }
    }
}