using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XFramework.Site.Home.Controllers
{
    public class SurveyController : FrontBaseController
    {
        //
        // GET: /Front/Survey/

        public ActionResult Index()
        {
            Response.Redirect("/");
            Response.End();

            return View();
        }

        public ActionResult Show(string file) {
            string viewName = string.Format("{0}/{1}",XFramework.Site.Home.Models.XFrontContext.Current.Language,file);
            return View(viewName);
        }
        [HttpPost]
        public ActionResult Submit() {
            return View();
        }

        public ActionResult Test() {
            return View();
        }

    }
}
