using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Library_Manage.Models;

namespace Web_Library_Manage.Logics
{
    public class OrderLogic
    {
        LibraryContext context = new LibraryContext();

        public OrderLogic()
        {
            context = new LibraryContext();
        }

        public List<Order> GetOrders()
        {
            return context.Orders.ToList();
        }

        public List<OrderDetail> GetOrderDetailsByOid(int oid)
        {
            return context.OrderDetails.Where(x => x.Oid == oid).ToList();
        }

        public OrderDetail GetOrderDetailsByOidAndPid(int oid, int pid)
        {
            return context.OrderDetails.FirstOrDefault(x => x.Oid == oid && x.Pid == pid);
        }
    }
}
