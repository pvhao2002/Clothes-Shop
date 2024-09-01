using ClothesShop.Models;
using ClothesShop.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class ProductAdminController : Controller
    {
        // GET: Admin/ProductAdmin
        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Số mục trên mỗi trang
            int pageNumber = (page ?? 1);
            using (var ctx = new DBContext())
            {
                var list = ctx.products
                    .Include(item => item.category)
                    .Where(item => item.status.Equals("active") && "active".Equals(item.category.status))
                    .OrderBy(item => item.product_id);

                var pagedResult = list
                    .ToPagedList(pageNumber, pageSize);
                return View(pagedResult);
            }
        }

        public ActionResult Add()
        {
            using (var ctx = new DBContext())
            {
                var listCate = ctx.categories.Where(item => item.status.Equals("active")).ToList();
                return View(new ProductViewModel(listCate, new product()));
            }
        }

        [HttpPost]
        public ActionResult doAdd(product product, HttpPostedFileBase img, FormCollection form)
        {
            using (var ctx = new DBContext())
            {
                var p = new product();
                p.price = product.price;
                p.product_name = product.product_name;
                p.description = product.description;
                p.status = "active";
                p.created_at = DateTime.Now;
                p.updated_at = DateTime.Now;
                p.stock = product.stock;
                p.sold = 0;
                if (img.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(img.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/images"), _FileName);
                    img.SaveAs(_path);
                    p.product_image = "~/Content/images/" + _FileName;
                }
                var cateId = form["cate"];
                p.category_id = Convert.ToInt32(cateId);
                ctx.products.Add(p);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index", "ProductAdmin");
        }


        public ActionResult Edit(int id)
        {
            using (var ctx = new DBContext())
            {
                var product = ctx.products.FirstOrDefault(item => item.product_id == id);
                if (product == null)
                {
                    return RedirectToAction("Index", "ProductAdmin");
                }
                var listCate = ctx.categories.Where(item => item.status.Equals("active")).ToList();
                var pModel = new ProductViewModel(listCate, product);
                return View(pModel);
            }
        }

        [HttpPost]
        public ActionResult doUpdate(product product, HttpPostedFileBase img, FormCollection form)
        {
            using (var ctx = new DBContext())
            {
                var p = ctx.products.FirstOrDefault(item => item.product_id == product.product_id);
                p.price = product.price;
                p.product_name = product.product_name;
                p.description = product.description;
                p.status = "active";
                p.updated_at = DateTime.Now;
                p.stock = product.stock;
                if (img.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(img.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/images"), _FileName);
                    img.SaveAs(_path);
                    p.product_image = "~/Content/images/" + _FileName;
                }
                var cateId = form["cate"];
                p.category_id = Convert.ToInt32(cateId);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index", "ProductAdmin");
        }


        public ActionResult delete(int id)
        {
            using (var ctx = new DBContext())
            {
                var p = ctx.products.FirstOrDefault(item => item.product_id == id);
                if (p != null)
                {
                    p.status = "inactive";
                    p.updated_at = DateTime.Now;
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("Index", "ProductAdmin");
        }
    }
}