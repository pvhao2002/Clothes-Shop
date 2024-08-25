using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Admin/Category
        public ActionResult Index()
        {
            using (var ctx = new DBContext())
            {
                var listCate = ctx.categories
                    .Where(item => "active".Equals(item.status.ToLower())).ToList();
                return View(listCate);
            }
        }

        public ActionResult Add()
        {
            return View(new category());
        }

        public ActionResult doAdd(category cate)
        {
            using (var ctx = new DBContext())
            {
                cate.status = "active";
                cate.created_at = DateTime.Now;
                cate.updated_at = DateTime.Now;
                ctx.categories.Add(cate);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            using (var ctx = new DBContext())
            {
                var cate = ctx.categories.FirstOrDefault(item => item.category_id == id);
                if(cate == null)
                {
                    return RedirectToAction("Index");
                }
                return View(cate);
            }
        }

        public ActionResult doUpdate(category cate)
        {
            using (var ctx = new DBContext())
            {
                var c = ctx.categories.FirstOrDefault(item => item.category_id == cate.category_id);
                if (c != null)
                {
                    c.description = cate.description;
                    c.category_name = cate.category_name;
                    c.updated_at = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            using (var ctx = new DBContext())
            {
                var cate = ctx.categories.FirstOrDefault(item => item.category_id == id);
                if (cate != null)
                {
                    cate.status = "inactive";
                    ctx.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }
    }
}