using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebAPI.ViewModels
{
    public class TeachersWithCountViewModel
    {
        public int Count { get; set; }
        [JsonProperty(PropertyName = "List")]
        public List<TeacherViewModel> ListOfTeachers { get; set; }
    }
}