using DataAccess.ViewModels;
using DataAccess.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DataAccess.Models;

namespace SchoolWebAPI.Repositories
{
    public class StudentRepo : IStudentRepo
    {
        private readonly ILogger logger;
        private readonly ModalStateErrors modalStateErrors;
        private readonly SchoolDBEntities context;
        private readonly HttpRequest request;
        private readonly HttpRequestMessage requestMessage;

        public StudentRepo(ILogger logger, ModalStateErrors modalStateErrors, SchoolDBEntities context)
        {
            this.logger = logger;
            this.modalStateErrors = modalStateErrors;
            this.context = context;
            request = HttpContext.Current.Request;
            requestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
        }

        public IHttpActionResult GetAll()
        {
            IList<StudentViewModel> students = null;
            students = context.Students
                        .Select(s => new StudentViewModel()
                        {
                            Id = s.StudentID,
                            Name = s.StudentName,
                            standardId = s.StandardId,
                        }).ToList<StudentViewModel>();
            logger.Info("GET | Requested for the list of all students");
            if (students.Count == 0)
            {
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "No student Records found!");
            }

            return new CustomResponse<IList<StudentViewModel>>(requestMessage, (int)ResultStatus.Success, "", students);
        }

        public IHttpActionResult GetStudent(int id)
        {
            StudentViewModel student = null;

            student = context.Students
                        .Where(s => s.StudentID == id)
                        .Select(s => new StudentViewModel()
                        {
                            Id = s.StudentID,
                            Name = s.StudentName,
                            standardId = s.StandardId,
                        }).FirstOrDefault();
            if (student == null)
            {
                logger.Info("GET | Requested information of student with id=" + id + " not found!");
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
            }
            logger.Info("GET | Requested the information of student with id=" + id);
            return new CustomResponse<StudentViewModel>(requestMessage, (int)ResultStatus.Success, "", student);
        }

