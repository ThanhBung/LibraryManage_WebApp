using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Library_Manage.Logics;
using Web_Library_Manage.Models;

namespace Web_Library_Manage.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Login(string uname, string upassword)
        {
            if (uname == null || upassword == null )
            {
                return View("/Views/Home/Login.cshtml");
            }
            if (uname.Trim().Equals(String.Empty) || upassword.Trim().Equals(String.Empty))
            {
                ViewBag.error = "Wrong formnat!";
                return View("/Views/Home/Login.cshtml");
            }
            UserLogic ul = new UserLogic();
            User u = ul.Login(uname, upassword);
            if(u == null)
            {
                ViewBag.error = "This account does not exist!";
                return View("/Views/Home/Login.cshtml");
            }
            else
            {
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(u));
                return Home();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return View("/Views/Home/Login.cshtml");
        }

        public IActionResult Register(string uname, string upassword, string urepassword)
        {
            if (uname == null || upassword == null || urepassword == null)
            {
                return View("/Views/Home/Register.cshtml");
            }
            if (uname.Trim().Equals(String.Empty) || upassword.Trim().Equals(String.Empty) || urepassword.Trim().Equals(String.Empty))
            {
                ViewBag.error = "Wrong formnat!";
                return View("/Views/Home/Register.cshtml");
            }
            UserLogic ul = new UserLogic();
            List<User> listUser = ul.GetAllUser();
            int ok = 1;
            for (int i = 0; i < listUser.Count; i++)
            {
                if (uname.Trim().ToLower().Equals(listUser[i].Uname.ToLower()))
                {
                    ok = 0;
                    break;
                }
            }
            if (!upassword.Equals(urepassword))
            {
                ViewBag.error = "Wrong password!";
                return View("/Views/Home/Register.cshtml");
            }
            if(ok == 0)
            {
                ViewBag.error = "This account has existed!";
                return View("/Views/Home/Register.cshtml");
            }
            else
            {
                LibraryContext context = new LibraryContext();
                context.Users.Add(new User(listUser.Count+1, uname, upassword,null,null, "customer"));
                context.SaveChanges();
                ViewBag.successful = "Registered successfully";
                View("/Views/Home/Register.cshtml");
            }
            return View("/Views/Home/Register.cshtml");
        }

        public IActionResult Home()
        {
            if(HttpContext.Session.GetString("user") == null)
            {
                return View("/Views/Home/Login.cshtml");
            }
            else
            {
                User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
                ViewBag.userLogin = u;
            }
            return View("/Views/Home/Home.cshtml");
        }

        public IActionResult ListBooks(int cid = 0, int bstatus = 2, int bpage = 1)
        {
            BookLogic bl = new BookLogic();
            CategoryLogic cl = new CategoryLogic();

            List<Product> listBook = bl.GetProducts();
            List<Product> listBookByCid = bl.GetProductsByCatIdAndBstatus(cid, bstatus);
            List<Category> listCategory = cl.GetCategories();

            int count = listBookByCid.Count;
            int endPage = count / 4;
            if (count % 4 != 0) endPage++;
            ViewBag.EndPage = endPage;

            int numberOfObjectsPerPage = 4;
            ViewBag.listCategory = listCategory;
            ViewBag.listBookByCid = listBookByCid.Skip(numberOfObjectsPerPage * (bpage - 1)).Take(numberOfObjectsPerPage).ToList();
            ViewBag.CurrentPage = bpage;

            ViewBag.CurrentCid = cid;
            ViewBag.CurrentBstatus = bstatus;

            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;

            if (Request.Cookies["cart"] != null)
            {
                Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
                ViewBag.sizeCart = cart.Count;
            }
            return View("/Views/Home/ListBooks.cshtml");
        }

        public IActionResult AddBook(int cid, int bstatus, int bpage, string cname, string bname, int bquantity, int bcost, string bdes, string bimg)
        {
            LibraryContext context = new LibraryContext();
            BookLogic bl = new BookLogic();
            CategoryLogic cl = new CategoryLogic();

            int cid_new = context.Categories.FirstOrDefault(x => x.Cname.Equals(cname)).Cid;
            int bookLastId = context.Products.OrderBy(x => x.Pid).LastOrDefault().Pid;
            Product book_raw = context.Products.FirstOrDefault(x => x.Pname.ToLower().Equals(bname.ToLower()));
            if(book_raw != null)
            {
                ViewBag.add0 = "Add Failed! This book has existed!";
            }
            else
            {
                context.Products.Add(new Product(bookLastId+1, cid_new, bname, bquantity, false, bcost, bdes, bimg));
                context.SaveChanges();
                ViewBag.add1 = "Add Successfully!";
            }

            //List Books
            List<Product> listBook = bl.GetProducts();
            List<Product> listBookByCid = bl.GetProductsByCatIdAndBstatus(cid, bstatus);
            List<Category> listCategory = cl.GetCategories();

            int count = listBookByCid.Count;
            int endPage = count / 4;
            if (count % 4 != 0) endPage++;
            ViewBag.EndPage = endPage;

            int numberOfObjectsPerPage = 4;
            ViewBag.listCategory = listCategory;
            ViewBag.listBookByCid = listBookByCid.Skip(numberOfObjectsPerPage * (bpage - 1)).Take(numberOfObjectsPerPage).ToList();
            ViewBag.CurrentPage = bpage;

            ViewBag.CurrentCid = cid;
            ViewBag.CurrentBstatus = bstatus;

            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;

            if (Request.Cookies["cart"] != null)
            {
                Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
                ViewBag.sizeCart = cart.Count;
            }
            return View("/Views/Home/ListBooks.cshtml");
        }

        public IActionResult EditBook(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0)
        {
            LibraryContext context = new LibraryContext();
            CategoryLogic cl = new CategoryLogic();
            List<Category> listCategory = cl.GetCategories();
            Product book = context.Products.FirstOrDefault(x => x.Pid == pid);
            ViewBag.listCategory = listCategory;
            ViewBag.book = book;

            //User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            //ViewBag.userLogin = u;

            return View("/Views/Home/EditBook.cshtml");
        }

        public IActionResult SaveChangeBook(int bid, string cname, string bname, int bquantity, int bcost,
            string bdes, string bimg, string status)
        {
            LibraryContext context = new LibraryContext();
            Product p = context.Products.FirstOrDefault(x => x.Pid == bid);
            int cid_new = context.Categories.FirstOrDefault(x => x.Cname.Equals(cname)).Cid;
            p.Pname = bname;
            p.Cid = cid_new;
            p.Pquantity = bquantity;
            p.Pcost = bcost;
            p.Pdescription = bdes;
            p.Pimg = p.Pimg;
            p.Pstatus = status == "Published" ? true : false;
            context.SaveChanges();
            ViewBag.edit1 = "Save Changed Successfully!";

            CategoryLogic cl = new CategoryLogic();
            List<Category> listCategory = cl.GetCategories();
            Product book = context.Products.FirstOrDefault(x => x.Pid == bid);
            ViewBag.listCategory = listCategory;
            ViewBag.book = book;
            //User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            //ViewBag.userLogin = u;

            return View("/Views/Home/EditBook.cshtml");
        }

        public IActionResult DeleteBook(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0)
        {
            LibraryContext context = new LibraryContext();
            BookLogic bl = new BookLogic();
            CategoryLogic cl = new CategoryLogic();

            OrderDetail od_raw = context.OrderDetails.FirstOrDefault(x => x.Pid == pid);
            if(od_raw == null)
            {
                context.Remove(context.Products.FirstOrDefault(x => x.Pid == pid));
                context.SaveChanges();
                ViewBag.delete1 = "Delete Successfully";
            }
            else
            {
                ViewBag.delete0 = "Delete Failed! The Order Details has this Product!";
            }

            //List Books
            List<Product> listBook = bl.GetProducts();
            List<Product> listBookByCid = bl.GetProductsByCatIdAndBstatus(cid, bstatus);
            List<Category> listCategory = cl.GetCategories();

            int count = listBookByCid.Count;
            int endPage = count / 4;
            if (count % 4 != 0) endPage++;
            ViewBag.EndPage = endPage;

            int numberOfObjectsPerPage = 4;
            ViewBag.listCategory = listCategory;
            ViewBag.listBookByCid = listBookByCid.Skip(numberOfObjectsPerPage * (bpage - 1)).Take(numberOfObjectsPerPage).ToList();
            ViewBag.CurrentPage = bpage;

            ViewBag.CurrentCid = cid;
            ViewBag.CurrentBstatus = bstatus;

            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;

            if (Request.Cookies["cart"] != null)
            {
                Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
                ViewBag.sizeCart = cart.Count;
            }
            return View("/Views/Home/ListBooks.cshtml");
        }

        public IActionResult AddCustomer(string cusname, string cuspassword, string cusphone, string cusaddress)
        {
            LibraryContext context = new LibraryContext();
            UserLogic ul = new UserLogic();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            User u_check = context.Users.FirstOrDefault(x => x.Uname.ToLower().Equals(cusname.ToLower()));
            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            if (u_check != null)
            {
                ViewBag.error = "This customer already exists!";
                ViewBag.listCustomers = listCustomers;
                ViewBag.userLogin = u;
                return View("/Views/Home/ListCustomers.cshtml");
            }

            //u_check == null
            int uid_last = context.Users.OrderBy(x => x.Uid).LastOrDefault().Uid;
            User u_new = new User(uid_last + 1, cusname, cuspassword, cusphone, cusaddress, "customer");
            context.Users.Add(u_new);
            context.SaveChanges();

            listCustomers = ul.GetUsersByRoleCustomer();
            ViewBag.successful = "Add Customer Successfully!";
            ViewBag.listCustomers = listCustomers;
            ViewBag.userLogin = u;

            return View("/Views/Home/ListCustomers.cshtml");
        }

        public IActionResult ListLibrarians()
        {
            UserLogic ul = new UserLogic();
            List<User> listLibrarians = ul.GetUsersByRoleLibrarian();
            ViewBag.listLibrarians = listLibrarians;
            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;
            return View("/Views/Home/ListLibrarians.cshtml");
        }

        public IActionResult ListCustomers()
        {
            UserLogic ul = new UserLogic();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            ViewBag.listCustomers = listCustomers;
            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;
            return View("/Views/Home/ListCustomers.cshtml");
        }

        public IActionResult AddItem(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0)
        {
            Dictionary<int, int> cart;
            if (Request.Cookies["cart"] != null)
            {
                cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
                int check = 0;
                foreach (int i in cart.Keys)
                {
                    if (pid == i)
                    {
                        check = 1;
                        cart[i] += 1;
                        break;
                    }
                }
                if (check == 0) cart.Add(pid, 1);
            }
            else
            {
                cart = new Dictionary<int, int>();
                cart.Add(pid, 1);
            }
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cart), cookieOptions);

            BookLogic bl = new BookLogic();
            CategoryLogic cl = new CategoryLogic();

            List<Product> listBook = bl.GetProducts();
            List<Product> listBookByCid = bl.GetProductsByCatIdAndBstatus(cid, bstatus);
            List<Category> listCategory = cl.GetCategories();

            int count = listBookByCid.Count;
            int endPage = count / 5;
            if (count % 5 != 0) endPage++;
            ViewBag.EndPage = endPage;

            int numberOfObjectsPerPage = 5;
            ViewBag.listCategory = listCategory;
            ViewBag.listBookByCid = listBookByCid.Skip(numberOfObjectsPerPage * (bpage - 1)).Take(numberOfObjectsPerPage).ToList();
            ViewBag.CurrentPage = bpage;

            ViewBag.CurrentCid = cid;
            ViewBag.CurrentBstatus = bstatus;

            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;

            ViewBag.sizeCart = cart.Count;

            return View("/Views/Home/ListBooks.cshtml");
        }

        public IActionResult ViewCart()
        {
            BookLogic bl = new BookLogic();
            int size;
            List<Cart> carts = new List<Cart>();
            if (Request.Cookies["cart"] != null)
            {
                Dictionary<int, int> cok = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
                foreach (int key in cok.Keys)
                {
                    Product p = bl.GetProductsByPid(key);
                    Cart cart = new Cart(p, cok[key]);
                    carts.Add(cart);
                }
                size = carts.Count;
                ViewBag.Cart = carts.ToList();
                if (size < 1) ViewBag.NonPage = 1;
                else ViewBag.NonPage = 0;
            }

            UserLogic ul = new UserLogic();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            ViewBag.listCustomers = listCustomers;

            //User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            //ViewBag.userLogin = u;

            return View("/Views/Home/ViewCart.cshtml");
        }

        public IActionResult DoDelete(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0)
        {
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
            cart.Remove(pid);
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cart), cookieOptions);
            return RedirectToAction("ViewCart");
        }

        public IActionResult Process(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0, int process = 2)
        {
            BookLogic bl = new BookLogic();
            List<Cart> carts = new List<Cart>();
            Dictionary<int, int> cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
            Product p_raw = bl.GetProductsByPid(pid);
            foreach (int key in cart.Keys)
            {
                if (key == p_raw.Pid)
                {
                    if (process == 0)
                    {
                        if (cart[key] == 1) cart.Remove(key);
                        else cart[key] -= 1;
                    }
                    if (process == 1)
                    {
                        cart[key] += 1;
                    }
                }
            }
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(3) };
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cart), cookieOptions);
            return RedirectToAction("ViewCart");
        }

        public IActionResult ListOrders()
        {
            OrderLogic ol = new OrderLogic();
            UserLogic ul = new UserLogic();
            List<Order> listOrders = ol.GetOrders();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            ViewBag.listOrders = listOrders;
            ViewBag.listCustomers = listCustomers;
            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;
            return View("/Views/Home/ListOrders.cshtml");
        }

        public IActionResult OrderDetails(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0, int process = 2, int oid = 0)
        {
            OrderLogic ol = new OrderLogic();
            UserLogic ul = new UserLogic();
            BookLogic bl = new BookLogic();

            List<Order> listOrders = ol.GetOrders();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            List<OrderDetail> listOrderDetails = ol.GetOrderDetailsByOid(oid);
            List<Product> listProducts = bl.GetProducts();

            ViewBag.listOrderDetails = listOrderDetails;
            ViewBag.listOrders = listOrders;
            ViewBag.listCustomers = listCustomers;
            ViewBag.listProducts = listProducts;
            ViewBag.currentOid = oid;

            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;
            return View("/Views/Home/OrderDetails.cshtml");
        }       

        public IActionResult DeleteOrder(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0, int process = 2, int oid = 0)
        {
            LibraryContext context = new LibraryContext();
            OrderLogic ol = new OrderLogic();
            UserLogic ul = new UserLogic();

            List<OrderDetail> listOrderDetails = ol.GetOrderDetailsByOid(oid);
            if(listOrderDetails.Count == 0)
            {
                Order o = context.Orders.FirstOrDefault(x => x.Oid == oid);
                context.Orders.Remove(o);
                context.SaveChanges();
                ViewBag.delete1 = "Delete Successfully!";
            }
            else
            {
                ViewBag.delete0 = "Delete Failed! This Customer have the Order Details for this Order!";
            }

            //List Orders
            List<Order> listOrders = ol.GetOrders();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            ViewBag.listOrders = listOrders;
            ViewBag.listCustomers = listCustomers;
            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;
            return View("/Views/Home/ListOrders.cshtml");
        }

        public IActionResult UpdateStatus(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0, int process = 2, int oid = 0, int odpid = 0)
        {
            LibraryContext context = new LibraryContext();
            OrderLogic ol = new OrderLogic();
            UserLogic ul = new UserLogic();
            BookLogic bl = new BookLogic();

            OrderDetail od_old = context.OrderDetails.FirstOrDefault(x => x.Oid == oid && x.Pid == odpid);
            Product p = context.Products.FirstOrDefault(x => x.Pid == odpid);

            if (od_old.Status == false)
            {
                od_old.Status = true;
                context.SaveChanges();
                p.Pquantity += od_old.Quantity;
                context.SaveChanges();
            }
            else
            {
                od_old.Status = false;
                context.SaveChanges();
                p.Pquantity -= od_old.Quantity;
                context.SaveChanges();
            }
            context.SaveChanges();

            List<Order> listOrders = ol.GetOrders();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            List<OrderDetail> listOrderDetails = ol.GetOrderDetailsByOid(oid);
            List<Product> listProducts = bl.GetProducts();

            ViewBag.update = "Update Successfully!";
            ViewBag.listOrderDetails = listOrderDetails;
            ViewBag.listOrders = listOrders;
            ViewBag.listCustomers = listCustomers;
            ViewBag.listProducts = listProducts;
            ViewBag.currentOid = oid;

            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;
            return View("/Views/Home/OrderDetails.cshtml");
        }

        public IActionResult DeleteOrderDetails(int cid = 0, int bstatus = 2, int bpage = 1, int pid = 0, int process = 2, int oid = 0, int odpid = 0)
        {
            LibraryContext context = new LibraryContext();
            OrderLogic ol = new OrderLogic();
            UserLogic ul = new UserLogic();
            BookLogic bl = new BookLogic();

            OrderDetail od = context.OrderDetails.FirstOrDefault(x => x.Oid == oid && x.Pid == odpid);
            if(od.Status == true)
            {
                context.OrderDetails.Remove(od);
                context.SaveChanges();
                ViewBag.delete1 = "Delete Successfully!";
            } else
            {
                ViewBag.delete0 = "Delete Failed! This Customer has not returned the Book!";
            }

            //Order Details
            List<Order> listOrders = ol.GetOrders();
            List<User> listCustomers = ul.GetUsersByRoleCustomer();
            List<OrderDetail> listOrderDetails = ol.GetOrderDetailsByOid(oid);
            List<Product> listProducts = bl.GetProducts();

            ViewBag.listOrderDetails = listOrderDetails;
            ViewBag.listOrders = listOrders;
            ViewBag.listCustomers = listCustomers;
            ViewBag.listProducts = listProducts;
            ViewBag.currentOid = oid;

            User u = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.userLogin = u;
            return View("/Views/Home/OrderDetails.cshtml");
        }

        public IActionResult AddToOrder(string duration, string cusname, string cusphone, string cusaddress)
        {
            LibraryContext context = new LibraryContext();
            BookLogic bl = new BookLogic();
            Cart cart = new Cart();
            OrderLogic ol = new OrderLogic();
            List<Order> listOrders = ol.GetOrders();
            UserLogic ul = new UserLogic();

            User cus = context.Users.FirstOrDefault(x => x.Uname.ToLower().Equals(cusname.ToLower()));
            cus.Uphone = cusphone;
            cus.Uaddress = cusaddress;
            context.SaveChanges();
            DateTime now = DateTime.Now;
            Order o = new Order(listOrders.Count + 1, cus.Uid, now);
            Dictionary<int, int> cok = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Cookies["cart"]);
            context.Orders.Add(o);
            context.SaveChanges();

            int ordID = context.Orders.OrderBy(x => x.Oid).LastOrDefault().Oid;

            foreach (int key in cok.Keys)
            {
                Product p = bl.GetProductsByPid(key);
                //oid - pid - quantity - dateStart - dateEnd
                string[] arr = duration.Split(' ');
                double durationAdd = Convert.ToDouble(arr[0]);
                DateTime dateEnd = now.AddDays(durationAdd);
                context.OrderDetails.Add(new OrderDetail(ordID, p.Pid, cok[key], now, dateEnd, false));
                context.SaveChanges();
                Product pnew = context.Products.FirstOrDefault(x => x.Pid == p.Pid);
                pnew.Pquantity -= cok[key];
                context.SaveChanges();
                cok.Remove(key);
            }
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cok), cookieOptions);
            return RedirectToAction("ViewCart");
        }
    }
}
