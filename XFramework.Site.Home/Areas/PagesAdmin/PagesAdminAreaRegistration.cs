using System.Web.Mvc;

namespace XFramework.Site.PagesAdmin
{
    public class PagesAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PagesAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PagesAdmin_default",
                "PagesAdmin/{controller}/{action}",
                new { action = "Main", controller="Home" }
            );
        }
    }
}
