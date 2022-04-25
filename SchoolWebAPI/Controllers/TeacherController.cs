using SchoolWebAPI.ViewModels;
using SchoolWebAPI.Repositories;
using System.Web.Http;
using SchoolWebAPI.Filters;

namespace SchoolWebAPI.Controllers
{
    [CustomAuthorizationFilter]
    [CustomLoggerActionFilter]
    public class TeacherController : ApiController
    {
        private readonly ITeacherRepo teacherRepo;

        public TeacherController(ITeacherRepo teacherRepo)
        {
            this.teacherRepo = teacherRepo;
        }

        /// <summary>
        /// Returns a list of all teachers if not provided standardId
        /// Returns a list of all teachers teaching in standardId provided
        /// </summary>
        /// <param name="standardId">standardId in which teacher is teaching</param>
        /// <returns>Get list of all teachers or teachers teaching in specific standard</returns>
        [HttpGet]
        public IHttpActionResult Get(int standardId = 0) => teacherRepo.Get(standardId);

        [HttpGet]
        [Route("api/teacher/getbyname/{name:alpha}")]
        public IHttpActionResult GetTeachersByName(string name) => teacherRepo.GetTeachersByName(name);

        [HttpGet]
        [Route("api/teacher/getbystandardid/{id:int}")]
        public IHttpActionResult GetCountOfTeachersInStandard(int id) => teacherRepo.GetCountOfTeachersByStandardId(id);

        [HttpGet]
        [Route("api/teacher/getCountByStandard")]
        public IHttpActionResult GetTeacherCountByStandard() => teacherRepo.GetCountByStandard();

        [HttpPost]
        public IHttpActionResult Post([FromBody] TeacherViewModel model = default) => teacherRepo.Create(ModelState, model);
        [HttpPut]
        public IHttpActionResult Put([FromUri] int id, [FromBody] TeacherViewModel model) => teacherRepo.UpdateTeacher(ModelState, id, model);
        [HttpDelete]
        public IHttpActionResult Delete([FromUri] int id) => teacherRepo.DeleteTeacher(id);

        /*
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IHttpActionResult Get(int standardId=0)
        {
            try
            {
                IList<TeacherViewModel> list = new List<TeacherViewModel>();
                using(var context = new SchoolDBEntities())
                {
                    switch(standardId)
                    {
                        case 0: 
                                list = context.Teachers.Select(t => new TeacherViewModel()
                                {
                                    TeacherId = t.TeacherId,
                                    TeacherName = t.TeacherName,
                                    TeacherType = (int)t.TeacherType,
                                    StandardId = (int)t.StandardId,
                                }).ToList();
                                break;
                        default:
                            list = context.Teachers.
                                Where(t => t.StandardId == standardId).
                                Select(t => new TeacherViewModel()
                                {
                                    TeacherId = t.TeacherId,
                                    TeacherName = t.TeacherName,
                                    TeacherType = (int)t.TeacherType,
                                    StandardId = (int)t.StandardId,
                                }).ToList();
                                break;

                    }
                    if(standardId == 0)
                    {
                        logger.Info("GET | Requested for list of all teachers");
                    }
                    else
                    {
                        logger.Info("GET | Requested for list of all teachers from standard " + standardId);
                    }
                    if (list.Count == 0)
                    {
                        return new CustomResponse<string>(Request, (int)ResultStatus.Success, "No teacher records found!");
                    }
                    return new CustomResponse<IList<TeacherViewModel>>(Request, (int)ResultStatus.Success, "", list);
                }
            }
            catch(Exception ex)
            {
                logger.Error("GET | " + ex.Message);
                return new CustomResponse<string>(Request, (int)ResultStatus.Failed, ex.Message);
            }
        }

        public IHttpActionResult Post([FromBody] TeacherViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    using (var ctx = new SchoolDBEntities())
                    {
                        if (model.StandardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.StandardId))
                        {
                            logger.Error("POST | Addition of the teacher data with StandardId=" + model.StandardId + " is violating the foreign key constraint!");
                            return new CustomResponse<string>(Request, (int)ResultStatus.Failed, "StandardId=" + model.StandardId + " is violating the constraints!");
                        }
                        ctx.Teachers.Add(new Teacher
                        {
                            TeacherName = model.TeacherName,
                            StandardId = model.StandardId,
                            TeacherType = model.TeacherType,
                        });
                        ctx.SaveChanges();
                        logger.Info("POST | New teacher data added!");
                        return new CustomResponse<string>(Request, (int)ResultStatus.Success, "Teacher details inserted successfully!");
                    }
                }
                catch(Exception ex)
                {
                    logger.Error("Post | " + ex.Message);
                    return new CustomResponse<string>(Request, (int)ResultStatus.Failed, ex.Message);
                }
            }
            else
            {
                logger.Error("POST | Addition of teacher data violating the modal state!");
                var errors = GetModelStateErrors(ModelState);
                return new CustomResponse<string>(Request, (int)ResultStatus.Failed, errors, "");
            }
        }

        public IHttpActionResult Put([FromUri] int id, [FromBody] TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var ctx = new SchoolDBEntities())
                    {
                        var oldData = ctx.Teachers.FirstOrDefault(s => s.TeacherId == id);
                        if (oldData == null)
                        {
                            logger.Info("PUT | Updation of teacher information with id=" + id + " not found!");
                            return new CustomResponse<string>(Request, (int)ResultStatus.NotFound, "Teacher with Id=" + id.ToString() + " not found!");
                        }
                        if (model.StandardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.StandardId))
                        {
                            logger.Error("PUT | Updation of the teacher data with StandardId=" + model.StandardId + " is violating the foreign key constraint!");
                            return new CustomResponse<string>(Request, (int)ResultStatus.Failed, "StandardId=" + model.StandardId + " is violating the constraints!");
                        }
                        oldData.TeacherName = model.TeacherName;
                        oldData.StandardId = model.StandardId;
                        oldData.TeacherType = model.TeacherType;
                        ctx.SaveChanges();
                        logger.Info("PUT | Teacher details updated successfully for id=" + id);
                        return new CustomResponse<string>(Request, (int)ResultStatus.Success, "Teacher details updated successfully!");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PUT | " + ex.Message);
                    return new CustomResponse<string>(Request, (int)ResultStatus.Failed, ex.Message, "");
                }
            }
            else
            {
                logger.Error("PUT | Updation of teacher data violating the modal state!");
                var errors = GetModelStateErrors(ModelState);
                return new CustomResponse<string>(Request, (int)ResultStatus.Failed, errors);
            }
        }

        public IHttpActionResult Delete([FromUri] int id)
        {
            try
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var data = ctx.Teachers.FirstOrDefault(s => s.TeacherId == id);
                    if (data == null)
                    {
                        logger.Error("DELETE | Can not delete teacher with id=" + id + " which does not exist.");
                        return new CustomResponse<string>(Request, (int)ResultStatus.NotFound, "Teacher with Id=" + id.ToString() + " not found!");
                    }
                    ctx.Teachers.Remove(data);
                    ctx.SaveChanges();
                    logger.Info("DELETE | Deleted teacher record with id=" + id);
                    return new CustomResponse<string>(Request, (int)ResultStatus.Success, "Teacher with Id=" + id.ToString() + " has been removed successfully!");
                }
            }
            catch (Exception ex)
            {
                logger.Error("DELETE | " + ex.Message);
                return new CustomResponse<string>(Request, (int)ResultStatus.Failed, ex.Message);
            }
        }


        [NonAction]
        public List<string> GetModelStateErrors(ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            return errors;
        }
        */
    }
}
