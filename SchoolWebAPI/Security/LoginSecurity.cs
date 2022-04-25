using SchoolWebAPI.Models;
using System;
using System.Linq;

namespace SchoolWebAPI.Security
{
    public class LoginSecurity
    {
        public SchoolDBEntities Context { get; }

        public static bool Login(string username, string password)
        {
            using(var context = new SchoolDBEntities())
            {
                bool result = false;
                foreach (var credential in context.Logins)
                {
                    result |= (credential.Username == username && credential.Password == password);
                    if (result) break;
                }
                //bool result = context.Logins.Any(s => s.Username == username && s.Password == password);
                return result;
            }
        }
    }
}