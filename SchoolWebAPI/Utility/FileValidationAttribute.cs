using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolWebAPI.Utility
{
    public class FileValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Method to check if the file is valid or not
        /// </summary>
        /// <param name="value">The file object</param>
        /// <returns>Returns a bool value specifying if the file is valid or not</returns>
        public override bool IsValid(object value)
        {
            int MAX_LENGTH = 1000 * 1000 * 10;
            string[] AllowedExtensions = new string[] { "pdf", "txt" };
            var file = value as HttpPostedFile;
            bool flag = false;
            if (file == null)
            {
                ErrorMessage = "File is required!";
                return false;
            }
            flag = AllowedExtensions.Any(e => file.FileName.EndsWith(e));
            if(!flag)
            {
                ErrorMessage = "Please select file with text or pdf format";
            }
            else if(file.ContentLength > MAX_LENGTH)
            {
                flag = false;
                ErrorMessage = "The size of file is too large!";
            }
            return flag;
        }
    }
}