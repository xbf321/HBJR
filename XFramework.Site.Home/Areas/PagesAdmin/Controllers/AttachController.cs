using System.Web.Mvc;


using XFramework.Model;
using XFramework.Services;
using XFramework.Site.PagesAdmin.Models;

namespace XFramework.Site.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class AttachController : Controller
    {
        //
        // GET: /PagesAdmin/Attach/

        public ActionResult List()
        {
            int pageIndex = Controleng.Common.CECRequest.GetQueryInt("page",1);
            ViewBag.attachList = AttachmentService.List(new SearchSetting() { 
                PageIndex = pageIndex,
                PageSize = 10,
                ShowDeleted = true
            });
            return View();
        }
        public ActionResult Add() {
            int id = Controleng.Common.CECRequest.GetQueryInt("id",0);
            var model = AttachmentService.Get(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(AttachmentInfo model) {
            bool errors = false;
            if(string.IsNullOrEmpty(model.Title)){
                errors = true;
                ModelState.AddModelError("Title","请输入标题");
            }
            if(!errors && ModelState.IsValid){
                var newModel = AttachmentService.Create(model);
                ViewBag.Msg = "保存成功！<a href=\"list\">返回</a>";
            }
            return View();
        }

    }
}
