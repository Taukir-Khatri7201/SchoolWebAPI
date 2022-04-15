using SchoolWebAPI.Models;
using SchoolWebAPI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SchoolWebAPI.Repositories
{
    public class TeacherRepo : ITeacherRepo
    {
        private readonly ILogger logger;
        private readonly ModalStateErrors modalStateErrors;

        public TeacherRepo(ILogger logger, ModalStateErrors modalStateErrors)
        {
            this.logger = logger;
            this.modalStateErrors = modalStateErrors;
        }

        public IHttpActionResult Get(HttpRequestMessage request, int standardId = 0)
        {
            try
            {
                IList<TeacherViewModel> list = new List<TeacherViewModel>();
                using (var context = new SchoolDBEntities())
                {
                    switch (standardId)
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
                    if (standardId == 0)
                    {
                        logger.Info("GET | Requested for list of all teachers");
                    }
                    else
                    {
                        logger.Info("GET | Requested for list of all teachers from standard " + standardId);
                    }
                    if (list.Count == 0)
                    {
                        return new CustomResponse<string>(request, (int)ResultStatus.Success, "No teacher records found!");
                    }
                    return new CustomResponse<IList<TeacherViewModel>>(request, (int)ResultStatus.Success, "", list);
                }
            }
            catch (Exception ex)
            {
                logger.Error("GET | " + ex.Message);
                return new CustomResponse<string>(request, (int)ResultStatus.Failed, ex.Message);
            }
        }
        public IHttpActionResult Create(HttpRequestMessage request, ModelStateDictionary modelState, TeacherViewModel model)
        {
            if (modelState.IsValid)
            {
                try
                {
                    using (var ctx = new SchoolDBEntities())
                    {
                        if (model.StandardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.StandardId))
                        {
                            logger.Error("POST | Addition of the teacher data with StandardId=" + model.StandardId + " is violating the foreign key constraint!");
                            return new CustomResponse<string>(request, (int)ResultStatus.Failed, "StandardId=" + model.StandardId + " is violating the constraints!");
                        }
                        ctx.Teachers.Add(new Teacher
                        {
                            TeacherName = model.TeacherName,
                            StandardId = model.StandardId,
                            TeacherType = model.TeacherType,
                        });
                        ctx.SaveChanges();
                        logger.Info("POST | New teacher data added!");
                        return new CustomResponse<string>(request, (int)ResultStatus.Success, "Teacher details inserted successfully!");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Post | " + ex.Message);
                    return new CustomResponse<string>(request, (int)ResultStatus.Failed, ex.Message);
                }
            }
            else
            {
                logger.Error("POST | Addition of teacher data violating the modal state!");
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                return new CustomResponse<string>(request, (int)ResultStatus.Failed, errors, "");
            }
        }
        public IHttpActionResult UpdateTeacher(HttpRequestMessage request, ModelStateDictionary modelState, int id, TeacherViewModel model)
        {
            if (modelState.IsValid)
            {
                try
                {
                    using (var ctx = new SchoolDBEntities())
                    {
                        var oldData = ctx.Teachers.FirstOrDefault(s => s.TeacherId == id);
                        if (oldData == null)
                        {
                            logger.Info("PUT | Updation of teacher information with id=" + id + " not found!");
                            return new CustomResponse<string>(request, (int)ResultStatus.NotFound, "Teacher with Id=" + id.ToString() + " not found!");
                        }
                        if (model.StandardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.StandardId))
                        {
                            logger.Error("PUT | Updation of the teacher data with StandardId=" + model.StandardId + " is violating the foreign key constraint!");
                            return new CustomResponse<string>(request, (int)ResultStatus.Failed, "StandardId=" + model.StandardId + " is violating the constraints!");
                        }
                        oldData.TeacherName = model.TeacherName;
                        oldData.StandardId = model.StandardId;
                        oldData.TeacherType = model.TeacherType;
                        ctx.SaveChanges();
                        logger.Info("PUT | Teacher details updated successfully for id=" + id);
                        return new CustomResponse<string>(request, (int)ResultStatus.Success, "Teacher details updated successfully!");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("PUT | " + ex.Message);
                    return new CustomResponse<string>(request, (int)ResultStatus.Failed, ex.Message, "");
                }
            }
            else
            {
                logger.Error("PUT | Updation of teacher data violating the modal state!");
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                return new CustomResponse<string>(request, (int)ResultStatus.Failed, errors);
            }
        }
        public IHttpActionResult DeleteTeacher(HttpRequestMessage request, int id)
        {
            try
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var data = ctx.Teachers.FirstOrDefault(s => s.TeacherId == id);
                    if (data == null)
                    {
                        logger.Error("DELETE | Can not delete teacher with id=" + id + " which does not exist.");
                        return new CustomResponse<string>(request, (int)ResultStatus.NotFound, "Teacher with Id=" + id.ToString() + " not found!");
                    }
                    ctx.Teachers.Remove(data);
                    ctx.SaveChanges();
                    logger.Info("DELETE | Deleted teacher record with id=" + id);
                    return new CustomResponse<string>(request, (int)ResultStatus.Success, "Teacher with Id=" + id.ToString() + " has been removed successfully!");
                }
            }
            catch (Exception ex)
            {
                logger.Error("DELETE | " + ex.Message);
                return new CustomResponse<string>(request, (int)ResultStatus.Failed, ex.Message);
            }
        }
    }
}