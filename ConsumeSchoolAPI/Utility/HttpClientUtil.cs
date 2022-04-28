using System.Net.Http;

namespace ConsumeSchoolAPI.Utility
{
    public static class HttpClientUtil
    {
        public readonly static HttpClient client = new HttpClient()
        {
            BaseAddress = new System.Uri("https://localhost:44361/")
        };
    }
}
