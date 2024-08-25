using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            ViewBag.CurrentController = "Register";
            return View();
        }

        [HttpPost]
        public ActionResult doRegister(user user, FormCollection form)
        {
            ViewBag.CurrentController = "Register";
            bool isPasswordValid = IsPasswordValid(user.password);
            if (!isPasswordValid)
            {
                Session["error"] = "Mật khẩu phải có ít nhất 8 ký tự, ít nhất 1 chữ số, 1 chữ hoa, 1 chữ thường và 1 ký tự đặc biệt.";
                return View("Index");
            }
            using (var ctx = new DBContext())
            {
                if (!user.password.Equals(form["confirm-password"]))
                {
                    Session["error"] = "Nhập lại mật khẩu không khớp";
                    return View("Index");
                }
                var isExistEmail = ctx.users.FirstOrDefault(item => item.email.Equals(user.email));
                if (isExistEmail != null)
                {
                    Session["error"] = "Email đã tồn tại";
                    return View("Index");
                }
                user.role = "user";
                user.created_at = DateTime.Now;
                user.updated_at = DateTime.Now;
                user.status = "active";
                ctx.users.Add(user);
                ctx.SaveChanges();
            }
            Session["success"] = "Đăng ký thành công";
            return RedirectToAction("Index", "Login");
        }
        private bool IsPasswordValid(string password)
        {
            // Kiểm tra mật khẩu theo yêu cầu của bạn
            Regex pattern = new Regex("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!#$%^&*()\\-_=+{};:,<.>/?]).{8,}");
            return pattern.IsMatch(password);
        }
    }
}