using SchoolWebAPI.Utility;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebAPI.Models
{
    public class FileUploadViewModel
    {
        [FileValidation]
        public HttpPostedFile file { get; set; }
    }
}