        public IHttpActionResult GetStudentsByStandard(int id)
        {
            List<StudentViewModel> student = null;
            student = context.Students
                        .Where(s => s.StandardId == id)
                        .Select(s => new StudentViewModel()
                        {
                            Id = s.StudentID,
                            Name = s.StudentName,
                            standardId = s.StandardId,
                        }).ToList();
            if (student == null)
            {
                logger.Info("GET | Requested information of student who are in standard " + id + " not found!");
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.NotFound, "No students are there in standard " + id);
            }
            logger.Info("GET | Requested the information of student who are in standard " + id);
            return new CustomResponse<List<StudentViewModel>>(requestMessage, (int)ResultStatus.Success, "", student);
        }

        public IHttpActionResult Create(ModelStateDictionary modelState, StudentViewModel model)
        {
            if (modelState.IsValid)
            {
                if (model.standardId != null && !context.Standards.AsEnumerable().Any(s => s.StandardId == model.standardId))
                {
                    logger.Error("POST | Addition of the student data with StandardId=" + model.standardId + " is violating the foreign key constraint!");
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "StandardId=" + model.standardId + " is violating the constraints!");
                }
                context.Students.Add(new Student
                {
                    StudentName = model.Name,
                    StandardId = model.standardId,
                });
                context.SaveChanges();
                logger.Info("POST | New student data added");
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Student details inserted successfully!");
            }
            else
            {
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                logger.Error("POST | Addition of the student data violating the modal state!");
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, errors);
            }
        }

        public IHttpActionResult UpdateStudent(ModelStateDictionary modelState, int id, StudentViewModel model)
        {
            if (modelState.IsValid)
            {
                try
                {
                    var oldData = context.Students.FirstOrDefault(s => s.StudentID == id);
                    if (oldData == null)
                    {
                        logger.Info("PUT | Updation of student information with id=" + id + " not found!");
                        return new CustomResponse<string>(requestMessage, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
                    }
                    if (model.standardId != null && !context.Standards.AsEnumerable().Any(s => s.StandardId == model.standardId))
                    {
                        logger.Error("PUT | Updation of the student data with StandardId=" + model.standardId + " is violating the foreign key constraint!");
                        return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "StandardId=" + model.standardId + " is violating the constraints!");
                    }
                    oldData.StudentName = model.Name;
                    oldData.StandardId = model.standardId;
                    context.SaveChanges();
                    logger.Info("PUT | Student details updated successfully for id=" + id);
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Student details updated successfully!");
                }
                catch (Exception ex)
                {
                    logger.Error("PUT | " + ex.Message);
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message, "");
                }
            }
            else
            {
                logger.Error("PUT | Updation of student data violating the modal state!");
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, errors);
            }
        }

        public IHttpActionResult DeleteStudent(int id)
        {
            try
            {
                var data = context.Students.FirstOrDefault(s => s.StudentID == id);
                if (data == null)
                {
                    logger.Error("DELETE | Can not delete student with id=" + id + " which does not exist.");
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.NotFound, "Student with Id=" + id.ToString() + " not found!");
                }
                context.Students.Remove(context.Students.Where(s => s.StudentID == id).Select(s => s).First());
                context.SaveChanges();
                logger.Info("DELETE | Deleted student record with id=" + id);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Student with Id=" + id.ToString() + " has been removed successfully!");
            }
            catch (Exception ex)
            {
                logger.Error("DELETE | " + ex.Message);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }

        /// <summary>
        /// Save Uploaded File Content Directly in Database
        /// </summary>
        /// <param name="id">StudentId</param>
        /// <returns></returns>
        //public IHttpActionResult UploadDocument(int id)
        //{
        //    try
        //    {
        //        if (request.Files.Count > 0)
        //        {
        //            var fileName = request.Files[0].FileName;
        //            var file = request.Files[0];
        //            var AllowedExtensions = new string[] { "application/pdf", "text/plain" };

        //            if (AllowedExtensions.Contains(file.ContentType))
        //            {
        //                var MAX_LENGTH = 1000 * 1000 * 10;
        //                if (file.ContentLength > MAX_LENGTH)
        //                {
        //                    logger.Info(string.Format("UPLOAD | Uploaded a file with size greater than 10MB"));
        //                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "The size of file must be less than 10MB");
        //                }
        //                var byteData = new byte[file.ContentLength];
        //                file.InputStream.Read(byteData, 0, file.ContentLength);
        //                if (!context.Students.AsEnumerable().Any(s => s.StudentID == id))
        //                {
        //                    logger.Info(string.Format("UPLOAD | No student exists with StudentId={0}", id));
        //                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, string.Format("No student exists with StudentId={0}!", id));
        //                }
        //                if (context.StudentDocuments.Any(s => s.StudentId == id))
        //                {
        //                    logger.Info(string.Format("UPLOAD | A try to overwrite student document for student with StudentId={0}", id));
        //                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, string.Format("Student document already exists for student with StudentId={0}!", id));
        //                }
        //                context.StudentDocuments.Add(new StudentDocument()
        //                {
        //                    StudentId = id,
        //                    FileName = fileName,
        //                    FileContent = byteData,
        //                });
        //                context.SaveChanges();
        //                logger.Info(string.Format("UPLOAD | Document uploaded for student with StudentId={0}", id));
        //                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Student Document Uploaded Successfully!");
        //            }
        //            logger.Info("UPLOAD | Wrong file uploaded");
        //            return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "Please upload text or pdf file only!");
        //        }
        //        else
        //        {

        //            logger.Error("UPLOAD | Wrong file uploaded");
        //            return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "Please upload a file!");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("UPLOAD | " + ex.Message);
        //        return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
        //    }
        //}


        /// <summary>
        /// Save Uploaded File in StudentDocumentsDirectory
        /// </summary>
        /// <param name="id">StudentId</param>
        /// <returns></returns>
        public IHttpActionResult UploadDocument(int id)
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
                            logger.Info(string.Format("UPLOAD | Uploaded a file with size greater than 10MB"));
                            return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "The size of file must be less than 10MB");
                        }
                        string path = HttpContext.Current.Server.MapPath("~/StudentDocuments/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string filePath = path + fileName;
                        file.SaveAs(filePath);

                        if (!context.Students.AsEnumerable().Any(s => s.StudentID == id))
                        {
                            logger.Info(string.Format("UPLOAD | No student exists with StudentId={0}", id));
                            return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, string.Format("No student exists with StudentId={0}!", id));
                        }
                        if (context.StudentDocuments2.Any(s => s.StudentId == id))
                        {
                            logger.Info(string.Format("UPLOAD | A try to overwrite student document for student with StudentId={0}", id));
                            return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, string.Format("Student document already exists for student with StudentId={0}!", id));
                        }
                        context.StudentDocuments2.Add(new StudentDocuments2()
                        {
                            StudentId = id,
                            FileName = fileName,
                            FileContent = filePath,
                        });
                        context.SaveChanges();

                        logger.Info(string.Format("UPLOAD | Document uploaded for student with StudentId={0}", id));
                        return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Student Document Uploaded Successfully!");
                    }
                    logger.Info("UPLOAD | Wrong file uploaded");
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "Please upload text or pdf file only!");
                }
                else
                {

                    logger.Error("UPLOAD | Wrong file uploaded");
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "Please upload a file!");
                }
            }
            catch (Exception ex)
            {
                logger.Error("UPLOAD | " + ex.Message);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }

        /// <summary>
        /// Download Document From FileContent stored in Database in form of byte array
        /// </summary>
        /// <param name="id">StudentId</param>
        /// <returns></returns>
        //public HttpResponseMessage DownloadDocument(int id)
        //{
        //    try
        //    {
        //        var available = context.StudentDocuments.Where(s => s.StudentId == id).FirstOrDefault();
        //        if (available == null)
        //        {
        //            string responseStr = string.Format("No document exists for student with StudentId={0}", id);
        //            logger.Info("DOWNLOAD | " + responseStr);
        //            var responseObj = new CustomDataWrapper<string>()
        //            {
        //                Data = null,
        //                StatusCode = (int)ResultStatus.Failed,
        //                messages = new List<string>() { responseStr + "!" },
        //            };
        //            return requestMessage.CreateResponse(HttpStatusCode.OK, responseObj);
        //        }
        //        var fileName = available.FileName;
        //        var fileContent = available.FileContent;
        //        var response = requestMessage.CreateResponse(HttpStatusCode.OK);
        //        response.Content = new ByteArrayContent(fileContent);
        //        response.Content.Headers.ContentLength = fileContent.Length;
        //        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //        response.Content.Headers.ContentDisposition.FileName = fileName;
        //        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
        //        logger.Info("DOWNLOAD | " + string.Format("Requested the document for student with StudentId={0}", id));
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("DOWNLOAD | " + ex.Message);
        //        var responseObj = new CustomDataWrapper<string>()
        //        {
        //            Data = null,
        //            StatusCode = (int)ResultStatus.Failed,
        //            messages = new List<string>() { ex.Message },
        //        };
        //        return requestMessage.CreateResponse(HttpStatusCode.OK, responseObj);
        //    }
        //}

        /// <summary>
        /// Download file using the file path stored in database table
        /// </summary>
        /// <param name="id">StudentId</param>
        /// <returns></returns>
        public HttpResponseMessage DownloadDocument(int id)
        {
            try
            {
                var available = context.StudentDocuments2.Where(s => s.StudentId == id).FirstOrDefault();
                if (available == null)
                {
                    string responseStr = string.Format("No document exists for student with StudentId={0}", id);
                    logger.Info("DOWNLOAD | " + responseStr);
                    var responseObj = new CustomDataWrapper<string>()
                    {
                        Data = null,
                        StatusCode = (int)ResultStatus.Failed,
                        messages = new List<string>() { responseStr + "!" },
                    };
                    return requestMessage.CreateResponse(HttpStatusCode.OK, responseObj);
                }
                var fileName = available.FileName;
                var filePath = available.FileContent;
                var fileContent = File.ReadAllBytes(filePath);
                var response = requestMessage.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(fileContent);
                response.Content.Headers.ContentLength = fileContent.Length;
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
                logger.Info("DOWNLOAD | " + string.Format("Requested the document for student with StudentId={0}", id));
                return response;
            }
            catch (Exception ex)
            {
                logger.Error("DOWNLOAD | " + ex.Message);
                var responseObj = new CustomDataWrapper<string>()
                {
                    Data = null,
                    StatusCode = (int)ResultStatus.Failed,
                    messages = new List<string>() { ex.Message },
                };
                return requestMessage.CreateResponse(HttpStatusCode.OK, responseObj);
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