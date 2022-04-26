using DataAccess.Utility;
using System.Web;

namespace DataAccess.ViewModels
{
    public class FileUploadViewModel
    {
        [FileValidation]
        public HttpPostedFile file { get; set; }
    }
}