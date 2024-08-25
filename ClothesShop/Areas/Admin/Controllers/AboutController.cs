using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class AboutController : Controller
    {
        // GET: Admin/About
        public ActionResult Index()
        {
            using (var ctx = new DBContext())
            {
                var aboutItem = ctx.abouts.FirstOrDefault();
                if(aboutItem == null)
                {
                    aboutItem = new about();
                }
                return View(aboutItem);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult doUpdate(about item, FormCollection form)
        {
            using (var ctx = new DBContext())
            {
                var content = form["content"];
                var item2 = ctx.abouts.FirstOrDefault(i => i.about_id == item.about_id);
                if (item2 == null)
                {
                    var newItem = new about
                    {
                        content = content,
                    };
                    ctx.abouts.Add(newItem);
                }
                else
                {
                    item2.content = content;
                }
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}