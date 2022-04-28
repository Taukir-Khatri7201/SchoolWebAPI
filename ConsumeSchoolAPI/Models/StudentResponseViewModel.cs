using DataAccess.ViewModels;
using System.Collections.Generic;

namespace ConsumeSchoolAPI.Models
{
    public class StudentResponseViewModel
    {
        public List<StudentViewModel> Data { get; set; }
        public int StatusCode { get; set; }
        public List<string> messages { get; set; }
    }
}
