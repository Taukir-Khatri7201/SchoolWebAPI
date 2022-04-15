using SchoolWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NLog;
using SchoolWebAPI.Utility;

namespace SchoolWebAPI.Controllers
{
    public class StudentController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //[HttpGet]
        public IHttpActionResult Get()
        {
            IList<StudentViewModel> students = null;

            using (var ctx = new SchoolDBEntities())
            {
                students = ctx.Students
                            .Select(s => new StudentViewModel()
                            {
                                Id = s.StudentID,
                                Name = s.StudentName,
                                standardId = s.StandardId,
                            }).ToList<StudentViewModel>();
            }
            logger.Info("GET | Requested for the list of all students");
            if (students.Count == 0)
            {
                return new CustomResponse<string,string>(Request, (int)ResultStatus.Success, "No student Records found!");
            }

            return new CustomResponse<IList<StudentViewModel>, string>(Request, (int)ResultStatus.Success, "", students);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            StudentViewModel student = null;

            using (var ctx = new SchoolDBEntities())
            {
                student = ctx.Students
                            .Where(s => s.StudentID == id)
                            .Select(s => new StudentViewModel()
                            {
                                Id = s.StudentID,
                                Name = s.StudentName,
                                standardId = s.StandardId,
                            }).FirstOrDefault();
            }
            if (student == null)
            {
                logger.Info("GET | Requested information of student with id=" + id + " not found!");
                return new CustomResponse<string, string>(Request, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
            }
            logger.Info("GET | Requested the information of student with id=" + id);
            return new CustomResponse<StudentViewModel, string>(Request, (int)ResultStatus.Success, "", student);
        }

        public IHttpActionResult Post([FromBody] StudentViewModel model)
        {
            if(ModelState.IsValid)
            {
                using(var ctx = new SchoolDBEntities())
                {
                    if(model.standardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.standardId))
                    {
                        logger.Error("POST | Addition of the student data with StandardId=" + model.standardId + " is violating the foreign key constraint!");
                        return new CustomResponse<string, string>(Request, (int)ResultStatus.Failed, "StandardId=" + model.standardId + " is violating the constraints!");
                    }
                    ctx.Students.Add(new Student
                    {
                        StudentName = model.Name,
                        StandardId = model.standardId,
                    });
                    ctx.SaveChanges();
                    logger.Info("POST | New student data added");
                    return new CustomResponse<string, string>(Request, (int)ResultStatus.Success, "Student details inserted successfully!");
                }
            }
            else
            {
                var errors = GetModelStateErrors(ModelState);
                logger.Error("POST | Addition of the student data violating the modal state!");
                return new CustomResponse<string, List<string>>(Request, (int)ResultStatus.Failed, errors, "");
            }
        }

        public IHttpActionResult Put([FromUri] int id, [FromBody] StudentViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    using(var ctx = new SchoolDBEntities())
                    {
                        var oldData = ctx.Students.FirstOrDefault(s => s.StudentID == id);
                        if(oldData == null)
                        {
                            logger.Info("PUT | Updation of student information with id=" + id + " not found!");
                            return new CustomResponse<string, string>(Request, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
                        }
                        if (model.standardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.standardId))
                        {
                            logger.Error("PUT | Updation of the student data with StandardId=" + model.standardId + " is violating the foreign key constraint!");
                            return new CustomResponse<string, string>(Request, (int)ResultStatus.Failed, "StandardId=" + model.standardId + " is violating the constraints!");
                        }
                        oldData.StudentName = model.Name;
                        oldData.StandardId = model.standardId;
                        ctx.SaveChanges();
                        logger.Info("PUT | Student details updated successfully for id=" + id);
                        return new CustomResponse<string, string>(Request, (int)ResultStatus.Success, "Student details updated successfully!");
                    }
                }
                catch(Exception ex)
                {
                    logger.Error("PUT | " + ex.Message);
                    return new CustomResponse<string, string>(Request, (int)ResultStatus.Failed, ex.Message, "");
                }
            }
            else
            {
                logger.Error("PUT | Updation of student data violating the modal state!");
                var errors = GetModelStateErrors(ModelState);
                return new CustomResponse<string, List<string>>(Request, (int)ResultStatus.Failed, errors, "");
            }
        }
        public IHttpActionResult Delete([FromUri] int id)
        {
            try
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var data = ctx.Students.FirstOrDefault(s => s.StudentID == id);
                    if (data == null)
                    {
                        logger.Error("DELETE | Can not delete student with id=" + id + " which does not exist.");
                        return new CustomResponse<string, string>(Request, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
                    }
                    ctx.Students.Remove(ctx.Students.Where(s => s.StudentID == id).Select(s => s).First());
                    ctx.SaveChanges();
                    logger.Info("DELETE | Deleted student record with id=" + id);
                    return new CustomResponse<string, string>(Request, (int)ResultStatus.Success, "Student with Id=" + id.ToString() + " has been removed successfully!");
                }
            }
            catch (Exception ex)
            {
                logger.Error("DELETE | " + ex.Message);
                return new CustomResponse<string, string>(Request, (int)ResultStatus.Failed, ex.Message);
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
    }

    public enum ResultStatus
    {
        Success = 1,
        Failed,
        NotFound
    }
}
