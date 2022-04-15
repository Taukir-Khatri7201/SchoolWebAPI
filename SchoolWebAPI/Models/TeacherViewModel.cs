using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolWebAPI.Models
{
    public class TeacherViewModel
    {
        public int TeacherId { get; set; }
        [Required(ErrorMessage = "Name of the teacher is required!")]
        public string TeacherName { get; set; }
        public int? StandardId { get; set; }
        [Required(ErrorMessage = "Teacher type is required!")]
        public int? TeacherType { get; set; }

    }
}