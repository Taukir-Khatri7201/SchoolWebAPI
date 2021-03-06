//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SchoolDBEntities : DbContext
    {
        public SchoolDBEntities()
            : base("name=SchoolDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Standard> Standards { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentAddress> StudentAddresses { get; set; }
        public virtual DbSet<StudentDocument> StudentDocuments { get; set; }
        public virtual DbSet<StudentDocuments2> StudentDocuments2 { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<StudentMark> StudentMarks { get; set; }
        public virtual DbSet<View_StudentCourse> View_StudentCourse { get; set; }
        public virtual DbSet<vWGetNumberOfTeachersByStandard> vWGetNumberOfTeachersByStandards { get; set; }
        public virtual DbSet<vwStudentMark> vwStudentMarks { get; set; }
        public virtual DbSet<vwStudentWithMaxMarksInEachCourse> vwStudentWithMaxMarksInEachCourses { get; set; }
    
        public virtual ObjectResult<GetCoursesByStudentId_Result> GetCoursesByStudentId(Nullable<int> studentId)
        {
            var studentIdParameter = studentId.HasValue ?
                new ObjectParameter("StudentId", studentId) :
                new ObjectParameter("StudentId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCoursesByStudentId_Result>("GetCoursesByStudentId", studentIdParameter);
        }
    
        public virtual int sp_DeleteStudent(Nullable<int> studentId)
        {
            var studentIdParameter = studentId.HasValue ?
                new ObjectParameter("StudentId", studentId) :
                new ObjectParameter("StudentId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_DeleteStudent", studentIdParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> sp_InsertStudentInfo(Nullable<int> standardId, string studentName)
        {
            var standardIdParameter = standardId.HasValue ?
                new ObjectParameter("StandardId", standardId) :
                new ObjectParameter("StandardId", typeof(int));
    
            var studentNameParameter = studentName != null ?
                new ObjectParameter("StudentName", studentName) :
                new ObjectParameter("StudentName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("sp_InsertStudentInfo", standardIdParameter, studentNameParameter);
        }
    
        public virtual int sp_UpdateStudent(Nullable<int> studentId, Nullable<int> standardId, string studentName)
        {
            var studentIdParameter = studentId.HasValue ?
                new ObjectParameter("StudentId", studentId) :
                new ObjectParameter("StudentId", typeof(int));
    
            var standardIdParameter = standardId.HasValue ?
                new ObjectParameter("StandardId", standardId) :
                new ObjectParameter("StandardId", typeof(int));
    
            var studentNameParameter = studentName != null ?
                new ObjectParameter("StudentName", studentName) :
                new ObjectParameter("StudentName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_UpdateStudent", studentIdParameter, standardIdParameter, studentNameParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> spCountTeachersByStandardId(Nullable<int> id, ObjectParameter count)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spCountTeachersByStandardId", idParameter, count);
        }
    
        public virtual ObjectResult<spGetTeacherByNameWithCount_Result> spGetTeacherByNameWithCount(string name, ObjectParameter count)
        {
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetTeacherByNameWithCount_Result>("spGetTeacherByNameWithCount", nameParameter, count);
        }
    
        public virtual ObjectResult<spGetTeachers_Result> spGetTeachers(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetTeachers_Result>("spGetTeachers", idParameter);
        }
    }
}
