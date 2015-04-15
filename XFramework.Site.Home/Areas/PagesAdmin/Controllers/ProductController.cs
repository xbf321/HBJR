using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XFramework.Services;
using XFramework.Site.PagesAdmin.Models;
using XFramework.Model;
using Controleng.Common;

namespace XFramework.Site.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class ProductController : Controller
    {
        #region == List ==
        public ActionResult List()
        {
            int pageIndex = CECRequest.GetQueryInt("page", 1);
            int pageSize = 20;
            int catId = CECRequest.GetQueryInt("cid", 0);
            string txtTitle = CECRequest.GetQueryString("title");

            var productList = ProductService.List(new SearchSetting()
            {
                CategoryId = catId,
                PageIndex = pageIndex,
                Title = txtTitle,
                PageSize = pageSize
            });
            ViewBag.ProductList = productList;
            return View();
        }
        #endregion

        #region == Add or Edit ==
        public ActionResult Add()
        {
            int id = Controleng.Common.CECRequest.GetQueryInt("id", 0);
            ProductInfo model = ProductService.Get(id);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ProductInfo productInfo)
        {
            bool errors = false;
            bool isAdd = productInfo.Id == 0 ? true : false;
            if (productInfo.CategoryId == 0)
            {
                errors = true;
                ModelState.AddModelError("CAT", "请选择分类");
            }
            if (string.IsNullOrEmpty(productInfo.Title))
            {
                errors = true;
                ModelState.AddModelError("TITLE", "请输入产品标题");
            }
            if (!errors && ModelState.IsValid)
            {
                ProductService.Create(productInfo);
                if (isAdd)
                {
                    ViewBag.Msg = "添加成功！是否继续？【<a href=\"add\">是</a>】&nbsp;&nbsp;【<a href=\"list\">否</a>】";
                }
                else
                {
                    ViewBag.Msg = "修改成功！是否继续？【<a href=\"add\">是</a>】&nbsp;&nbsp;【<a href=\"list\">否</a>】";
                }
            }

            return View(productInfo);
        }
        #endregion

    }
}
