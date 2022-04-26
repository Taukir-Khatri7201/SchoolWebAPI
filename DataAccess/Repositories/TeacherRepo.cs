using DataAccess.ViewModels;
using DataAccess.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class TeacherRepo : ITeacherRepo
    {
        private readonly ILogger logger;
        private readonly ModalStateErrors modalStateErrors;
        private readonly SchoolDBEntities context;
        private readonly HttpRequest request;
        private readonly HttpRequestMessage requestMessage;

        public TeacherRepo(ILogger logger, ModalStateErrors modalStateErrors, SchoolDBEntities context)
        {
            this.logger = logger;
            this.modalStateErrors = modalStateErrors;
            this.context = context;
            request = HttpContext.Current.Request;
            requestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
        }

        public IHttpActionResult Get(int standardId = 0)
        {
            try
            {
                IList<TeacherViewModel> list = new List<TeacherViewModel>();
                #region [1] Way to get all teachers list without using Stored Procedure
                //switch (standardId)
                //{
                //    case 0:
                //        list = context.spGetTeachers(standardId).ToList<Teacher>().Select(s => new TeacherViewModel()
                //        {
                //            TeacherId = s.TeacherId,
                //            StandardId = s.StandardId,
                //            TeacherName = s.TeacherName,
                //            TeacherType = s.TeacherType,
                //        }).ToList();

                //        //list = context.Teachers.Select(t => new TeacherViewModel()
                //        //{
                //        //    TeacherId = t.TeacherId,
                //        //    TeacherName = t.TeacherName,
                //        //    TeacherType = (int)t.TeacherType,
                //        //    StandardId = (int)t.StandardId,
                //        //}).ToList();
                //        break;
                //    default:
                //        list = context.spGetTeachers(standardId).ToList<Teacher>().Select(s => new TeacherViewModel()
                //        {
                //            TeacherId = s.TeacherId,
                //            StandardId = s.StandardId,
                //            TeacherName = s.TeacherName,
                //            TeacherType = s.TeacherType,
                //        }).ToList();
                //        break;

                //}
                #endregion

                #region [2] Way to get all teachers list using Stored Procedure
                list = context.spGetTeachers(standardId).Select(s => new TeacherViewModel()
                {
                    TeacherId = s.TeacherId,
                    StandardId = s.StandardId,
                    TeacherName = s.TeacherName,
                    TeacherType = s.TeacherType,
                }).ToList();
                #endregion

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
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "No teacher records found!");
                }
                return new CustomResponse<IList<TeacherViewModel>>(requestMessage, (int)ResultStatus.Success, "", list);
            }
            catch (Exception ex)
            {
                logger.Error("GET | " + ex.Message);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }

        /// <summary>
        /// Get Techers By Name using Entity Framework
        /// </summary>
        //public IHttpActionResult GetTeachersByName(string name)
        //{
        //    try
        //    {
        //        var list = new TeachersWithCountViewModel();
        //        list.ListOfTeachers = new List<TeacherViewModel>();
        //        var parameter1 = new SqlParameter
        //        {
        //            ParameterName = "Name",
        //            Value = name,
        //        };
        //        var parameter2 = new SqlParameter
        //        {
        //            ParameterName = "Count",
        //            Value = 0,
        //        };
        //        var output = new ObjectParameter("Count", 0);
        //        list.ListOfTeachers = context.spGetTeacherByNameWithCount(name, output).Select(s => new TeacherViewModel()
        //        {
        //            TeacherId = s.TeacherId,
        //            StandardId = s.StandardId,
        //            TeacherName = s.TeacherName,
        //            TeacherType = s.TeacherType,
        //        }).ToList();
        //        list.Count = (int)output.Value;

        //        logger.Info(string.Format("GET | Requested for list of all teachers with name '{0}'", name));
        //        if (list.Count == 0)
        //        {
        //            return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "No teacher records found!");
        //        }
        //        return new CustomResponse<TeachersWithCountViewModel>(requestMessage, (int)ResultStatus.Success, "", list);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("GET | " + ex.Message);
        //        return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
        //    }
        //}

        /// <summary>
        /// Get Teachers By Name using SqlCommand
        /// </summary>
        public IHttpActionResult GetTeachersByName(string name)
        {
            try
            {
                var list = new TeachersWithCountViewModel();
                list.ListOfTeachers = new List<TeacherViewModel>();
                SqlConnection conn = new SqlConnection();
                SqlCommand cmd = new SqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SchoolDBConnection"].ConnectionString;
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetTeacherByNameWithCount";
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.Add("@Count", SqlDbType.Int);
                cmd.Parameters["@Count"].Direction = ParameterDirection.Output;
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = new TeacherViewModel()
                            {
                                TeacherId = (int)reader[1],
                                TeacherName = (string)reader[2],
                                StandardId = (int)reader[3],
                                TeacherType = (int)reader[4],
                            };
                            list.ListOfTeachers.Add(model);
                        }
                    }
                    int count = Convert.ToInt32(cmd.Parameters["@Count"].Value);
                    list.Count = count;
                    return new CustomResponse<TeachersWithCountViewModel>(requestMessage, (int)ResultStatus.Success, "", list);
                }
                catch (Exception ex)
                {
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error("GET | " + ex.Message);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }
        public IHttpActionResult GetCountOfTeachersByStandardId(int id)
        {
            var count = new ObjectParameter("Count", typeof(int));
            int? totalCount;
            totalCount = context.spCountTeachersByStandardId(id, count).FirstOrDefault();

            logger.Info(string.Format("GET | Requested for list of all teachers with StandardId={0}", id));
            if (totalCount == null)
            {
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "No teacher records found!");
            }
            return new CustomResponse<int?>(requestMessage, (int)ResultStatus.Success, "", totalCount);
        }
        public IHttpActionResult GetCountByStandard()
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SchoolDBConnection"].ConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from vWGetNumberOfTeachersByStandard";
                cmd.Connection = conn;
                var list = new List<vWGetNumberOfTeachersByStandard>();
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = new vWGetNumberOfTeachersByStandard()
                            {
                                ID = (long)reader[0],
                                StandardId = (int)reader[1],
                                Total = (int)reader[2],
                            };
                            list.Add(model);
                        }
                    }
                    return new CustomResponse<List<vWGetNumberOfTeachersByStandard>>(requestMessage, (int)ResultStatus.Success, "", list);
                }
                catch (Exception ex)
                {
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }
        public IHttpActionResult GetStudentMarks()
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SchoolDBConnection"].ConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from vwStudentMarks";
                cmd.Connection = conn;
                var list = new List<vwStudentMark>();
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = new vwStudentMark()
                            {
                                StudentId = (int)reader[0],
                                StudentName = (string)reader[1],
                                CourseName = (string)reader[2],
                                MarksObtained = (int)reader[3],
                            };
                            list.Add(model);
                        }
                    }
                    return new CustomResponse<List<vwStudentMark>>(requestMessage, (int)ResultStatus.Success, "", list);
                }
                catch (Exception ex)
                {
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }
        public IHttpActionResult GetStudentWithMaxMarksInEachCourse()
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SchoolDBConnection"].ConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from vwStudentWithMaxMarksInEachCourse";
                cmd.Connection = conn;
                var list = new List<vwStudentWithMaxMarksInEachCourse>();
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = new vwStudentWithMaxMarksInEachCourse()
                            {
                                StudentID = (int)reader[0],
                                StudentName = (string)reader[1],
                                CourseName = (string)reader[2],
                                MarksObtained = (int)reader[3],
                            };
                            list.Add(model);
                        }
                    }
                    return new CustomResponse<List<vwStudentWithMaxMarksInEachCourse>>(requestMessage, (int)ResultStatus.Success, "", list);
                }
                catch (Exception ex)
                {
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }
        public IHttpActionResult Create(ModelStateDictionary modelState, TeacherViewModel model)
        {
            if (modelState.IsValid)
            {
                try
                {
                    if (model.StandardId != null && !context.Standards.AsEnumerable().Any(s => s.StandardId == model.StandardId))
                    {
                        logger.Error("POST | Addition of the teacher data with StandardId=" + model.StandardId + " is violating the foreign key constraint!");
                        return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "StandardId=" + model.StandardId + " is violating the constraints!");
                    }
                    context.Teachers.Add(new Teacher
                    {
                        TeacherName = model.TeacherName,
                        StandardId = model.StandardId,
                        TeacherType = model.TeacherType,
                    });
                    context.SaveChanges();
                    logger.Info("POST | New teacher data added!");
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Teacher details inserted successfully!");
                }
                catch (Exception ex)
                {
                    logger.Error("Post | " + ex.Message);
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
                }
            }
            else
            {
                logger.Error("POST | Addition of teacher data violating the modal state!");
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, errors, "");
            }
        }
        public IHttpActionResult UpdateTeacher(ModelStateDictionary modelState, int id, TeacherViewModel model)
        {
            if (modelState.IsValid)
            {
                try
                {
                    var oldData = context.Teachers.FirstOrDefault(s => s.TeacherId == id);
                    if (oldData == null)
                    {
                        logger.Info("PUT | Updation of teacher information with id=" + id + " not found!");
                        return new CustomResponse<string>(requestMessage, (int)ResultStatus.NotFound, "Teacher with Id=" + id.ToString() + " not found!");
                    }
                    if (model.StandardId != null && !context.Standards.AsEnumerable().Any(s => s.StandardId == model.StandardId))
                    {
                        logger.Error("PUT | Updation of the teacher data with StandardId=" + model.StandardId + " is violating the foreign key constraint!");
                        return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, "StandardId=" + model.StandardId + " is violating the constraints!");
                    }
                    oldData.TeacherName = model.TeacherName;
                    oldData.StandardId = model.StandardId;
                    oldData.TeacherType = model.TeacherType;
                    context.SaveChanges();
                    logger.Info("PUT | Teacher details updated successfully for id=" + id);
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Teacher details updated successfully!");
                }
                catch (Exception ex)
                {
                    logger.Error("PUT | " + ex.Message);
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message, "");
                }
            }
            else
            {
                logger.Error("PUT | Updation of teacher data violating the modal state!");
                var errors = modalStateErrors.GetModelStateErrors(modelState);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, errors);
            }
        }
        public IHttpActionResult DeleteTeacher(int id)
        {
            try
            {
                var data = context.Teachers.FirstOrDefault(s => s.TeacherId == id);
                if (data == null)
                {
                    logger.Error("DELETE | Can not delete teacher with id=" + id + " which does not exist.");
                    return new CustomResponse<string>(requestMessage, (int)ResultStatus.NotFound, "Teacher with Id=" + id.ToString() + " not found!");
                }
                context.Teachers.Remove(data);
                context.SaveChanges();
                logger.Info("DELETE | Deleted teacher record with id=" + id);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Success, "Teacher with Id=" + id.ToString() + " has been removed successfully!");
            }
            catch (Exception ex)
            {
                logger.Error("DELETE | " + ex.Message);
                return new CustomResponse<string>(requestMessage, (int)ResultStatus.Failed, ex.Message);
            }
        }

    }
}