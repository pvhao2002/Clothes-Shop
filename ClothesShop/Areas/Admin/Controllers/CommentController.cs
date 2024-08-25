using ClothesShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        // GET: Admin/Comment
        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Số mục trên mỗi trang
            int pageNumber = (page ?? 1);
            using (var ctx = new DBContext())
            {
                var list = ctx.comments
                    .Include(item => item.user)
                    .Include(item => item.product)
                    .OrderByDescending(item => item.comment_date);

                var pagedResult = list
                    .ToPagedList(pageNumber, pageSize);
                return View(pagedResult);
            }
        }

        public ActionResult Delete(int id)
        {
            using (var ctx = new DBContext())
            {
                var cmt = ctx.comments.FirstOrDefault(item => item.comment_id == id);
                ctx.comments.Remove(cmt);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}