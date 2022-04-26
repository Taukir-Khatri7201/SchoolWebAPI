using SchoolWebAPI.ViewModels;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SchoolWebAPI.Repositories
{
    public interface ITeacherRepo
    {
        IHttpActionResult Get(int standardId);
        IHttpActionResult GetTeachersByName(string name);
        IHttpActionResult GetCountOfTeachersByStandardId(int id);
        IHttpActionResult GetCountByStandard();
        IHttpActionResult GetStudentMarks();
        IHttpActionResult GetStudentWithMaxMarksInEachCourse();
        IHttpActionResult Create(ModelStateDictionary modelState, TeacherViewModel model);
        IHttpActionResult UpdateTeacher(ModelStateDictionary modelState, int id, TeacherViewModel model);
        IHttpActionResult DeleteTeacher(int id);
    }
}
