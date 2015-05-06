using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;


using XFramework.Services;
using XFramework.Model;
using Controleng.Common;
using XFramework.Common;

namespace XFramework.Site.Home.Controllers
{
    public class ChannelController : FrontBaseController
    {

        #region == Show ==
        public ActionResult Show()
        {
            /*
            总模板页需要以下变量
            * 1，根据Url获得根节点信息，因为左边需要导航信息
            * 2，根据Url获得所属类别，因为右边区域有个副导航
            */
            //Channel格式，必须为/Channel/(\d+).html
            //优先选择这样的格式
            int categoryId = 0;
            string fileName = Goodspeed.Web.UrlHelper.Current.FileName;
            WebLanguage lang = XFramework.Site.Home.Models.XFrontContext.Current.Language;
            string _urlCatName = Regex.Match(fileName, @"(\w+)\.html", RegexOptions.IgnoreCase).Groups[1].Value;
            if (Regex.IsMatch(_urlCatName, @"\d+"))
            {
                //不是别名
                categoryId = Controleng.Common.Utils.StrToInt(_urlCatName, 0);
            }
            else
            {
                //是别名的情况
                categoryId = CategoryService.ListByLanguage(lang).FirstOrDefault(p => p.Alias == _urlCatName).Id;
            }
            //当前节点
            var currentCategoryInfo = CategoryService.Get(categoryId);
            if (currentCategoryInfo.Id == 0 || currentCategoryInfo.IsDeleted == true)
            {
                return Content("Arguments Error!");
            }
            
            return LoadShowView(currentCategoryInfo,lang);
        }
        [NonAction]
        private ActionResult LoadShowView(CategoryInfo currentCatInfo,WebLanguage lang) {
            CategoryInfo rootCatInfo = currentCatInfo;

            //获取当前节点的顶级父节点
            //可以是未启用的，因为有的列表就是页面顶部导航不显示，但是左边需要显示
            //不显示已删除的
            var _rootCatInfo = CategoryService.ListUpById(currentCatInfo.Id, lang).Where(p=>p.IsDeleted==false).FirstOrDefault();
            if (_rootCatInfo != null)
            {
                rootCatInfo = _rootCatInfo;
            }
            //是否显示子分类的首节点
            //如果是，当前节点更改为子节点
            //只显示启用的以及未删除的
            if(currentCatInfo.IsShowFirstChildNode){
                var firstChildNode = CategoryService.ListByParentId(currentCatInfo.Id).Where(p=>(p.IsDeleted == false && p.IsEnabled == true)).FirstOrDefault();
                if(firstChildNode != null){
                    currentCatInfo = firstChildNode;
                }
            }
            //如果根节点是未启用状态并且还不是显示第一个子分类
            //说明这个根节点对外是不可访问的
            //如果是显示第一个子节点
            //说明对外根分类是不对外，但是子节点对外，可以在导航上不出现根分类，但是可以访问这个跟分类中的子分类
            if(currentCatInfo.ParentId == 0 && currentCatInfo.IsEnabled == false){
                //
                return Content("Arguments Error!");
            }

            //模板类型
            switch(currentCatInfo.TemplateType){
                case (int)TemplateType.ArticleList:
                    int pageIndex = CECRequest.GetQueryInt("page",1);
                    int pageSize = 15;
                    ViewBag.ArticleList = ArticleService.List(new SearchSetting { 
                        PageSize =pageSize,
                        PageIndex = pageIndex,
                        CategoryId = currentCatInfo.Id,
                        Language = lang
                    });
                    break;
            }
            ViewBag.RootCategoryInfo = rootCatInfo;
            ViewBag.CurrentCategoryInfo = currentCatInfo;
            return View();
        }
        #endregion

