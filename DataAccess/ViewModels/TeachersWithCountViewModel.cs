using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataAccess.ViewModels
{
    public class TeachersWithCountViewModel
    {
        public int Count { get; set; }
        [JsonProperty(PropertyName = "List")]
        public List<TeacherViewModel> ListOfTeachers { get; set; }
    }
}