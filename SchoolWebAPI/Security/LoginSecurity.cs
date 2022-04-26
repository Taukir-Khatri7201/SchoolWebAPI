using DataAccess.Models;

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
                    result |= (string.Compare(credential.Username, username, System.StringComparison.CurrentCultureIgnoreCase)==0 && credential.Password == password);
                    if (result) break;
                }
                return result;
            }
        }
    }
}