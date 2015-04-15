using System.Web.Mvc;


using XFramework.Services;
using XFramework.Site.PagesAdmin.Models;

namespace XFramework.Site.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class FeedbackController : Controller
    {
        //
        // GET: /PagesAdmin/Feedback/

        public ActionResult List()
        {
            int pageIndex = 1;
            int pageSize = 10;
            pageIndex = Controleng.Common.CECRequest.GetQueryInt("page",1);
            ViewBag.List = FeedbackService.List(pageIndex,pageSize);
            return View();
        }

    }
}
