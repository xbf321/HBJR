using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XFramework.Model;
using XFramework.Data;

namespace XFramework.Services
{
    public static class CategoryService
    {
        #region == Edit Or Add ==
        /// <summary>
        /// 添加或更新分类信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns>主键ID</returns>
        public static int Create(CategoryInfo model)
        {
            if (model.Id == 0)
            {
                //Insert
                int i = ArticleCategoryManage.Insert(model);
                model.Id = i;
            }
            else
            {
                //Update
                ArticleCategoryManage.Update(model);
            }
            return model.Id;
        }
        #endregion

        #region == Delete ==
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void Delete(int id) {
            ArticleCategoryManage.Delete(id);
        }
        #endregion

        #region == Restore ==
        /// <summary>
        /// 恢复分类
        /// </summary>
        /// <param name="id"></param>
        public static void Restore(int id) {
            ArticleCategoryManage.Restore(id);
        }
        #endregion

        #region == 获得此ID的类别详细信息==
        /// <summary>
        /// 获得此ID的类别详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CategoryInfo Get(int id)
        {
            var item =  ArticleCategoryManage.Get(id);
            LoadExtionsion(item);
            return item;
        }
        #endregion

        #region == 分类名是否存在 ==
        /// <summary>
        ///分类名是否存在，在同一语言下
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static bool ExistsName(int id, string name, int parentId,WebLanguage lang)
        {
            return ArticleCategoryManage.ExistsName(id, name, parentId,lang);
        }
        #endregion

        #region == 分类别名是否存在 ==
        /// <summary>
       /// 是否存在别名，别名不允许重复
       /// </summary>
       /// <param name="appId"></param>
       /// <param name="cid"></param>
       /// <param name="englishName"></param>
       /// <returns></returns>
        public static bool ExistsAlias(int cid,string englishName,WebLanguage lang) {
            if (string.IsNullOrEmpty(englishName)) { return false; }
            return ArticleCategoryManage.ExistsAlias(cid,englishName,lang);
        }
        #endregion

        #region == 获得某语言下的全部分类,包含删除的和未启用的 ==
        /// <summary>
        /// 获得某语言下的全部分类,包含删除的和未启用的
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListByLanguage(WebLanguage lang)
        {
            var list = ArticleCategoryManage.ListByLanguage(lang);
            foreach (var item in list)
            {
                LoadExtionsion(item);
            }
            return list;
        }
        #endregion

        #region == 根据父ID获取此ID下一级栏目，只显示一级子分类，包含删除的和未启用的 ==
        /// <summary>
        /// 根据父ID获取此ID下一级栏目，只显示一级子分类，包含删除的和未启用的
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListByParentId(int parentId) {
            var _list = ArticleCategoryManage.ListByParentId(parentId);
            foreach (var item in _list)
            {
                LoadExtionsion(item);
            }
            return _list;
        }
        #endregion

        #region == 生成类别URL ==
        /// <summary>
        /// 生成类别URL
        /// </summary>
        /// <param name="catInfo"></param>
        private static void LoadExtionsion(CategoryInfo catInfo) {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(catInfo.LinkUrl))
            {
                sb.AppendFormat("{1}/channel/{0}.html",
                         string.IsNullOrEmpty(catInfo.Alias) ? catInfo.Id.ToString() : catInfo.Alias,
                         catInfo.Language == WebLanguage.zh_cn ? string.Empty : "/en");

            }
            else
            {
                //直接连接
                sb.AppendFormat("{0}", catInfo.LinkUrl);
            }
            catInfo.Url = sb.ToString();

        }
        #endregion

