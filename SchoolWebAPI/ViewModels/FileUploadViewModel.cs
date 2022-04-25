using SchoolWebAPI.Utility;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebAPI.ViewModels
{
    public class FileUploadViewModel
    {
        [FileValidation]
        public HttpPostedFile file { get; set; }
    }
}