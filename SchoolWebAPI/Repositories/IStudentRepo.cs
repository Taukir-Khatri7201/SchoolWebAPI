using SchoolWebAPI.Models;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SchoolWebAPI.Repositories
{
    public interface IStudentRepo
    {
        IHttpActionResult GetAll(HttpRequestMessage request);
        IHttpActionResult GetStudent(HttpRequestMessage request, int id);
        IHttpActionResult GetStudentsByStandard(HttpRequestMessage request, int id);
        IHttpActionResult Create(HttpRequestMessage request, ModelStateDictionary modelState, StudentViewModel student);
        IHttpActionResult UpdateStudent(HttpRequestMessage request, ModelStateDictionary modelState, int id, StudentViewModel student);
        IHttpActionResult DeleteStudent(HttpRequestMessage request, int id);
        IHttpActionResult UploadDocument(HttpRequest request, HttpRequestMessage requestMessage, ModelStateDictionary modelState, int id);
        HttpResponseMessage DownloadDocument(HttpRequestMessage request, int id);
    }
}
