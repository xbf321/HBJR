using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XFramework.Site.PagesAdmin.Models
{
    public class PagesAdminAuthAttribute : FilterAttribute, IAuthorizationFilter
    {

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //验证用户是否登录
            string controller = filterContext.RouteData.Values["controller"] as string;
            string action = filterContext.RouteData.Values["action"] as string;
            if(!PagesAdminContext.Current.IsLogin){
                filterContext.HttpContext.Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginUrl"]);
                filterContext.HttpContext.Response.End();
            }
        }

        #endregion
    }
}