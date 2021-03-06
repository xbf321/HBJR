﻿using System.Linq;
using System.Web.Mvc;
using System.Text;

using XFramework.Services;
using XFramework.Model;
using XFramework.Site.Home.Models;
using System;
using XFramework.Common;

namespace XFramework.Site.Home.Controllers
{
    public class HomeController : FrontBaseController
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            
            WebLanguage lang = XFrontContext.Current.Language;
            string viewName = string.Format("Index_{0}", lang.ToString());
            int productRootId = Convert.ToInt32(LanguageResourceHelper.GetString("product-category-root-id", lang));
            if (lang == WebLanguage.zh_cn)
            {
                //显示启用的以及未删除的

                ViewBag.ProductService = CategoryService.ListByParentId(productRootId).Where(p => (p.IsEnabled == true && p.IsDeleted == false)).Take(8).ToList();


                ViewBag.CompanyNews = ArticleService.ListWithoutPageV2("新闻动态", 7, lang);

                ViewBag.FocusImageList = ArticleService.ListWithoutPageV2("首页设置/焦点图片", 5, lang);

                ViewBag.Business = ArticleService.ListWithoutPageV2("技术资料", 7, lang);

                ViewBag.NoticeList = NoticeService.List();
            }
            if(lang == WebLanguage.en){
                ViewBag.ProductService = CategoryService.ListByParentId(productRootId).Where(p => (p.IsEnabled == true && p.IsDeleted == false)).Take(8).ToList();
            }
            return View(viewName);
        }
        public ActionResult Ping() {
            return Content("Ok");
        }
        /// <summary>
        /// 输出模板页的导航
        /// </summary>
        /// <param name="Language"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderHeaderMenu(WebLanguage Language = WebLanguage.zh_cn)
        {
            StringBuilder sb = new StringBuilder();
            //只显示启用的和未删除的一级分类
            var menuList = CategoryService.ListByLanguage(Language).Where(m => (m.ParentId == 0 && m.IsDeleted == false && m.IsEnabled == true));
            foreach(var item in menuList){
                sb.AppendFormat("<li id=\"{0}\"><a href=\"{2}\" title=\"{1}\">{1}</a></li>", item.Id,item.Name,item.Url);
            }
            return Content(sb.ToString());
        }

        public ActionResult Search() {
            return View();
        }
    }
}
