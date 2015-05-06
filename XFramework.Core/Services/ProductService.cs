using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XFramework.Model;
using XFramework.Data;

namespace XFramework.Services
{
    public static class ProductService
    {
        #region == Edit OR Add ==
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Create(ProductInfo model)
        {
            if (model.Id > 0)
            {
                //Update
                ProductManage.Update(model);

            }
            else
            {
                int i = ProductManage.Insert(model);
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
        public static IPageOfList<ProductInfo> List(SearchSetting setting)
        {
            var list = ProductManage.List(setting);
            foreach(var item in list){
                LoadExtendInfo(item,setting.Language);
            }
            return list;
        }
        #endregion

        #region == 根据ID,获得详细信息 ==
        /// <summary>
        /// 根据ID,获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProductInfo Get(int id)
        {
            var model = ProductManage.Get(id);
            return model;
        }
        #endregion

        #region == 根据URl，获得详细信息 ==
        /// <summary>
        /// 根据GUID获得详细信息
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="language">语言，主要生成URL用</param>
        /// <returns></returns>
        public static ProductInfo GetByGUID(string guid, WebLanguage language = WebLanguage.zh_cn)
        {
            var model = ProductManage.GetByGUID(guid);
            LoadExtendInfo(model);
            return model;
        }
        #endregion

        private static void LoadExtendInfo(ProductInfo model,WebLanguage language = WebLanguage.zh_cn) {
            model.LinkUrl = string.Format("{0}/product/detail?guid={1}",language == WebLanguage.zh_cn ? string.Empty : "/en",model.GUID);
        }

        #region == 更新浏览数 ==
        public static void UpdateViewCount(int id) {
            ProductManage.UpdateViewCount(id);
        }
        #endregion
    }
}
