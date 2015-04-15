using System;
using System.Web;
using System.Web.Mvc;

using XFramework.Services;
using XFramework.Site.PagesAdmin.Models;

namespace XFramework.Site.PagesAdmin.Controllers
{
    /// <summary>
    /// 公告管理
    /// </summary>
    [PagesAdminAuth]
    public class NoticeController : Controller
    {
        public ActionResult List()
        {
            ViewBag.NoticeInfo = Tuple.Create(string.Empty, string.Empty, 0);
            string guid = Controleng.Common.CECRequest.GetQueryString("guid");

            string m = Controleng.Common.CECRequest.GetQueryString("m").ToLower();
            if (m == "delete")
            {
                //删除
                NoticeService.Delete(guid);
                //跳转到list
                Response.Redirect("List");
                Response.End();
            }


            if (!string.IsNullOrEmpty(guid))
            {
                ViewBag.NoticeInfo = NoticeService.Get(guid);
            }
            if (HttpContext.Request.HttpMethod == "POST")
            {
                bool errors = false;
                string title = Request.Form["title"];
                string url = Request.Form["url"];
                int sort = Controleng.Common.Utils.StrToInt(Request.Form["sort"], 0);
                if (string.IsNullOrEmpty(title))
                {
                    errors = true;
                    ModelState.AddModelError("title", "标题不能为空");
                }
                if (string.IsNullOrEmpty(url))
                {
                    errors = true;
                    ModelState.AddModelError("url", "链接不能为空");
                }
                if (!errors)
                {
                    NoticeService.Update(title, url, sort, guid);
                    //跳转到list
                    Response.Redirect("List");
                    Response.End();
                }

            }

            var noticeList = NoticeService.List();
            ViewBag.NoticeList = noticeList;

            return View();
        }

    }
}
