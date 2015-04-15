using System.Web.Mvc;
using XFramework.Services;
using Controleng.Common;
using XFramework.Model;

namespace XFramework.Site.Home.Controllers
{
    public class DownloadController : FrontBaseController
    {
        //
        // GET: /Front/Download/

        public ActionResult List()
        {
            //在线互动
            ViewBag.RootCategoryInfo = CategoryService.Get(87);
            //相关下载
            ViewBag.CurrentCategoryInfo = CategoryService.Get(91);


            int pageIndex = CECRequest.GetQueryInt("page",1);

            ViewBag.List = AttachmentService.List(new SearchSetting() { 
                PageIndex = pageIndex,
                PageSize = 20,
                ShowDeleted = false
            });

            return View();
        }

    }
}
