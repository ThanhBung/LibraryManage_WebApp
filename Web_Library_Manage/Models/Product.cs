using System;
using System.Collections.Generic;

#nullable disable

namespace Web_Library_Manage.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Pid { get; set; }
        public int Cid { get; set; }
        public string Pname { get; set; }
        public int? Pquantity { get; set; }
        public bool? Pstatus { get; set; }
        public decimal? Pcost { get; set; }
        public string Pdescription { get; set; }
        public string Pimg { get; set; }

        public Product(int pid, int cid, string pname, int? pquantity, bool? pstatus, decimal? pcost, string pdescription, string pimg)
        {
            Pid = pid;
            Cid = cid;
            Pname = pname;
            Pquantity = pquantity;
            Pstatus = pstatus;
            Pcost = pcost;
            Pdescription = pdescription;
            Pimg = pimg;
        }

        public virtual Category CidNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
