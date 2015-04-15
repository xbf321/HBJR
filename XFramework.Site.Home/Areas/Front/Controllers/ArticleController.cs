using System.Web.Mvc;
using System.Linq;
using System.Text.RegularExpressions;


using XFramework.Services;
using XFramework.Model;

namespace XFramework.Site.Home.Controllers
{
    public class ArticleController : FrontBaseController
    {
        public ActionResult Show()
        {
            WebLanguage lang = XFramework.Site.Home.Models.XFrontContext.Current.Language;
            /*
             总模板页需要以下变量
             * 1，根据Url获得根节点信息，因为左边需要导航信息
             * 2，根据Url获得所属类别，因为右边区域有个副导航
             */
            string fileName = Goodspeed.Web.UrlHelper.Current.FileName;

            string guid = Regex.Match(fileName, @"([a-z0-9-]+)\.html", RegexOptions.IgnoreCase).Groups[1].Value;
            var articleInfo = ArticleService.GetByGUID(guid, lang);

            if (articleInfo.Id > 0)
            {
                //获取此文章所属类别ID，用于左边导航列表
                var currentCategoryInfo = CategoryService.Get(articleInfo.CategoryId);
                if(currentCategoryInfo.IsEnabled == false && currentCategoryInfo.IsDeleted == true){
                    //说明此分类设置为未启用或者分类已删除
                    return Content("此文章的分类已删除或设置为未启用！");
                }
                //获得根类别，用于左边导航列表
                //在这里顶级类别可以是已删除或未启用的状态
                //具体判断在显示左边导航列表的时候执行
                var rootCategoryInfo = CategoryService.ListUpById(currentCategoryInfo.Id,lang).FirstOrDefault();
                ViewBag.RootCategoryInfo = rootCategoryInfo;
                ViewBag.CurrentCategoryInfo = currentCategoryInfo;
                //设置页面标题
                ViewBag.Title = articleInfo.Title;
                ViewBag.Keywords = articleInfo.Tags;
                //ViewBag.Description = Goodspeed.Common.CharHelper.Truncate(Controleng.Common.Utils.RemoveHtml(articleInfo.Content), 60);
            }
            return View(articleInfo);
        }

    }
}
