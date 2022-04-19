using SchoolWebAPI.Models;
using System.Web.Http;
using SchoolWebAPI.Repositories;
using System.Web;
using System.Net.Http;
using System.Net;

namespace SchoolWebAPI.Controllers
{
    [RoutePrefix("api/student")]
    public class StudentController : ApiController
    {
        private readonly IStudentRepo studentRepo;

        public StudentController(IStudentRepo studentRepo)
        {
            this.studentRepo = studentRepo;
        }

        [HttpGet]
        [Route("~/api/student")]
        public IHttpActionResult AllStudentsDetail() => studentRepo.GetAll(Request);

        [HttpGet]
        [Route("~/api/getStudentWithId/{id:int:min(1)}")]
        public IHttpActionResult Get(int id) => studentRepo.GetStudent(Request, id);

        [HttpPost]
        [Route(Name = "Create")]
        public IHttpActionResult Post([FromBody] StudentViewModel model) => studentRepo.Create(Request, ModelState, model);

        [HttpPut]
        [Route("{id}/update")]
        public IHttpActionResult Put([FromUri] int id, [FromBody] StudentViewModel model) => studentRepo.UpdateStudent(Request, ModelState, id, model);

        [HttpDelete]
        [Route("{id}/remove")]
        public IHttpActionResult Delete([FromUri] int id) => studentRepo.DeleteStudent(Request, id);

        [HttpPost]
        [Route("upload/{id:int}")]
        public IHttpActionResult Upload([FromUri] int id) => studentRepo.UploadDocument(HttpContext.Current.Request, Request, ModelState, id);

        [HttpGet]
        [Route("download/{id:int}")]
        public HttpResponseMessage Download([FromUri] int id) => studentRepo.DownloadDocument(Request, id);
    }

    public enum ResultStatus
    {
        Success = 1,
        Failed,
        NotFound
    }
}
