using System;
using System.Collections.Generic;

#nullable disable

namespace Web_Library_Manage.Models
{
    public partial class OrderDetail
    {
        public int Oid { get; set; }
        public int Pid { get; set; }
        public int? Quantity { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool? Status { get; set; }

        public OrderDetail(int oid, int pid, int? quantity, DateTime? dateStart, DateTime? dateEnd, bool? status)
        {
            Oid = oid;
            Pid = pid;
            Quantity = quantity;
            DateStart = dateStart;
            DateEnd = dateEnd;
            Status = status;
        }

        public virtual Order OidNavigation { get; set; }
        public virtual Product PidNavigation { get; set; }
    }
}
