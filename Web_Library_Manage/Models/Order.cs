using System;
using System.Collections.Generic;

#nullable disable

namespace Web_Library_Manage.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Oid { get; set; }
        public int? Uid { get; set; }
        public DateTime? Odate { get; set; }

        public Order(int oid, int? uid, DateTime? odate)
        {
            Oid = oid;
            Uid = uid;
            Odate = odate;
        }

        public virtual User UidNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
