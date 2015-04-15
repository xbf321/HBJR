using System;
using System.Linq;
using System.Web.Mvc;

using XFramework.Model;
using XFramework.Services;

namespace XFramework.Site.Home.Controllers
{
    /// <summary>
    /// 在线留言
    /// </summary>
    public class FeedbackController : FrontBaseController
    {
        /// <summary>
        /// 留言
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Index(FeedbackInfo model) {      

            //POST
            if(String.Equals(Request.HttpMethod,"POST",StringComparison.OrdinalIgnoreCase)){
                _Submit(model,"在线留言");
            }
            return View();
        }
        /// <summary>
        /// 投诉建议
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Advise(FeedbackInfo model) {
            //POST
            if (String.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                _Submit(model, "投诉建议");
            }
            return View();
        }
        [NonAction]
        private void _Submit(FeedbackInfo model,string feedbackType) {
            bool errors = false;
            if (string.IsNullOrEmpty(model.Realname))
            {
                errors = true;
                ModelState.AddModelError("Realname", "请填写您的姓名");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                errors = true;
                ModelState.AddModelError("Email", "请填写您的Email");
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                if (!Controleng.Common.Utils.IsValidEmail(model.Email))
                {
                    errors = true;
                    ModelState.AddModelError("Email", "请填写正确的Email格式");
                }
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                errors = true;
                ModelState.AddModelError("Title", "请填写标题");
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                errors = true;
                ModelState.AddModelError("Content", "请填写内容");
            }
            if (!errors && ModelState.IsValid)
            {
                model.IP = Goodspeed.Common.BrowserInfo.Current.IP;
                model.FeedbackType = feedbackType;

                model.Content = Goodspeed.Library.Char.HtmlHelper.RemoveHtml(model.Content);

                int id = FeedbackService.Post(model);
                if (id > 0)
                {
                    ViewBag.Msg = "留言或建议成功！";
                }
                else
                {
                    ViewBag.Msg = "出现错误，请重试。";
                }
            }                
        }

    }
}