        #region == [ChildActionOnly] 输出Channel页面左边子栏目列表 ==
        /// <summary>
        /// 输出Channel页面左边子栏目列表
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SubCategoryListForChannelPage(int rootId, int selectedId, WebLanguage language = WebLanguage.zh_cn,string url = "")
        {
            if (rootId == 0)
            {
                //在当前页面不会列出此站点的所有类别的
                return Content(string.Empty);
            }
            StringBuilder sbHtml = new StringBuilder();

            var rootCategoryInfo = CategoryService.Get(rootId);
            //显示启用的以及未删除的所有分类
            var catAllList = CategoryService.ListByLanguage(language).Where(p=>(p.IsEnabled == true && p.IsDeleted == false));
            //二级分类
            var subList = catAllList.Where(p=>p.ParentId == rootId);

            //没有二分类的话，就显示主分类
            //避免菜单空着
            if(subList.Count() == 0){
                sbHtml.AppendFormat("<h3 class=\"on\"><a href=\"{1}\" title=\"{0}\">{0}</a></h3>",
                    rootCategoryInfo.Name,
                    rootCategoryInfo.Url);
                
            }
            foreach(var item in subList){
                bool isH3Selected = item.Id == selectedId ;
                sbHtml.AppendFormat("<h3{1}><a href=\"{2}\" title=\"{0}\" id=\"{3}\">{0}</a></h3>", 
                    item.Name, 
                    isH3Selected ? " class=\"on\"" : string.Empty,
                    string.IsNullOrEmpty(url) ? item.Url : string.Format((language == WebLanguage.zh_cn ? string.Empty : "/en") + url, item.Id), 
                    item.Id);

                //三级分类
                var threeList = catAllList.Where(p=>p.ParentId == item.Id);
                //是否有三级分类
                var threeListCount = threeList.Count();                
                if (threeListCount > 0)
                {
                    //有三级分类，则输出
                    bool isSubUlShow = isH3Selected ? true : threeList.Where(p => p.Id == selectedId).Count() > 0;
                    sbHtml.AppendFormat("<ul class=\"about_list2sort\" style=\"display:{1};\" id=\"sub_cat_{0}\">", item.Id, isSubUlShow ? "block" : "none");
                    foreach(var sub in threeList){
                        sbHtml.AppendFormat("<li{1}><a href=\"{2}\" title=\"{0}\">{0}</a></li>", 
                            sub.Name, 
                            sub.Id == selectedId ? " class=\"on\"" : string.Empty,
                            string.IsNullOrEmpty(url) ? sub.Url : string.Format((language == WebLanguage.zh_cn ? string.Empty : "/en") + url, sub.Id));
                    }
                    sbHtml.Append("</ul>");
                }
            }
            return Content(sbHtml.ToString());
        }
        #endregion

        #region == [ChildActionOnly] 输出Channel页面右边的当前栏目的导航信息 ==
        /// <summary>
        /// 输出Channel页面右边的当前栏目的导航信息
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderSubNavForChannelPage(int catId, string customTitle = "", WebLanguage language = WebLanguage.zh_cn,string url = "")
        {

            /*
             <span class="fgray">您所在的位置：</span><a href="#">首页</a> &gt; <a href="http://www.mtd.com.cn/about.htm">
                关于机科</a> &gt; 公司简介
             */
            StringBuilder sbNav = new StringBuilder(string.Format("<span class=\"fgray\">{2}：</span><a href=\"/{1}\">{0}</a>",
                LanguageResourceHelper.GetString("channel-sub-nav-home-text", language),
                (language == WebLanguage.zh_cn ? string.Empty : language.ToString()),
                LanguageResourceHelper.GetString("current-text", language))
             );
            if (catId > 0)
            {
                //可以显示未启用的分类，但是不能显示已删除的分类
                var upList = CategoryService.ListUpById(catId,language).Where(p=>p.IsDeleted == false);

                foreach (var item in upList)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (string.IsNullOrEmpty(item.LinkUrl))
                        {
                            sbNav.AppendFormat("&nbsp;&nbsp;>&nbsp;&nbsp;<a href=\"{0}\" title=\"{1}\">{1}</a>", string.Format((language == WebLanguage.zh_cn ? string.Empty : "/en") + url, item.Id), item.Name);
                        }
                        else {
                            sbNav.AppendFormat("&nbsp;&nbsp;>&nbsp;&nbsp;<a href=\"{0}\" title=\"{1}\">{1}</a>",item.LinkUrl,item.Name);
                        }
                    }
                    else {
                        sbNav.AppendFormat("&nbsp;&nbsp;>&nbsp;&nbsp;<a href=\"{1}\" title=\"{0}\">{0}</a>", item.Name, item.Url);
                    }
                    
                }
            }
            if (!string.IsNullOrEmpty(customTitle))
            {
                sbNav.AppendFormat("&nbsp;&nbsp;>&nbsp;&nbsp;{0}", customTitle);
            }

            return Content(sbNav.ToString());
        }
        #endregion

    }
}
