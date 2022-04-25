using SchoolWebAPI.Models;
using SchoolWebAPI.Repositories;
using SchoolWebAPI.Utility;
using System;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace SchoolWebAPI
{
    public static class UnityConfig
    {
        public static UnityContainer container { get; set; }
        public static void RegisterComponents()
        {
            container = new UnityContainer();
            container.RegisterType<ILogger, MyLogger>();
            var logger = container.Resolve<ILogger>();
            container.RegisterInstance(logger, InstanceLifetime.Singleton);
            container.RegisterInstance(typeof(ModalStateErrors));
            container.RegisterType<IStudentRepo, StudentRepo>();
            container.RegisterType<ITeacherRepo, TeacherRepo>();
            var context = new SchoolDBEntities();
            container.RegisterInstance(context, InstanceLifetime.Singleton);
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}