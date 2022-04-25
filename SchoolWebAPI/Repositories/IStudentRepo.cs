using SchoolWebAPI.ViewModels;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SchoolWebAPI.Repositories
{
    public interface IStudentRepo
    {
        IHttpActionResult GetAll();
        IHttpActionResult GetStudent(int id);
        IHttpActionResult GetStudentsByStandard(int id);
        IHttpActionResult Create(ModelStateDictionary modelState, StudentViewModel student);
        IHttpActionResult UpdateStudent(ModelStateDictionary modelState, int id, StudentViewModel student);
        IHttpActionResult DeleteStudent(int id);
        IHttpActionResult UploadDocument(int id);
        HttpResponseMessage DownloadDocument(int id);
    }
}