        #region == 根据此ID获得所有祖先节点，正序排列，包括此节点【递归实现】 ==
        /// <summary>
        /// 根据此ID获得所有祖先，正序排列，包括此节点，包含未启用的
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lang">首先根据语言缩小查找范围</param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListUpById(int id,WebLanguage lang) {
            IList<CategoryInfo> upList = new List<CategoryInfo>();
            //向上遍历则包含删除的和未启用的
            var listAll = ListByLanguage(lang);
            BuildListUpByParentId(listAll,id,ref upList);
            upList = upList.Reverse().ToList();
            foreach(var item in upList){
                LoadExtionsion(item);
            }
            return upList;
        }
        private static void BuildListUpByParentId(IEnumerable<CategoryInfo> list, int parentId, ref IList<CategoryInfo> upList)
        {
            var item = list.Where(p => p.Id == parentId).FirstOrDefault();
            if (item != null && item.Id > 0)
            {
                upList.Add(item);
                if (item.ParentId > 0)
                {
                    BuildListUpByParentId(list, item.ParentId, ref upList);
                }
            }
        }
        #endregion

        #region == 根据此ID获得所有孩子节点，正序排列，包括此节点【递归实现】 ==
        /// <summary>
        /// 根据此ID获得所有孩子节点，包括此节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lang">首先根据语言缩小查找范围</param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListDownById(int id,WebLanguage lang) {
            IList<CategoryInfo> downList = new List<CategoryInfo>();

            //向下遍历则去掉删除的和未启用的
            var listAll = ListByLanguage(lang).Where(p=>(!p.IsDeleted && p.IsEnabled));

            //添加此节点本身
            downList.Add(listAll.Where( p => p.Id == id).FirstOrDefault());

            BuildListDownByParentId(listAll, id, ref downList);
            foreach(var item in downList){
                LoadExtionsion(item);
            }
            return downList;
        }
        private static void BuildListDownByParentId(IEnumerable<CategoryInfo> list, int parentId, ref IList<CategoryInfo> downList)
        {
            var itemList = list.Where(p=>p.ParentId == parentId);
            if (itemList != null && itemList.Count() > 0)
            {
                foreach (var item in itemList) {
                    downList.Add(item);
                    BuildListDownByParentId(list, item.Id, ref downList);
                }
                
            }
        }
        #endregion        
        
        #region == 生成带有层级的树列表 ==
        /// <summary>
        /// 生成带有层级的树列表
        /// </summary>
        /// <param name="newList">新列表</param>
        /// <param name="oldList">原始数据</param>
        /// <param name="parentId">指定父类别</param>
        public static void BuildListForTree(List<CategoryInfo> newList, List<CategoryInfo> oldList, int parentId)
        {
            var plist = oldList.FindAll(nc => nc.ParentId == parentId);

            if (plist.Count == 0) { return; }
            foreach (var item in plist)
            {
                //if (item.ParentId == 0)
                {
                    newList.Add(item);
                }
                int index = newList.FindIndex(delegate(CategoryInfo m) { return m.Id == item.ParentId; });
                if (index > -1)
                {
                    #region 判断level

                    int level = 0;
                    CategoryInfo ncTmp = newList.Find(x => x.Id == item.ParentId);
                    while (ncTmp != null)
                    {
                        ncTmp = newList.Find(x => x.Id == ncTmp.ParentId);
                        level++;
                    }
                    #endregion

                    #region 插入到父级索引后

                    index += 1;
                    //如果紧跟父级的项是属于该父级的子级或者子级的子级……(递归下去)
                    while (newList.Count > index && CompareParentID(newList, newList[index], item.ParentId))
                    {
                        //则插入到该子级索引后
                        index += 1;
                    }
                    item.Name = BuildLevelString(level) + item.Name;
                    newList.Insert(index, item);
                    #endregion
                }
                BuildListForTree(newList, oldList, item.Id);
            }
        }
        /// <summary>
        /// 判断某子项的所有父项中是否存在指定父ID
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="child">子项</param>
        /// <param name="compareParentId">父ID</param>
        /// <returns></returns>
        private static bool CompareParentID(List<CategoryInfo> list, CategoryInfo child, int compareParentId)
        {
            if (child.ParentId == compareParentId) return true;
            var category = list.Find(c => c.Id == child.ParentId);
            while (category != null)
            {
                if (category.ParentId == compareParentId) return true;
                var nextParentId = category.ParentId;
                category = list.Find(c => c.Id == nextParentId);
            }
            return false;
        }
        private static string BuildLevelString(int level)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                sb.Append("∟");
            }
            return sb.ToString();
        }        
        #endregion
       
        #region == 为后台创建TreeView(编辑或删除连接)（后台调用） ==
        public static string RenderTreeViewForAdminWithEdit(WebLanguage lang)
        {
            var list = ListByLanguage(lang); //后台显示所有的，包含删除的和未启用的
            return BuildListForAdminWithEdit(list, 0,lang);
        }
        public static string BuildListForAdminWithEdit(IEnumerable<CategoryInfo> list, int parentId, WebLanguage lang)
        {
            var pList = list.Where(nc => nc.ParentId == parentId);
            if (pList.Count() == 0) { return string.Empty; }
            var sb = new StringBuilder();
            sb.AppendFormat("<ul {0}>", parentId == 0 ? "class=\"treeview-black treeview\"" : string.Empty);
            foreach (var item in pList)
            {
                sb.Append("<li>");
                sb.AppendFormat("{0}",item.Name);
                sb.AppendFormat("（{0}-{1}-{2}）",item.Id,item.Alias,(TemplateType)Enum.Parse(typeof(TemplateType),item.TemplateType.ToString(),true));
                //sb.AppendFormat("<a id=\"{1}\" title=\"{0}\">{0}（{2}）</a>", item.Name, item.Id, item.Alias);
                //显示是调用的那个模板
                //sb.AppendFormat("&nbsp;&nbsp;（{0}）",(TemplateType)Enum.Parse(typeof(TemplateType),item.TemplateType.ToString(),true));
                if (!item.IsEnabled)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">未启用</font>");
                }
                if (item.IsDeleted)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">已删除</font>");
                }
                sb.AppendFormat("&nbsp;&nbsp;<a href=\"Add?id={0}\">编辑</a>",item.Id);
                //sb.AppendFormat("&nbsp;&nbsp;<a href=\"javascript:void(0);\" onclick=\"deleteCategory({0},{1})\">删除</a>",item.Id,item.SiteId);
                //递归
                sb.Append(BuildListForAdminWithEdit(list, item.Id, lang));
                sb.AppendLine("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        #endregion
    }

}
