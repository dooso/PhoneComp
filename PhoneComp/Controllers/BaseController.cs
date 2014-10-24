using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhoneComp.Lib;

namespace PhoneComp.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        /// <summary>
        /// 检查用户是否已登录以及用户权限
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = System.Web.HttpContext.Current;
            var request = ctx.Request;
            var currentUser = ctx.User.Identity.Name;
            string controller = request.RequestContext.RouteData.Values["controller"].ToString();
            string action = request.RequestContext.RouteData.Values["action"].ToString();
            if (CurrentUser.UID <= 0)
            {
                filterContext.Result = new RedirectResult("/Home/login");
            }
            else
            {
                if ((controller.ToUpper() == "USER" || controller.ToUpper() == "ROLE")&& CurrentUser.RoleID !=1)
                {
                    filterContext.Result = new RedirectResult("/Home/Index");
                }
            }
            base.OnActionExecuting(filterContext);
        }
        
    }
}
