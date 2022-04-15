using SchoolWebAPI.Repositories;
using SchoolWebAPI.Utility;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace SchoolWebAPI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ILogger, MyLogger>();
            container.RegisterInstance(typeof(ModalStateErrors));
            container.RegisterType<IStudentRepo, StudentRepo>();
            container.RegisterType<ITeacherRepo, TeacherRepo>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}