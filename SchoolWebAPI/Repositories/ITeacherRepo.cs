using SchoolWebAPI.Models;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SchoolWebAPI.Repositories
{
    public interface ITeacherRepo
    {
        IHttpActionResult Get(HttpRequestMessage request, int standardId);
        IHttpActionResult Create(HttpRequestMessage request, ModelStateDictionary modelState, TeacherViewModel model);
        IHttpActionResult UpdateTeacher(HttpRequestMessage request, ModelStateDictionary modelState, int id, TeacherViewModel model);
        IHttpActionResult DeleteTeacher(HttpRequestMessage request, int id);
    }
}
