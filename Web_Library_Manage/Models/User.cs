using System;
using System.Collections.Generic;

#nullable disable

namespace Web_Library_Manage.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int Uid { get; set; }
        public string Uname { get; set; }
        public string Upass { get; set; }
        public string Uphone { get; set; }
        public string Uaddress { get; set; }
        public string Urole { get; set; }

        public User(int uid, string uname, string upass, string uphone, string uaddress, string urole)
        {
            Uid = uid;
            Uname = uname;
            Upass = upass;
            Uphone = uphone;
            Uaddress = uaddress;
            Urole = urole;
        }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
