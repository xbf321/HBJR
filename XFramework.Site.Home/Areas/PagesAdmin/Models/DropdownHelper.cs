using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XFramework.Services;
using XFramework.Model;
using System.Data;
using System.Web.WebPages.Html;
using XFramework.Common;
using Controleng.Common;

namespace XFramework.Site.PagesAdmin.Models
{
    /// <summary>
    /// 下拉列表生成
    /// </summary>
    public static class DropdownHelper
    {
        #region == 输出类别下拉列表，语言没有在OptGroup中 ==
        /// <summary>
        /// 输出类别下拉列表，语言没有在OptGroup中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static string RenderCategoryList(string name,object selectValue) {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            sBuilder.Append(@"<option value=""0"">==请选择==</option>");
            var langDataTable = XFramework.Common.EnumHelper.EnumListTable(typeof(WebLanguage));
            foreach(DataRow dr in langDataTable.Rows){
                int value = Convert.ToInt32(dr["Value"]);
                sBuilder.AppendFormat(@"<option value=""{0}"" {2}>{1}</option>", value, dr["Text"], value.Equals(selectValue) ? "selected=\"selected\"" : string.Empty);
                var lang = (WebLanguage)Enum.Parse(typeof(WebLanguage), value.ToString());
                var oldList = CategoryService.ListByLanguage(lang).Where(p=>p.IsDeleted==false).ToList();
                var newList = new List<CategoryInfo>();
                CategoryService.BuildListForTree(newList, oldList, 0);
                foreach (var item in newList)
                {
                    sBuilder.AppendFormat("<option value=\"{0}\" parentid=\"{3}\" {2}>∟{1}</option>", item.Id, item.Name, selectValue.Equals(item.Id) ? "selected=\"selected\"" : string.Empty,item.ParentId);
                }
            }
            sBuilder.Append("</select>");
            return sBuilder.ToString();
        }
        #endregion

        #region == 输出类别下拉列表，语言在OptGroup中 ==
        /// <summary>
        /// 输出类别下拉列表，语言在OptGroup中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectValue"></param>
        /// <param name="exceptId">排除的ID</param>
        /// <returns></returns>
        public static string RenderCategoryListWithOptGroup(string name, object selectValue, params int[] exceptIds)
        {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            sBuilder.Append(@"<option value=""0"">==请选择==</option>");
            var langDataTable = XFramework.Common.EnumHelper.EnumListTable(typeof(WebLanguage));
            foreach (DataRow dr in langDataTable.Rows)
            {
                int value = Convert.ToInt32(dr["Value"]);                
                var lang = (WebLanguage)Enum.Parse(typeof(WebLanguage), value.ToString());
                var oldList = CategoryService.ListByLanguage(lang).Where(p => (p.IsDeleted == false && !exceptIds.Contains(p.Id))).ToList();
                var newList = new List<CategoryInfo>();

                CategoryService.BuildListForTree(newList, oldList, 0);

                sBuilder.AppendFormat(@"<optgroup label=""{0}"">", dr["Text"]);
                foreach (var item in newList)
                {
                    sBuilder.AppendFormat("<option value=\"{0}\" parentid=\"{3}\" {2}>{1}</option>", item.Id, item.Name, selectValue.Equals(item.Id) ? "selected=\"selected\"" : string.Empty,item.ParentId);
                }
                sBuilder.Append("</optgroup>");
            }
            sBuilder.Append("</select>");
            return sBuilder.ToString();
        }
        #endregion

        #region == 输出模板下拉列表 ==
        /// <summary>
        /// 输出模板下拉列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RenderTemplatesDropdownList(string name, object value)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DataTable dt = EnumHelper.EnumListTable(typeof(TemplateType));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    items.Add(new SelectListItem() { Text = dr[0].ToString(), Value = Convert.ToInt32(dr[1]).ToString() });
                }
            }
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            if (items != null)
            {
                foreach (SelectListItem item in items)
                {
                    string selected = "";
                    if (value != null && item.Value.Equals(value.ToString())) selected = "selected=\"selected\"";
                    sBuilder.AppendFormat(@"<option value=""{1}"" {2}>{0}</option>", item.Text, item.Value, selected);
                }
            }
            sBuilder.Append("</select>");
            return sBuilder.ToString();
        }
        #endregion

        #region == 输出产品下拉列表
        public static string RenaderProductDropdownList(string name, object selectValue)
        {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendFormat(@"<select id=""{0}"" name=""{0}"">", name);
            sBuilder.Append(@"<option value=""0"">==请选择==</option>");
            var langDataTable = XFramework.Common.EnumHelper.EnumListTable(typeof(WebLanguage));
            foreach (DataRow dr in langDataTable.Rows)
            {
                int value = Convert.ToInt32(dr["Value"]);
                var lang = (WebLanguage)Enum.Parse(typeof(WebLanguage), value.ToString());
                int rootId = CECRequest.StrToInt(LanguageResourceHelper.GetString("product-category-root-id", lang),0);                
                var oldList = CategoryService.ListByLanguage(lang).Where(p => p.IsDeleted == false).ToList();
                var newList = new List<CategoryInfo>();
                CategoryService.BuildListForTree(newList, oldList, rootId);

                sBuilder.AppendFormat(@"<optgroup label=""{0}"">", dr["Text"]);
                foreach (var item in newList)
                {
                    sBuilder.AppendFormat("<option value=\"{0}\" parentid=\"{3}\" {2}>{1}</option>", item.Id, item.Name, selectValue.Equals(item.Id) ? "selected=\"selected\"" : string.Empty, item.ParentId);
                }
                sBuilder.Append("</optgroup>");
            }
            sBuilder.Append("</select>");
            return sBuilder.ToString();
        }
        #endregion
    }
}