using ConsumeSchoolAPI.Models;
using ConsumeSchoolAPI.Utility;
using DataAccess.Utility;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace ConsumeSchoolAPI.Controllers
{
    public class StudentController : BaseController
    {
        private readonly HttpClient client;
        public JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = false
        };
        public StudentController()
        {
            client = HttpClientUtil.client;
        }
        public IActionResult Index()
        {
            var model = new CombinedViewModel();
            var response = client.GetAsync("api/student");
            response.Wait();
            var result = response.Result;
            if(result.IsSuccessStatusCode)
            {
                model.studentData = result.Content.ReadAsAsync<StudentResponseViewModel>().Result;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult AddStudentData(StudentViewModel model)
        {
            client.DefaultRequestHeaders.Add("Authorization", "taukir$khatri");
            var response = client.PostAsJsonAsync($"api/student/create", model);
            response.Wait();
            var result = response.Result;
            CustomDataWrapper<string> responseData = new CustomDataWrapper<string>();
            if (result.IsSuccessStatusCode)
            {
                responseData = result.Content.ReadAsAsync<CustomDataWrapper<string>>().Result;
            }
            client.DefaultRequestHeaders.Remove("Authorization");
            return Json(responseData, serializerOptions);
        }

        [HttpPut]
        public IActionResult UpdateStudentData(StudentViewModel model)
        {
            client.DefaultRequestHeaders.Add("Authorization", "taukir$khatri");
            var response = client.PutAsJsonAsync($"api/student/{model.Id}/update", model);
            response.Wait();
            var result = response.Result;
            CustomDataWrapper<string> responseData = new CustomDataWrapper<string>();
            if(result.IsSuccessStatusCode)
            {
                responseData = result.Content.ReadAsAsync<CustomDataWrapper<string>>().Result;
            }
            client.DefaultRequestHeaders.Remove("Authorization");
            return Json(responseData, serializerOptions);
        }

        [HttpDelete]
        public IActionResult RemoveStudentData(int id)
        {
            client.DefaultRequestHeaders.Add("Authorization", "taukir$khatri");
            var response = client.DeleteAsync($"api/student/{id}/remove");
            response.Wait();
            var result = response.Result;
            CustomDataWrapper<string> responseData = new CustomDataWrapper<string>();
            if (result.IsSuccessStatusCode)
            {
                responseData = result.Content.ReadAsAsync<CustomDataWrapper<string>>().Result;
            }
            client.DefaultRequestHeaders.Remove("Authorization");
            return Json(responseData, serializerOptions);
        }

        public IActionResult DownloadDocument()
        {
            return View();
        }
    }
}
