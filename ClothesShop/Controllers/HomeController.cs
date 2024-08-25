using ClothesShop.Models;
using ClothesShop.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.CurrentController = "Home";
            using (var ctx = new DBContext())
            {
                var listSlider = ctx.sliders.OrderBy(item => item.position).ToList();
                var listProduct = ctx.products.OrderByDescending(item => item.sold).Take(12).ToList();

                var model = new HomeViewModel(listSlider, listProduct);
                return View(model);
            }
        }
        public ActionResult Logout()
        {
            Session["user"] = null;
            Response.Cookies["userid"].Value = null;
            Request.Cookies["userid"].Value = null;
            return RedirectToAction("Index");
        }

        public ActionResult WarrentyPolicy()
        {
            ViewBag.CurrentController = "About";
            return View();
        }

        public ActionResult ReturnPolicy()
        {
            ViewBag.CurrentController = "About";
            return View();
        }
        public ActionResult ShippingPolicy()
        {
            ViewBag.CurrentController = "About";
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            ViewBag.CurrentController = "About";
            return View();
        }
        public ActionResult FAQ()
        {
            ViewBag.CurrentController = "About";
            return View();
        }
    }
}