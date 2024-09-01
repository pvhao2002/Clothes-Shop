using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        [HttpPost]
        public void doComment(comment cmt)
        {
            using (var ctx = new DBContext())
            {
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"].Value)
                    : (Session["user"] as user)?.user_id;
                if (user_id != null)
                {
                    cmt.user_id = user_id;
                    cmt.comment_date = DateTime.Now;
                    ctx.comments.Add(cmt);
                    ctx.SaveChanges();
                }
            }
            Session["tab-comment"] = "OK";
            Response.Redirect($"/Product/Detail/{cmt.product_id}");
        }
    }
}