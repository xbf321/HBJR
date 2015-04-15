using System;
using System.Web;

namespace XFramework.Site.PagesAdmin.Models
{
    public class PagesAdminContext
    {
        private PagesAdminContext()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static readonly string NOTAUTHTEXT = "您没有权限，请联系系统管理员！";
        public static PagesAdminContext Current
        {
            get
            {
                return new PagesAdminContext();
            }
        }
        public string UserName
        {
            get
            {
                string[] s = GetCookieValue().Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length > 1)
                {
                    return s[1];
                }
                return string.Empty;
            }
        }

        public bool IsLogin
        {
            get
            {
                if (!string.IsNullOrEmpty(UserName)) { return true; }
                return false;
            }
        }
        public string GetCookieValue()
        {
            string cookieValue = string.Empty;
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[System.Configuration.ConfigurationManager.AppSettings["COOKIENAME"]] != null)
            {
                cookieValue = HttpContext.Current.Request.Cookies[System.Configuration.ConfigurationManager.AppSettings["COOKIENAME"]].Value.ToString();
            }
            if (!string.IsNullOrEmpty(cookieValue))
            {
                cookieValue = Goodspeed.Library.Security.DESCryptography.Decrypt(cookieValue, System.Configuration.ConfigurationManager.AppSettings["DESKey"]);
            }
            return cookieValue;
        }
    }
}