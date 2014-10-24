using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneComp.Lib;

namespace PhoneComp.Controllers
{
    public class HomeController : Controller
    {
        [ToLoginFilterAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotFound()
        {
            var ss = Response;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 后台用户登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            string message = string.Empty;
            if (CurrentUser.AdminLogOn(userName, password, true, out message))
            {
                return Content("1|" + message + "|/Home/Index");
            }
            else
            {
                return Content("0|" + message);
            }
        }

        /// <summary>
        /// 后台用户登出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            CurrentUser.LogOut();
            return Content("1|登出成功|/Home/Login");
        }

        /// <summary>
        /// 判断当前用户是否已登录
        /// </summary>
        public class ToLoginFilterAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                string url = String.Empty;
                HttpContext ctx = System.Web.HttpContext.Current;
                var request = ctx.Request;
                var currentUser = ctx.User.Identity.Name;
                string controller = request.RequestContext.RouteData.Values["controller"].ToString();
                string action = request.RequestContext.RouteData.Values["action"].ToString();

                if (CurrentUser.UID <= 0)
                {
                    url = "/Home/login";
                    if (ctx.Request.FilePath != url)
                    {
                        filterContext.Result = new RedirectResult(url);
                    }
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
