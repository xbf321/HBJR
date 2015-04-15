using System.Web.Mvc;
using System.Web.Routing;
using XFramework.Mvc.Filters;

namespace XFramework.Site.Home
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SilenceHandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region == Default ==            
            #endregion
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            log4net.Config.XmlConfigurator.Configure();
            MvcHandler.DisableMvcResponseHeader = true;
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}