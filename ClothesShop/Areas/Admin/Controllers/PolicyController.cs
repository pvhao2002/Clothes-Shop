using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    public class PolicyController : Controller
    {
        // GET: Admin/Policy
        public ActionResult Index()
        {
            using (var ctx = new DBContext())
            {
                var item = ctx.policies.FirstOrDefault();
                if (item == null)
                {
                    item = new policy();
                }
                return View(item);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult doUpdate(policy item, FormCollection form)
        {
            using (var ctx = new DBContext())
            {
                var content = form["content"];
                var item2 = ctx.policies.FirstOrDefault(i => i.policy_id == item.policy_id);
                if (item2 == null)
                {
                    var newItem = new policy
                    {
                        policy_content = content,
                    };
                    ctx.policies.Add(newItem);
                }
                else
                {
                    item2.policy_content = content;
                }
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}