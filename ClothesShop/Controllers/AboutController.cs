using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class AboutController : Controller
    {
        // GET: About
        public ActionResult Index()
        {
            ViewBag.CurrentController = "About";
            using (var ctx = new DBContext())
            {
                var item = ctx.abouts.FirstOrDefault();
                if (item == null)
                {
                    item = new about();
                }
                return View(item);
            }
        }
    }
}