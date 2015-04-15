using System.Web.Mvc;

using XFramework.Model;
using XFramework.Services;
using Controleng.Common;
using XFramework.Site.PagesAdmin.Models;

namespace XFramework.Site.PagesAdmin.Controllers
{
    /// <summary>
    /// 文章管理
    /// </summary>
    [PagesAdminAuth]
    public class ArticleController : Controller
    {
        #region == List ==
        public ActionResult List()
        {
            int pageIndex = CECRequest.GetQueryInt("page", 1);
            int pageSize = 20;
            int catId = CECRequest.GetQueryInt("cid", 0);
            string txtTitle = CECRequest.GetQueryString("title");
            var publishDate = CECRequest.GetQueryString("m");

            var articleList = ArticleService.List(new SearchSetting()
            {
                CategoryId = catId,
                PageIndex = pageIndex,
                Title = txtTitle,
                PublishDate = publishDate,
                PageSize = pageSize,
                ShowDeleted = true
            });
            ViewBag.ArticleList = articleList;
            return View();
        }
        #endregion

        #region == Add or Edit ==
        public ActionResult Add()
        {
            int id = Controleng.Common.CECRequest.GetQueryInt("id", 0);
            ArticleInfo model = ArticleService.Get(id);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ArticleInfo articleInfo)
        {
            bool errors = false;
            bool isAdd = articleInfo.Id == 0 ? true : false;
            if (articleInfo.CategoryId == 0)
            {
                errors = true;
                ModelState.AddModelError("CAT", "请选择分类");
            }
            if (string.IsNullOrEmpty(articleInfo.Title))
            {
                errors = true;
                ModelState.AddModelError("TITLT", "请输入文章标题");
            }
            if (!errors && ModelState.IsValid)
            {
                ArticleService.Create(articleInfo);
                if (isAdd)
                {
                    ViewBag.Msg = "添加成功！是否继续？【<a href=\"add\">是</a>】&nbsp;&nbsp;【<a href=\"list\">否</a>】";
                }
                else
                {
                    ViewBag.Msg = "修改成功！是否继续？【<a href=\"add\">是</a>】&nbsp;&nbsp;【<a href=\"list\">否</a>】";
                }
            }

            return View(articleInfo);
        }
        #endregion

    }
}
