using DataAccess.ViewModels;
using System.Web.Http;
using DataAccess.Repositories;
using System.Net.Http;
using SchoolWebAPI.Filters;

namespace SchoolWebAPI.Controllers
{
    [RoutePrefix("api/student")]
    [CustomLoggerActionFilter]
    public class StudentController : ApiController
    {
        private readonly IStudentRepo studentRepo;

        public StudentController(IStudentRepo studentRepo)
        {
            this.studentRepo = studentRepo;
        }

        [HttpGet]
        [Route("~/api/student")]
        public IHttpActionResult AllStudentsDetail() => studentRepo.GetAll();

        [HttpGet]
        [Route("~/api/getStudentWithId/{id:int:min(1)}")]
        public IHttpActionResult Get(int id) => studentRepo.GetStudent(id);

        [HttpPost]
        [CustomAuthorizationFilter]
        public IHttpActionResult Post([FromBody] StudentViewModel model) => studentRepo.Create(ModelState, model);

        [HttpPut]
        [CustomAuthorizationFilter]
        [Route("{id}/update")]
        public IHttpActionResult Put([FromUri] int id, [FromBody] StudentViewModel model) => studentRepo.UpdateStudent(ModelState, id, model);

        [HttpDelete]
        [CustomAuthorizationFilter]
        [Route("{id}/remove")]
        public IHttpActionResult Delete([FromUri] int id) => studentRepo.DeleteStudent(id);

        [HttpPost]
        [CustomAuthorizationFilter]
        [Route("upload/{id:int}")]
        public IHttpActionResult Upload([FromUri] int id) => studentRepo.UploadDocument(id);

        [HttpGet]
        [Route("download/{id:int}")]
        public HttpResponseMessage Download([FromUri] int id) => studentRepo.DownloadDocument(id);
    }

    public enum ResultStatus
    {
        Success = 1,
        Failed,
        NotFound
    }
}
