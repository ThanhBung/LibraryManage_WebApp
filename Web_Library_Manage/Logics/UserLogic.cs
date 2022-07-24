using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Library_Manage.Models;

namespace Web_Library_Manage.Logics
{
    public class UserLogic
    {
        LibraryContext context = new LibraryContext();

        public UserLogic()
        {
            context = new LibraryContext();
        }
        public User Login(string uname, string upassword)
        {
            return context.Users.FirstOrDefault(x => x.Uname.Equals(uname) && x.Upass.Equals(upassword));
        }

        public List<User> GetAllUser()
        {
            return context.Users.ToList();
        }

        public User GetUsersByUname(string uname)
        {
            return context.Users.FirstOrDefault(x => x.Uname.ToLower().Equals(uname.ToLower()));
        }

        public List<User> GetUsersByRoleCustomer()
        {
            return context.Users.Where(x => x.Urole.Equals("customer")).ToList();
        }

        public List<User> GetUsersByRoleLibrarian()
        {
            return context.Users.Where(x => x.Urole.Equals("librarian")).ToList();
        }
    }
}
