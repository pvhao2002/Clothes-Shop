using ClothesShop.Models;
using ClothesShop.Models.Lib;
using ClothesShop.Utils;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            ViewBag.CurrentController = "Cart";
            if (Session["user"] != null || !string.IsNullOrEmpty(Request.Cookies["userid"]?.Value))
            {
                using (var ctx = new DBContext())
                {
                    var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"].Value)
                    : (Session["user"] as user).user_id;
                    var cart = ctx.carts
                        .FirstOrDefault(item => item.user_id == user_id);
                    var cartTemp = new cart
                    {
                        cart_id = cart.cart_id,
                        total_price = cart.total_price,
                        total_quantity = cart.total_quantity,
                        cart_items = cart.cart_items
                        .ToList()
                        .Select(item => new cart_items
                        {
                            cart_item_id = item.cart_item_id,
                            product = item.product,
                            total_price = item.total_price,
                            quantity = item.quantity
                        })
                        .ToList()
                    };
                    return View(cartTemp);
                }
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult Add(FormCollection form)
        {
            if (Session["user"] == null && string.IsNullOrEmpty(Request.Cookies["userid"]?.Value))
                return RedirectToAction("Index", "Login");
            using (var ctx = new DBContext())
            {
                var pid = Convert.ToInt32(form["pid"] ?? "0");
                var price = Convert.ToDecimal(form["price"]);
                var quantity = Convert.ToInt32(form["quantity"] ?? "1");
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"].Value)
                    : (Session["user"] as user).user_id;
                var product = ctx.products.FirstOrDefault(item => item.product_id == pid);
                if (product.stock < quantity)
                {
                    Session["error"] = "Số lượng sản phẩm không đủ";
                    return RedirectToAction("Index");
                }
                var cartByUser = ctx.carts.FirstOrDefault(item => item.user_id == user_id);
                if (cartByUser != null)
                {
                    var cartItem = ctx.cart_items
                        .FirstOrDefault(item => item.cart_id == cartByUser.cart_id && item.product_id == pid);
                    if (cartItem != null)
                    {
                        cartItem.quantity += quantity;
                        cartItem.total_price = cartItem.quantity * price;
                        cartItem.updated_at = DateTime.Now;

                        if (product.stock < cartItem.quantity)
                        {
                            Session["error"] = "Số lượng sản phẩm không đủ";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        cartByUser.cart_items.Add(new cart_items
                        {
                            cart_id = cartByUser.cart_id,
                            product_id = pid,
                            quantity = quantity,
                            total_price = price * quantity,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        });
                    }
                    cartByUser.total_quantity += quantity;
                    cartByUser.updated_at = DateTime.Now;
                    ctx.SaveChanges();
                    cartByUser.total_price = ctx.cart_items.Where(item => item.cart_id == cartByUser.cart_id).Sum(item => item.total_price);
                    cartByUser.total_quantity = ctx.cart_items.Where(item => item.cart_id == cartByUser.cart_id).Sum(item => item.quantity);
                }
                else
                {
                    var newCart = new cart
                    {
                        user_id = user_id,
                        total_price = price * quantity,
                        total_quantity = quantity,
                        updated_at = DateTime.Now,
                        created_at = DateTime.Now,
                    };
                    newCart.cart_items.Add(new cart_items
                    {
                        cart = newCart,
                        product_id = pid,
                        quantity = quantity,
                        total_price = price * quantity,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    });
                    ctx.carts.Add(newCart);
                }
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult updateCart(int quantity, int pid)
        {
            using (var ctx = new DBContext())
            {
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"].Value)
                    : (Session["user"] as user).user_id;
                var cartByUser = ctx.carts.FirstOrDefault(item => item.user_id == user_id);
                if (cartByUser != null)
                {
                    var cartItem = ctx.cart_items.FirstOrDefault(item => item.cart_id == cartByUser.cart_id && item.product_id == pid);
                    if (cartItem != null)
                    {
                        cartItem.quantity = quantity;
                        cartItem.total_price = cartItem.quantity * cartItem.product.price;
                        cartItem.updated_at = DateTime.Now;
                    }
                    ctx.SaveChanges();
                    cartByUser.updated_at = DateTime.Now;
                    cartByUser.total_price = ctx.cart_items.Where(item => item.cart_id == cartByUser.cart_id).Sum(item => item.total_price);
                    cartByUser.total_quantity = ctx.cart_items.Where(item => item.cart_id == cartByUser.cart_id).Sum(item => item.quantity);
                    ctx.SaveChanges();
                }
            }
            return Json(new { });
        }

        public ActionResult Remove(int id)
        {
            using (var ctx = new DBContext())
            {
                var cartItem = ctx.cart_items.FirstOrDefault(r => r.cart_item_id == id);
                if (cartItem != null)
                {
                    ctx.cart_items.Remove(cartItem);
                    ctx.SaveChanges();
                }
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"].Value)
                    : (Session["user"] as user).user_id;
                var cartByUser = ctx.carts.FirstOrDefault(item => item.user_id == user_id);
                cartByUser.updated_at = DateTime.Now;
                cartByUser.total_price = ctx.cart_items.Where(item => item.cart_id == cartByUser.cart_id).Sum(item => item.total_price);
                cartByUser.total_quantity = ctx.cart_items.Where(item => item.cart_id == cartByUser.cart_id).Sum(item => item.quantity);
                ctx.SaveChanges();
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public void Checkout(FormCollection form)
        {
            using (var ctx = new DBContext())
            {
                var user_id = Session["user"] == null
                    ? Convert.ToInt32(Request.Cookies["userid"]?.Value)
                    : (Session["user"] as user).user_id;
                var cartByUser = ctx.carts.FirstOrDefault(item => item.user_id == user_id);
                var o = new order();
                o.user_id = user_id;
                o.shipping_address = form["address"].ToString();
                o.phone_number = form["phone"].ToString();
                o.total_price = cartByUser.total_price;
                o.total_quantity = cartByUser.total_quantity;
                o.created_at = DateTime.Now;
                o.updated_at = DateTime.Now;
                o.status = "pending";
                o.full_name = form["fullname"].ToString();
                var paymentMethod = form["payment"];
                cartByUser.cart_items.ForEach(item =>
                {
                    o.order_items.Add(new order_items
                    {
                        order = o,
                        product_id = item.product_id,
                        quantity = item.quantity,
                        total_price = item.total_price,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                    });
                    var pro = ctx.products.FirstOrDefault(itemP => itemP.product_id == item.product_id);
                    pro.sold += item.quantity;
                    pro.stock -= item.quantity;
                    ctx.SaveChanges();
                });
                var returnOrder = ctx.orders.Add(o);
                ctx.cart_items.RemoveRange(cartByUser.cart_items);
                cartByUser.total_price = 0;
                cartByUser.total_quantity = 0;
                cartByUser.cart_items = null;
                cartByUser.updated_at = DateTime.Now;
                ctx.SaveChanges();
                if ("VNPAY".Equals(paymentMethod))
                {
                    doPaymentVNPAY(returnOrder);
                }
                else
                {
                    var od = new order
                    {
                        order_id = returnOrder.order_id,
                        full_name = returnOrder.full_name,
                        phone_number = returnOrder.phone_number,
                        shipping_address = returnOrder.shipping_address,
                        created_at = returnOrder.created_at,
                        total_price = returnOrder.total_price,
                        order_items = returnOrder.order_items.ToList()
                    };
                    sendEmailOrder(od, "Thanh toán khi nhận hàng", cartByUser.user.email);
                    Response.Redirect("/Thank/Index");
                }
            }
        }

        private void sendEmailOrder(order o, string payment, string to)
        {
            var template = $@"<!DOCTYPE html>
                    <html>
                        <head>
                            <meta charset=""UTF-8"">
                            <style>
                                .order-table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                }}
                                .order-table, .order-table th, .order-table td {{
                                    border: 1px solid #ddd;
                                    padding: 8px;
                                }}
                                .order-table th {{
                                    padding-top: 12px;
                                    padding-bottom: 12px;
                                    text-align: left;
                                    background-color: #f2f2f2;
                                }}
                            </style>
                        </head>
                    <body>
                        <h2>Chi tiết đơn hàng</h2>
                        <p>Mã đơn hàng: {o.order_id}</p>
                        <p>Người nhận: {o.full_name}</p>
                        <p>Số điện thoại: {o.phone_number}</p>
                        <p>Địa chỉ giao hàng: {o.shipping_address}</p>
                        <p>Ngày mua: {o.created_at} </p>
                        <p>Tổng tiền: {o.total_price?.ToString("N0")} VND </p>
                        <p>Phương thức thanh toán:{payment} </p>
                        <p>Trạng thái đơn hàng: Đang chờ xác nhận</p>

                        <h3>Danh sách sản phẩm:</h3>
                        <table class=""order-table"">
                            <tr>
                                <th>Sản phẩm</th>
                                <th>Đơn giá</th>
                                <th>Số lượng</th>
                                <th>Thành tiền</th>
                            </tr>
                            {getORderItem(o.order_items.ToList())}
                    </table>
        </body>
        </html>";
            MailUtils.sendEmail(to, "ĐƠN HÀNG CLOTHES SHOP", template);
        }

        private string getORderItem(List<order_items> order_Items)
        {
            string template = "";
            foreach (var item in order_Items)
            {
                template += $@"
                    <tr>
                        <td>{item.product.product_name}</td>
                        <td>{item.product?.price?.ToString("N0")}</td>
                        <td>{item.quantity}</td>
                        <td>{item.total_price?.ToString("N0")} VND</td>
                    </tr>
                ";
            }
            return template;
        }


        private void doPaymentVNPAY(order o)
        {
            string vnp_Returnurl = "http://AmyShop.somee.com/Thank/Index";
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var gmtPlus7CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(o.created_at.Value.ToUniversalTime(), gmtPlus7);


            string vnp_TmnCode = "GLE8YXG4";
            string vnp_HashSecret = "ZCVPMHAELZKRPGTFLWJDPLQVPHBWEKXG";
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (Convert.ToInt64(o.total_price) * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", gmtPlus7CreatedAt.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Models.Lib.Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {o.order_id}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", $"{DateTime.Now.Ticks}{o.order_id}");
            var gmtPlus7ExpireDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMinutes(15), gmtPlus7);
            vnpay.AddRequestData("vnp_ExpireDate", gmtPlus7ExpireDate.ToString("yyyyMMddHHmmss"));
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            Response.Redirect(paymentUrl);
        }


    }
}