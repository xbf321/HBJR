using System;
using System.Web.Mvc;


using XFramework.Services;
using XFramework.Model;
using XFramework.Common;
using Controleng.Common;

namespace XFramework.Site.Home.Controllers
{
    public class ProductController : FrontBaseController
    {
        //
        // GET: /Front/Product/

        public ActionResult Index()
        {
            
            WebLanguage lang = XFramework.Site.Home.Models.XFrontContext.Current.Language;

            int rootId = Convert.ToInt32(LanguageResourceHelper.GetString("product-category-root-id",lang));
            int pageIndex = CECRequest.GetQueryInt("page", 1);
            int cid = CECRequest.GetQueryInt("cid", rootId);
            var currentCategoryInfo = CategoryService.Get(cid);

            ViewBag.RootCategoryInfo = CategoryService.Get(rootId);
            ViewBag.CurrentCategoryInfo = currentCategoryInfo;

            ViewBag.Title = currentCategoryInfo.Name;            

            ViewBag.List = ProductService.List(new SearchSetting()
            {
                PageIndex = pageIndex,
                PageSize = 20,
                CategoryId = cid,
                ShowDeleted = false
            });

            return View();
        }

        public ActionResult Detail() {
            string guid = CECRequest.GetQueryString("guid");
            WebLanguage lang = XFramework.Site.Home.Models.XFrontContext.Current.Language;
            var productInfo = ProductService.GetByGUID(guid,lang);
            if(productInfo.Id ==0 ){
                Response.Redirect("/product");
            }

            int rootId = Convert.ToInt32(LanguageResourceHelper.GetString("product-category-root-id", lang));
            ViewBag.RootCategoryInfo = CategoryService.Get(rootId);
            ViewBag.CurrentCategoryInfo = CategoryService.Get(productInfo.CategoryId);

            return View(productInfo);
        }



    }
}
