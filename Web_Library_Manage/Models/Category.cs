using System;
using System.Collections.Generic;

#nullable disable

namespace Web_Library_Manage.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Cid { get; set; }
        public string Cname { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
