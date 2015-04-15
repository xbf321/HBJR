using System;
using System.Web.Mvc;

using XFramework.Model;
using XFramework.Services;
using Controleng.Common;
using XFramework.Site.PagesAdmin.Models;

namespace XFramework.Site.PagesAdmin.Controllers
{
    /// <summary>
    /// 类别管理
    /// </summary>
    [PagesAdminAuth]
    public class CategoryController : Controller
    {
        #region == List ==
        public ActionResult List() {
            return View();
        }
        #endregion

        #region == Ajax ==
        [HttpPost]
        public ActionResult Ajax() {
            string m = Controleng.Common.CECRequest.GetFormString("m");
            if(m == "treelist"){
                var lang = Utils.StrToInt(CECRequest.GetFormString("lang"),0);
                if (lang < 0)
                {
                    return Content(CategoryService.RenderTreeViewForAdminWithEdit((WebLanguage)Enum.Parse(typeof(WebLanguage),lang.ToString())));
                }
            }
            return Content(string.Empty);
        }
        #endregion

        #region == Add Or Edit ==
        public ActionResult Add()
        {
            int catId = CECRequest.GetQueryInt("id",0);
            var model = CategoryService.Get(catId);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(FormCollection fc,CategoryInfo modelInfo) {
            //判断是否选择中文
            //如果选择中文，则是根目录，否则为子类别
            bool isAdd = modelInfo.Id == 0 ? true : false;
            bool errors = false;
            int catId = Controleng.Common.Utils.StrToInt(fc["ddlCat"], 0);
            if(catId ==0){
                errors = true;
                ModelState.AddModelError("CAT","请选择类别");
            }
            if(string.IsNullOrEmpty(modelInfo.Name)){
                errors = true;
                ModelState.AddModelError("NAME","请输入类别名称");
            }
            //if(CategoryService.ExistsAlias(modelInfo.Id,modelInfo.Alias,)){}
            if (catId == (int)WebLanguage.en || catId == (int)WebLanguage.zh_cn)
            {
                WebLanguage lang = (WebLanguage)Enum.Parse(typeof(WebLanguage), catId.ToString());
                //说明选择的是“中文”或“英文”，添加或编辑的是跟类别
                if (CategoryService.ExistsAlias(modelInfo.Id, modelInfo.Alias, lang)) {
                    errors = true;
                    ModelState.AddModelError("ALIAS","别名已存在，请选择其他别名");
                }
                if(CategoryService.ExistsName(modelInfo.Id,modelInfo.Name,0,lang)){
                    errors = true;
                    ModelState.AddModelError("NAME","分类名称不能重复");
                }
            }
            else { 
                //说明添加的是子类别
                if (catId > 0)
                {
                    var parentModelInfo = CategoryService.Get(catId);
                    if(CategoryService.ExistsAlias(modelInfo.Id,modelInfo.Alias,parentModelInfo.Language)){
                        ModelState.AddModelError("ALIAS", "别名已存在，请选择其他别名");
                    }
                    if(CategoryService.ExistsName(modelInfo.Id,modelInfo.Name,parentModelInfo.Id,parentModelInfo.Language)){
                        errors = true;
                        ModelState.AddModelError("NAME", "分类名称不能重复");
                    }
                }
            }
            if(isAdd){
                //if(modelInfo.Alias.IndexOf("en")>0){
                //    errors = true;
                //    ModelState.AddModelError("ALIAS_EN","别名不能包含“en”");
                //}
            }
            if (!errors && ModelState.IsValid)
            {
                //modelInfo.TemplateType = Controleng.Common.Utils.StrToInt(fc["ddlTemplates"], 0);

                //在这没判断别名是否重复，暂时没做


                if (catId == (int)WebLanguage.en || catId == (int)WebLanguage.zh_cn)
                {
                    //说明添加的跟类别
                    modelInfo.Language = (WebLanguage)Enum.Parse(typeof(WebLanguage), catId.ToString());
                    modelInfo.ParentId = 0;
                    modelInfo.ParentIdList = "0";
                    CategoryService.Create(modelInfo);
                }
                else
                {
                    //说明是子类别，要获取上一类别的语言
                    if (catId > 0)
                    {
                        var parentModelInfo = CategoryService.Get(catId);
                        modelInfo.ParentId = catId;
                        modelInfo.ParentIdList = string.Format("{0},{1}", parentModelInfo.ParentIdList, modelInfo.ParentId);
                        modelInfo.Language = parentModelInfo.Language;
                        CategoryService.Create(modelInfo);
                    }
                }
                if (isAdd)
                {
                    ViewBag.Msg = "添加成功！是否继续添加？<a href=\"Add\">【是】</a>&nbsp;&nbsp;<a href=\"List?lang=-1\">【否】</a>";
                }
                else
                {
                    ViewBag.Msg = "更新成功！<a href=\"List?lang=-1\">返回</a>";
                }
            }
            
            return View(modelInfo);
        }
        #endregion

    }
}
