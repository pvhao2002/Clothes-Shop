using ClothesShop.Models;
using ClothesShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            ViewBag.CurrentController = "Profile";
            if (Session["user"] == null && string.IsNullOrEmpty(Request.Cookies["userid"]?.Value))
                return RedirectToAction("Index", "Login");
            using (var ctx = new DBContext())
            {
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"].Value)
                    : (Session["user"] as user).user_id;
                var listOrder = ctx.orders
                    .Where(item => item.user_id == user_id)
                    .ToList()
                    .Select(item => new order
                    {
                        order_id = item.order_id,
                        created_at = item.created_at,
                        total_price = item.total_price,
                        total_quantity = item.total_quantity,
                        full_name = item.full_name,
                        shipping_address = item.shipping_address,
                        phone_number = item.phone_number,
                        status = item.status,
                        order_items = item.order_items
                        .Select(oitem => new order_items
                        {
                            product_id = oitem.product_id,
                            product = oitem.product,
                            total_price = oitem.total_price,
                            quantity = oitem.quantity,
                        })
                        .ToList()
                    })
                    .ToList();
                var user = ctx.users.FirstOrDefault(item => item.user_id == user_id);
                var profileModel = new ProfileViewModel(listOrder, user);
                return View(profileModel);
            }
        }
    }
}