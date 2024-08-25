using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class RateController : Controller
    {
        [HttpGet]
        public void doRate(int p, int r)
        {
            using (var ctx = new DBContext())
            {
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"]?.Value)
                    : (Session["user"] as user)?.user_id;
                var rate = new rating
                {
                    product_id = p,
                    rate = r,
                    user_id = user_id
                };
                ctx.ratings.Add(rate);
                ctx.SaveChanges();
            }
            Response.Redirect($"/Product/Detail/{p}");
        }
    }
}