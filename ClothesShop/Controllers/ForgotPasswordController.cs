using ClothesShop.Models;
using ClothesShop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class ForgotPasswordController : Controller
    {
        // GET: ForgotPassword
        public ActionResult Index()
        {
            ViewBag.CurrentController = "Login";
            return View();
        }

        [HttpPost]
        public ActionResult doReset(user u)
        {
            using (var ctx = new DBContext())
            {
                var isExist = ctx.users.FirstOrDefault(item => item.email.Equals(u.email));
                if (isExist == null)
                {
                    Session["error"] = "Tài khoản chưa được đăng ký";
                }
                else
                {
                    var newPass = Guid.NewGuid().ToString().Substring(0, 5);
                    newPass = $"{newPass}A!1";
                    var isSendMailSuccess = MailUtils.sendEmail(u.email, 
                        "CÂP LẠI MẬT KHẨU CLOTHES SHOP",
                        $"Mật khẩu mới của bạn là: {newPass}");
                    if (isSendMailSuccess)
                    {
                        isExist.password = newPass;
                        isExist.updated_at = DateTime.Now;
                        ctx.SaveChanges();
                        Session["success"] = "Mật khẩu mới đã được gửi về mail của bạn.";
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}