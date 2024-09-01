using ClothesShop.Models;
using ClothesShop.ViewModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using System.Data.Entity;

namespace ClothesShop.Controllers
{
    public class LoginController : Controller
    {
        private static readonly string clientId = "512234703336-n9b6afgetl56eifs3jvgpda5oi390v65.apps.googleusercontent.com";
        private static readonly string clientSecret = "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";
        private static readonly string redirectUri = "http://AmyShop.somee.com/Login/GoogleLoginCallback";

        // GET: Login
        public ActionResult Index()
        {
            ViewBag.CurrentController = "Login";
            return View();
        }


        [HttpPost]
        public ActionResult doLogin(AccountViewModel user, FormCollection form)
        {
            ViewBag.CurrentController = "Login";
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            bool isPasswordValid = IsPasswordValid(user.Password);
            if (!isPasswordValid)
            {
                Session["error"] = "Mật khẩu phải có ít nhất 8 ký tự, ít nhất 1 chữ số, 1 chữ hoa, 1 chữ thường và 1 ký tự đặc biệt.";
                return View("Index");
            }

            using (var ctx = new DBContext())
            {
                var u = ctx.users.FirstOrDefault(item => item.email.Equals(user.Username));

                if (u != null)
                {
                    if ("block".Equals(u.status))
                    {
                        Session["error"] = "Tài khoản của bạn đã bị khóa";
                        return View("Index");
                    }
                    else if (u.password.Equals(user.Password))
                    {
                        Session["user"] = u;
                        Response.Cookies["userid"].Value = u.user_id.ToString();
                        // time expired after 30 days
                        Response.Cookies["userid"].Expires = DateTime.Now.AddDays(30);
                        if (u.role.Equals("admin"))
                        {
                            return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            Session["error"] = "Email hoặc mật khẩu không đúng";
            return View("Index");
        }
        private bool IsPasswordValid(string password)
        {
            // Kiểm tra mật khẩu theo yêu cầu của bạn
            Regex pattern = new Regex("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@!#$%^&*()\\-_=+{};:,<.>/?]).{8,}");
            return pattern.IsMatch(password);
        }

        [AllowAnonymous]
        public ActionResult LoginWithGoogle()
        {
            var authorizationUrl = GetAuthorizationUrl();
            return Redirect(authorizationUrl);
        }

        private string GetAuthorizationUrl()
        {
            var request = new AuthorizationCodeRequestUrl(
                new Uri("https://accounts.google.com/o/oauth2/auth"))
            {
                ClientId = clientId,
                Scope = "openid email profile",
                RedirectUri = redirectUri,
                ResponseType = "code"
            };
            return request.Build().ToString();
        }

        private async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
        {
            var tokenRequest = new AuthorizationCodeTokenRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = code,
                RedirectUri = redirectUri
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            });

            return await flow.ExchangeCodeForTokenAsync("user", code, redirectUri, CancellationToken.None);
        }

        private async Task<Userinfo> GetUserInfoAsync(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);
            var oauthService = new Oauth2Service(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            return await oauthService.Userinfo.Get().ExecuteAsync();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GoogleLoginCallback()
        {
            var code = Request["code"];
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Login");
            }

            var tokenResponse = await ExchangeCodeForTokenAsync(code);
            var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);
            var u = saveNewUser(userInfo);
            Session["user"] = u;
            Response.Cookies["userid"].Value = u.user_id.ToString();
            // time expired after 30 days
            Response.Cookies["userid"].Expires = DateTime.Now.AddDays(30);
            return RedirectToAction("Index", "Home");
        }


        private user saveNewUser(Userinfo userInfo)
        {
            using (var ctx = new DBContext())
            {
                var user = ctx.users.FirstOrDefault(item => item.email.Equals(userInfo.Email));
                if (user == null)
                {
                    var newPass = Guid.NewGuid().ToString().Substring(0, 5);
                    newPass = $"{newPass}A!1";
                    var newU = new user
                    {
                        email = userInfo.Email,
                        password = newPass,
                        role = "user",
                        full_name = userInfo.Name,
                        status = "active",
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                    };
                    var rUser = ctx.users.Add(newU);
                    ctx.SaveChanges();
                    return rUser;
                }
                else
                {
                    user.full_name = userInfo.Name;
                    user.updated_at = DateTime.Now;
                    ctx.SaveChanges();
                    return user;
                }
            }
        }

    }
}