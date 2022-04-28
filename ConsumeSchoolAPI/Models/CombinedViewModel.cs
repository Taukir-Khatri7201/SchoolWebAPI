using DataAccess.ViewModels;

namespace ConsumeSchoolAPI.Models
{
    public class CombinedViewModel
    {
        public StudentResponseViewModel studentData { get; set; }
        public ErrorViewModel ErrorViewModel { get; set; }
        public StudentViewModel studentViewModel { get; set; }
        public StudentViewModel addStudent { get; set; }
    }
}
