using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class OrderAdminController : Controller
    {
        // GET: Admin/OrderAdmin
        public ActionResult Index()
        {
            using (var ctx = new DBContext())
            {
                var listOrder = ctx.orders.OrderByDescending(item => item.created_at).ToList();
                return View(listOrder);
            }
        }

        public ActionResult Confirm(int id)
        {
            using (var ctx = new DBContext())
            {
                var o = ctx.orders.FirstOrDefault(item => item.order_id == id);
                if (o != null)
                {
                    o.status = "in_shipping";
                    o.updated_at = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Cancel(int id)
        {
            using (var ctx = new DBContext())
            {
                var o = ctx.orders.FirstOrDefault(item => item.order_id == id);
                if (o != null)
                {
                    o.status = "cancel";
                    o.updated_at = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Done(int id)
        {
            using (var ctx = new DBContext())
            {
                var o = ctx.orders.FirstOrDefault(item => item.order_id == id);
                if (o != null)
                {
                    o.status = "done";
                    o.updated_at = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}