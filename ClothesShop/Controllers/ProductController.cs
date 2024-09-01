using ClothesShop.Models;
using ClothesShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace ClothesShop.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(int? cate, int? page)
        {
            ViewBag.CurrentController = "Product";
            int pageSize = 9; // Số mục trên mỗi trang
            int pageNumber = (page ?? 1);
            using (var ctx = new DBContext())
            {
                var listCate = ctx.categories.Where(item => item.status.Equals("active")).ToList();
                var query = ctx.products.Where(item => item.status.Equals("active"));
                if (cate != null)
                {
                    query = query.Where(item => item.category_id == cate);
                }
                query = query.OrderBy(item => item.product_name);

                // Step 3: Paginate in-memory data
                var products = query.ToPagedList(pageNumber, pageSize);

                var productModel = new ProductViewModel(products, listCate, cate);
                return View(productModel);
            }
        }
        public ActionResult doSearch(int? cate, int? page, string textSearch)
        {
            ViewBag.CurrentController = "Product";
            int pageSize = 9; // Số mục trên mỗi trang
            int pageNumber = (page ?? 1);
            using (var ctx = new DBContext())
            {
                var listCate = ctx.categories.Where(item => item.status.Equals("active")).ToList();
                var filteredProducts = cate != null
                ? ctx.products
                    .Where(item => item.category_id == cate && item.status.Equals("active"))
                    .AsEnumerable() // Move the data to memory
                    .Where(item => convertToUnSign3(item.product_name.ToLower())
                        .Contains(convertToUnSign3(textSearch.ToLower())))
                : ctx.products
                    .Where(item => "active".Equals(item.status, StringComparison.InvariantCultureIgnoreCase))
                    .AsEnumerable() // Move the data to memory
                    .Where(item => convertToUnSign3(item.product_name.ToLower())
                        .Contains(convertToUnSign3(textSearch.ToLower())));

                // Convert filtered results to PagedList
                var products = filteredProducts
                    .OrderBy(item => item.product_name)
                    .ToPagedList(pageNumber, pageSize);

                var productModel = new ProductViewModel(products, listCate, cate);
                return View("Index", productModel);
            }
        }

        public ActionResult Detail(int? id)
        {
            ViewBag.CurrentController = "Product";
            using (var ctx = new DBContext())
            {
                var p = ctx.products.FirstOrDefault(item => item.product_id == id);
                if (p == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var policyItem = ctx.policies.FirstOrDefault();
                var cmt = new comment
                {
                    product_id = id,
                };
                var listCmt = ctx.comments
                    .Where(item => item.product_id == id)
                    .ToList()
                    .Select(item => new comment
                    {
                        content = item.content,
                        comment_date = item.comment_date,
                        user = new user
                        {
                            email = item.user.email
                        }
                    })
                    .ToList();
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"]?.Value)
                    : (Session["user"] as user)?.user_id;
                var rate = ctx.ratings.FirstOrDefault(item => item.product_id == id && item.user_id == user_id);
                var numberRating = ctx.ratings.Where(item => item.product_id == id).ToList();
                var averageRating = numberRating.Average(item => item.rate);
                int fullStars = (int)Math.Floor((decimal)(averageRating ?? 0f));
                int halfStars = (averageRating - fullStars) >= 0.5 ? 1 : 0;
                int emptyStars = 5 - fullStars - halfStars;
                var model = new ProductViewModel(p, policyItem, cmt, listCmt, rate, numberRating.Count, fullStars, halfStars, emptyStars);
                return View(model);
            }
        }

        public string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}