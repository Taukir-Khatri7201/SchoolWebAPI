using Microsoft.AspNetCore.Mvc;

namespace ConsumeSchoolAPI.Controllers
{
    public class BaseController : Controller
    {
        
    }

    public enum ResultStatus
    {
        Success = 1,
        Failed,
        NotFound
    }
}
