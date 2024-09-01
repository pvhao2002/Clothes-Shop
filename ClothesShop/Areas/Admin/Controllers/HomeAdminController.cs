using ClothesShop.Models;
using ClothesShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        // GET: Admin/HomeAdmin
        public ActionResult Index()
        {
            using (var ctx = new DBContext())
            {
                var totalP = ctx.products.Where(item => item.status.Equals("active")).ToList().Count;
                var totalO = ctx.orders.ToList().Count;
                var totalR = ctx.orders.Sum(item => item.total_price);
                var homeModel = new HomeAdminViewModel(totalP, totalO, totalR);
                return View(homeModel);
            }
        }

        public ActionResult search(DateTime fromDate, DateTime toDate)
        {
            fromDate = fromDate.Date;
            toDate = toDate.AddDays(1).AddSeconds(-1);
            using (var ctx = new DBContext())
            {
                var totalP = ctx.products.Where(item => item.status.Equals("active")).ToList().Count;
                var totalO = ctx.orders
                    .Where(item => item.created_at >= fromDate && item.created_at <= toDate)
                    .ToList().Count;
                var totalR = ctx.orders
                    .Where(item => item.created_at >= fromDate && item.created_at <= toDate)
                    .Sum(item => item.total_price);
                var homeModel = new HomeAdminViewModel(totalP, totalO, totalR);
                return View("Index", homeModel);
            }
        }

        public ActionResult Logout()
        {
            Session["user"] = null;
            Response.Cookies["userid"].Value = null;
            Request.Cookies["userid"].Value = null;
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}