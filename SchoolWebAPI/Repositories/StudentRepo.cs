using SchoolWebAPI.Models;
using SchoolWebAPI.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;

namespace SchoolWebAPI.Repositories
{
    public class StudentRepo : IStudentRepo
    {
        private readonly ILogger logger;
        private readonly ModalStateErrors modalStateErrors;

        public StudentRepo(ILogger logger, ModalStateErrors modalStateErrors)
        {
            this.logger = logger;
            this.modalStateErrors = modalStateErrors;
        }

        public IHttpActionResult GetAll(HttpRequestMessage request)
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
                return new CustomResponse<string>(request, (int)ResultStatus.Success, "No student Records found!");
            }

            return new CustomResponse<IList<StudentViewModel>>(request, (int)ResultStatus.Success, "", students);
        }

        public IHttpActionResult GetStudent(HttpRequestMessage request, int id)
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
                return new CustomResponse<string>(request, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
            }
            logger.Info("GET | Requested the information of student with id=" + id);
            return new CustomResponse<StudentViewModel>(request, (int)ResultStatus.Success, "", student);
        }
        
        public IHttpActionResult GetStudentsByStandard(HttpRequestMessage request, int id)
        {
            List<StudentViewModel> student = null;

            using (var ctx = new SchoolDBEntities())
            {
                student = ctx.Students
                            .Where(s => s.StandardId == id)
                            .Select(s => new StudentViewModel()
                            {
                                Id = s.StudentID,
                                Name = s.StudentName,
                                standardId = s.StandardId,
                            }).ToList();
            }
            if (student == null)
            {
                logger.Info("GET | Requested information of student who are in standard " + id + " not found!");
                return new CustomResponse<string>(request, (int)ResultStatus.NotFound, "No students are there in standard " + id);
            }
            logger.Info("GET | Requested the information of student who are in standard " + id);
            return new CustomResponse<List<StudentViewModel>>(request, (int)ResultStatus.Success, "", student);
        }

        public IHttpActionResult Create(HttpRequestMessage request, System.Web.Http.ModelBinding.ModelStateDictionary modelState, StudentViewModel model)
        {
            if (modelState.IsValid)
            {
                using (var ctx = new SchoolDBEntities())
                {
                    if (model.standardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.standardId))
                    {
                        logger.Error("POST | Addition of the student data with StandardId=" + model.standardId + " is violating the foreign key constraint!");
                        return new CustomResponse<string>(request, (int)ResultStatus.Failed, "StandardId=" + model.standardId + " is violating the constraints!");
                    }
                    ctx.Students.Add(new Student
                    {
                        StudentName = model.Name,
                        StandardId = model.standardId,
                    });
                    ctx.SaveChanges();
                    logger.Info("POST | New student data added");
                    return new CustomResponse<string>(request, (int)ResultStatus.Success, "Student details inserted successfully!");
                }
            }
            else
            {
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                logger.Error("POST | Addition of the student data violating the modal state!");
                return new CustomResponse<string>(request, (int)ResultStatus.Failed, errors);
            }
        }

        public IHttpActionResult UpdateStudent(HttpRequestMessage request, System.Web.Http.ModelBinding.ModelStateDictionary modelState, int id, StudentViewModel model)
        {
            if (modelState.IsValid)
            {
                try
                {
                    using (var ctx = new SchoolDBEntities())
                    {
                        var oldData = ctx.Students.FirstOrDefault(s => s.StudentID == id);
                        if (oldData == null)
                        {
                            logger.Info("PUT | Updation of student information with id=" + id + " not found!");
                            return new CustomResponse<string>(request, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
                        }
                        if (model.standardId != null && !ctx.Standards.AsEnumerable().Any(s => s.StandardId == model.standardId))
                        {
                            logger.Error("PUT | Updation of the student data with StandardId=" + model.standardId + " is violating the foreign key constraint!");
                            return new CustomResponse<string>(request, (int)ResultStatus.Failed, "StandardId=" + model.standardId + " is violating the constraints!");
                        }
                        oldData.StudentName = model.Name;
                        oldData.StandardId = model.standardId;
                        ctx.SaveChanges();
                        logger.Info("PUT | Student details updated successfully for id=" + id);
                        return new CustomResponse<string>(request, (int)ResultStatus.Success, "Student details updated successfully!");
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
                logger.Error("PUT | Updation of student data violating the modal state!");
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                return new CustomResponse<string>(request, (int)ResultStatus.Failed, errors);
            }
        }

        public IHttpActionResult DeleteStudent(HttpRequestMessage request, int id)
        {
            try
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var data = ctx.Students.FirstOrDefault(s => s.StudentID == id);
                    if (data == null)
                    {
                        logger.Error("DELETE | Can not delete student with id=" + id + " which does not exist.");
                        return new CustomResponse<string>(request, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
                    }
                    ctx.Students.Remove(ctx.Students.Where(s => s.StudentID == id).Select(s => s).First());
                    ctx.SaveChanges();
                    logger.Info("DELETE | Deleted student record with id=" + id);
                    return new CustomResponse<string>(request, (int)ResultStatus.Success, "Student with Id=" + id.ToString() + " has been removed successfully!");
                }
            }
            catch (Exception ex)
            {
                logger.Error("DELETE | " + ex.Message);
                return new CustomResponse<string>(request, (int)ResultStatus.Failed, ex.Message);
            }
        }

        public IHttpActionResult UploadDocument(HttpRequest request, HttpRequestMessage requestMessage, System.Web.Http.ModelBinding.ModelStateDictionary modelState, int id)
        {
            try
            {
                if (request.Files.Count > 0)
                {
                    var fileName = request.Files[0].FileName;
                    var file = request.Files[0];
                    var AllowedExtensions = new string[] { "application/pdf", "text/plain" };

                    if (AllowedExtensions.Contains(file.ContentType))
                    {
                        var MAX_LENGTH = 1000 * 1000 * 10;
                        if (file.ContentLength > MAX_LENGTH)
                        {
                            return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "The size of file must be less than 10MB");
                        }
                        var byteData = new byte[file.ContentLength];
                        file.InputStream.Read(byteData, 0, file.ContentLength);
                        using (var ctx = new SchoolDBEntities())
                        {
                            if (!ctx.Students.AsEnumerable().Any(s => s.StudentID == id))
                            {
                                logger.Error(String.Format("POST | No student exists with StudentId={0}", id));
                                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, String.Format("No student exists with StudentId={0}!", id));
                            }

                            ctx.StudentDocuments.Add(new StudentDocument()
                            {
                                StudentId = id,
                                FileName = fileName,
                                FileContent = byteData,
                            });
                            ctx.SaveChanges();
                        }
                        return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Student Document Uploaded Successfully!");
                    }
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "Please upload text or pdf file only!");
                }
                else
                {
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "Please upload a file!");
                }
            }
            catch(Exception ex)
            {
                logger.Error("UPLOAD | " + ex.Message);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }
    
        public HttpResponseMessage DownloadDocument(HttpRequestMessage request, int id)
        {
            try
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var available = ctx.StudentDocuments.Where(s => s.StudentId == id).FirstOrDefault();
                    if (available == null)
                    {
                        string responseStr = String.Format("No document exists for student with StudentId={0}", id);
                        logger.Error("GET | " + responseStr);
                        var responseObj = new CustomDataWrapper<string>()
                        {
                            Data = null,
                            StatusCode = (int)ResultStatus.Failed,
                            messages = new List<string>() { responseStr + "!" },
                        };
                        return request.CreateResponse(HttpStatusCode.OK, responseObj);
                    }
                    var fileName = available.FileName;
                    var fileContent = available.FileContent;
                    var response = request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileContent);
                    response.Content.Headers.ContentLength = fileContent.Length;
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileName;
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
                    logger.Info("GET | " + String.Format("Requested the document for student with StudentId={0}", id));
                    return response;
                }
            }
            catch(Exception ex)
            {
                logger.Error("DOWNLOAD | " + ex.Message);
                var responseObj = new CustomDataWrapper<string>()
                {
                    Data = null,
                    StatusCode = (int)ResultStatus.Failed,
                    messages = new List<string>() { ex.Message },
                };
                return request.CreateResponse(HttpStatusCode.OK, responseObj);
            }
        }
    }

    public enum ResultStatus
    {
        Success = 1,
        Failed,
        NotFound
    }
}