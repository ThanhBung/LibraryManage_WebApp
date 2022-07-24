using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Library_Manage.Models;

namespace Web_Library_Manage.Logics
{
    public class BookLogic
    {
        LibraryContext context = new LibraryContext();
        public BookLogic()
        {
            context = new LibraryContext();
        }

        public List<Product> GetProducts()
        {
            return context.Products.ToList();
        }

        public Product GetProductsByPid(int pid)
        {
            return context.Products.FirstOrDefault(x => x.Pid == pid);
        }

        public List<Product> GetProductsByCatId(int cid)
        {
            if (cid == 0) return context.Products.ToList();
            return context.Products.Where(x => x.Cid == cid).ToList();
        }

        public List<Product> GetProductsByCatIdAndBstatus(int cid, int bstatus)
        {
            if (cid == 0)
            {
                if (bstatus == 2) return context.Products.ToList();
                else if (bstatus == 0) return context.Products.Where(x => x.Pstatus == false).ToList();
                else return context.Products.Where(x => x.Pstatus == true).ToList();
            }
            else
            {
                if (bstatus == 2) return context.Products.Where(x => x.Cid == cid).ToList();
                else if (bstatus == 0) return context.Products.Where(x => x.Pstatus == false && x.Cid == cid).ToList();
                else return context.Products.Where(x => x.Pstatus == true && x.Cid == cid).ToList();
            }
        }


    }
}
