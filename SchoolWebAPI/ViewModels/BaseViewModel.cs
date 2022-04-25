using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolWebAPI.ViewModels
{
    public class BaseViewModel
    {
        public int Id { get; set; }
        public int TotalCount { get; set; }
    }
}