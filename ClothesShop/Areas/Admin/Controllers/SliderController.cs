using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class SliderController : Controller
    {
        // GET: Admin/Slider
        public ActionResult Index()
        {
            using (var ctx = new DBContext())
            {
                var list = ctx.sliders.ToList();
                return View(list);
            }
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            using (var ctx = new DBContext())
            {
                var slider = ctx.sliders.FirstOrDefault(item => item.slider_id == id);
                if (slider != null)
                    return View(slider);

                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult doAdd(HttpPostedFileBase img)
        {
            if (img.ContentLength > 0)
            {
                var slider = new slider();
                string _FileName = Path.GetFileName(img.FileName);
                string _path = Path.Combine(Server.MapPath("~/Content/images"), _FileName);
                img.SaveAs(_path);
                slider.slider_img = "~/Content/images/" + _FileName;
                using (var ctx = new DBContext())
                {
                    var lastSlider = ctx.sliders
                        .OrderByDescending(item => item.slider_id)
                        .FirstOrDefault();
                    slider.position = lastSlider == null ? 1 : lastSlider.position + 1;
                    ctx.sliders.Add(slider);
                    ctx.SaveChanges();
                    Session["success"] = "Thêm slider thành công";
                }
            }
            return View("Add");
        }


        [HttpPost]
        public ActionResult doUpdate(HttpPostedFileBase img, slider s)
        {
            if (img.ContentLength > 0)
            {
                using (var ctx = new DBContext())
                {
                    var slider = ctx.sliders.FirstOrDefault(item => item.slider_id == s.slider_id);
                    string _FileName = Path.GetFileName(img.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/images"), _FileName);
                    img.SaveAs(_path);
                    slider.slider_img = "~/Content/images/" + _FileName;
                    ctx.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View("Add");
        }

        public ActionResult Delete(int id)
        {
            using (var ctx = new DBContext())
            {
                var slider = ctx.sliders.FirstOrDefault(item => item.slider_id == id);
                if (slider != null)
                {
                    var list = ctx.sliders.Where(item => item.position > slider.position);
                    foreach (var item in list)
                    {
                        item.position -= 1;
                    }
                    ctx.sliders.Remove(slider);
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Down(int id)
        {
            updatePosition(id, "down");
            return RedirectToAction("Index");
        }
        public ActionResult Up(int id)
        {
            updatePosition(id, "up");
            return RedirectToAction("Index");
        }

        private void updatePosition(int id, string action)
        {
            using (var ctx = new DBContext())
            {
                var s = ctx.sliders.FirstOrDefault(item => item.slider_id == id);
                if (s != null)
                {
                    if ("up".Equals(action))
                    {
                        var previousSlider = ctx.sliders.FirstOrDefault(item => item.position == s.position - 1);
                        var img = s.slider_img;
                        s.slider_img = previousSlider.slider_img;
                        previousSlider.slider_img = img;
                    }
                    else
                    {
                        var nextSlider = ctx.sliders
                            .FirstOrDefault(item => item.position == s.position + 1);
                        var img = s.slider_img;
                        s.slider_img = nextSlider.slider_img;
                        nextSlider.slider_img = img;
                    }
                    ctx.SaveChanges();
                }
            }
        }
    }
}