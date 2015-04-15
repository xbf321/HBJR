using System;
using System.Collections.Generic;
using System.Linq;


using XFramework.Model;
using XFramework.Data;

namespace XFramework.Services
{
    public static class ArticleService
    {
        #region == Edit OR Add ==
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Create(ArticleInfo model) {
            if (model.Id > 0)
            {
                //Update
                ArticleManage.Update(model);
                
            }
            else {
                int i = ArticleManage.Insert(model);
                model.Id = i;
            }
            return model.Id;
        }
        #endregion

        #region == List With Pager ==
        /// <summary>
        /// 查询，带分页
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<ArticleInfo> List(SearchSetting setting) {
            var list = ArticleManage.List(setting);
            foreach(var item in list){
                LoadExtensionInfo(item,setting.Language);
            }
            return list;
        }
        #endregion

        #region == 根据类别ID获取TOP几条，没有分页 ==
        /// <summary>
        /// 根据类别ID获取文章，没有分页
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPage(int categoryId,int topCount,WebLanguage language = WebLanguage.zh_cn) {
            return ListWithoutPage(categoryId,topCount,false,language);
        }
        /// <summary>
        /// 根据类别ID获取文章,没有分页
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="topCount"></param>
        /// <param name="topOneImage">首条是否需要图片，默认False</param>
        /// <param name="language">语言，主要生成URL用</param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPage(int categoryId, int topCount, bool topOneImage,WebLanguage language) {
            var list = ArticleManage.ListWithoutPage(categoryId, topCount,topOneImage);
            foreach (var item in list)
            {
                LoadExtensionInfo(item,language);
            }
            return list;
        }
        /// <summary>
        /// 根据类别名获取文章,没有分页
        /// </summary>
        /// <param name="categoryNames">格式：【首页设置/焦点图片】或【首页设置】</param>
        /// <param name="topCout">获取几条</param>
        /// <param name="language">语言，主要生成URL用</param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPageV2(string categoryNames, int topCount, WebLanguage language = WebLanguage.zh_cn)
        {
            return ListWithoutPageV2(categoryNames, topCount, false, language);
        }
        /// <summary>
        /// 根据类别名获取文章,没有分页
        /// </summary>
        /// <param name="categoryNames">格式：【首页设置/焦点图片】或【首页设置】</param>
        /// <param name="topCount"></param>
        /// <param name="topOneImage">首条是否是图片，默认False</param>
        /// <param name="language">语言，主要生成URL用</param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPageV2(string categoryNames, int topCount, bool topOneImage, WebLanguage language = WebLanguage.zh_cn)
        {

            List<string> _categoryNames = categoryNames.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Func<List<string>, int, int> fb = null;
            //不显示删除的分类
            var categoryList = CategoryService.ListByLanguage(language).Where(p=>p.IsDeleted==false);
            fb = (n, pid) =>
            {
                string _name = n[0];
                var _item = categoryList.Where(p => p.Name == _name && p.ParentId == pid).FirstOrDefault();
                if (_item == null || _item.Id == 0) { return 0; }
                n.Remove(_name);
                if (n.Count == 0) return _item.Id;
                return fb(n, _item.Id);
            };

            int cid = fb(_categoryNames, 0);
            return ListWithoutPage(cid, topCount, topOneImage, language);
        }
        
        #endregion

        #region == 根据文章ID,获得文章详细信息 ==
        /// <summary>
        /// 根据文章ID,获得文章详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ArticleInfo Get(int id)
        {
            var model = ArticleManage.Get(id);
            return model;
        }
        #endregion

        #region == 添加扩展信息 ==
        /// <summary>
        /// 添加扩展信息，主要生成文章URL
        /// </summary>
        /// <param name="model"></param>
        /// <param name="language"></param>
        private static void LoadExtensionInfo(ArticleInfo model,WebLanguage language = WebLanguage.zh_cn) {

            if (!string.IsNullOrEmpty(model.LinkUrl))
            {
                model.Url = model.LinkUrl;
            }
            else
            {
                model.Url = string.Format("{2}/{0}/{1}.html", "n", model.GUID.ToString().ToLower(), language == WebLanguage.zh_cn ? string.Empty : "/" + language.ToString());
            }
        }
        #endregion

        #region == 根据文章URl，获得文章详细信息 ==
        /// <summary>
        /// 根据GUID获得文章详细信息
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="language">语言，主要生成URL用</param>
        /// <returns></returns>
        public static ArticleInfo GetByGUID(string guid,WebLanguage language = WebLanguage.zh_cn) { 
            var model = ArticleManage.GetByGUID(guid);
            LoadExtensionInfo(model, language);
            return model;
        }
        #endregion

        #region == 查询,暂时没用 ==
        /// <summary>
        /// 查询页面所调用方法,获取所有的数据
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        //public static List<ArticleInfo> Seek(int siteId, string key)
        //{
        //    var list = ArticleManage.Seek(key);
        //    foreach(var item in list){
        //        LoadExtensionInfo(item);
        //    }
        //    return list;
        //}
        #endregion

        #region == 获得某一站点所有文章的发布时间，主要为后台查询文章用,暂时没用 ==
        /// <summary>
        /// 获得某一站点所有文章的发布时间，主要为后台查询文章用
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        //public static List<Tuple<string, string>> GetAllPublishDateBySiteId(int siteId) {
        //    //return ArticleManage.GetAllPublishDateBySiteId(siteId);
        //    return null;
        //}
        #endregion
    }
}
