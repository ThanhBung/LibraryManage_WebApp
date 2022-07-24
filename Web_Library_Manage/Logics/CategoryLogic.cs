using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Library_Manage.Models;

namespace Web_Library_Manage.Logics
{
    public class CategoryLogic
    {
        LibraryContext context = new LibraryContext();

        public CategoryLogic()
        {
            context = new LibraryContext();
        }

        public List<Category> GetCategories()
        {
            return context.Categories.ToList();
        }
    }
}
