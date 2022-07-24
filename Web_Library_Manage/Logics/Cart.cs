using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Library_Manage.Models
{
    public class Cart
    {
        Product p;
        int quantity;
        DateTime duration;

        public Cart()
        {
        }

        public Cart(Product p, int quantity)
        {
            this.p = p;
            this.quantity = quantity;
        }

        public Product Product
        {
            get { return p; }
            set { p = value; }
        }

        public int Quantities
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public DateTime Duration
        {
            get { return duration; }
            set { duration = value; }
        }

    }
}
