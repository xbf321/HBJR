using System;

using System.Data;
using System.Data.SqlClient;

using XFramework.Model;
using XFramework.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace XFramework.Data
{
    internal static class ArticleManage
    {

        #region == Add ==
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(ArticleInfo model) {
            string strSQL = "INSERT INTO Articles(CategoryId,Title,Content,ImageUrl,LinkUrl,Tags,Sort,IsTop,IsDeleted,PublishDateTime,CreateDateTime,Remark,Timespan) VALUES(@CategoryId,@Title,@Content,@ImageUrl,@LinkUrl,@Tags,@Sort,@IsTop,@IsDeleted,@PublishDateTime,GETDATE(),@Remark,@Timespan);SELECT @@IDENTITY;";

            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Content",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("LinkUrl",SqlDbType.NVarChar),
                                    new SqlParameter("Tags",SqlDbType.NVarChar),
                                    new SqlParameter("IsTop",SqlDbType.Int),
                                    new SqlParameter("IsDeleted",SqlDbType.Int),
                                    new SqlParameter("PublishDateTime",SqlDbType.DateTime),
                                    new SqlParameter("Remark",SqlDbType.NVarChar),
                                    new SqlParameter("TimeSpan",SqlDbType.NVarChar),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.CategoryId;
            parms[2].Value = string.IsNullOrEmpty(model.Title) ? string.Empty : model.Title;
            parms[3].Value = string.IsNullOrEmpty(model.Content) ? string.Empty :model.Content;
            parms[4].Value = string.IsNullOrEmpty(model.ImageUrl) ? string.Empty : model.ImageUrl;
            parms[5].Value = string.IsNullOrEmpty(model.LinkUrl) ? string.Empty : model.LinkUrl;
            parms[6].Value = string.IsNullOrEmpty(model.Tags) ? string.Empty : model.Tags;
            parms[7].Value = model.IsTop;
            parms[8].Value = model.IsDeleted;            
            parms[9].Value = model.PublishDateTime<= DateTime.MinValue ? DateTime.Now : model.PublishDateTime;
            parms[10].Value = string.IsNullOrEmpty(model.Remark) ? string.Empty : model.Remark;
            parms[11].Value = string.IsNullOrEmpty(model.Timespan) ? DateTime.Now.ToString("HHmmssfff") : model.Timespan;
            parms[12].Value = model.Sort;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        #endregion

        #region == Update ==
        /// <summary>
       /// 更新
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        public static int Update(ArticleInfo model) {
            string strSQL = "UPDATE Articles SET CategoryId = @CategoryId,Title = @Title,Content = @Content,ImageUrl = @ImageUrl,LinkUrl = @LinkUrl,Tags = @Tags,Sort = @Sort,IsTop = @IsTop,PublishDateTime = @PublishDateTime,IsDeleted = @IsDeleted,Remark = @Remark WHERE ID = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Content",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("LinkUrl",SqlDbType.NVarChar),
                                    new SqlParameter("Tags",SqlDbType.NVarChar),
                                    new SqlParameter("IsTop",SqlDbType.Int),
                                    new SqlParameter("IsDeleted",SqlDbType.Int),
                                    new SqlParameter("PublishDateTime",SqlDbType.DateTime),
                                    new SqlParameter("Remark",SqlDbType.NVarChar),
                                    new SqlParameter("TimeSpan",SqlDbType.NVarChar),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.CategoryId;
            parms[2].Value = string.IsNullOrEmpty(model.Title) ? string.Empty : model.Title;
            parms[3].Value = string.IsNullOrEmpty(model.Content) ? string.Empty : model.Content;
            parms[4].Value = string.IsNullOrEmpty(model.ImageUrl) ? string.Empty : model.ImageUrl;
            parms[5].Value = string.IsNullOrEmpty(model.LinkUrl) ? string.Empty : model.LinkUrl;
            parms[6].Value = string.IsNullOrEmpty(model.Tags) ? string.Empty : model.Tags;
            parms[7].Value = model.IsTop;
            parms[8].Value = model.IsDeleted;
            parms[9].Value = model.PublishDateTime <= DateTime.MinValue ? DateTime.Now : model.PublishDateTime;
            parms[10].Value = string.IsNullOrEmpty(model.Remark) ? string.Empty : model.Remark;
            parms[11].Value = string.IsNullOrEmpty(model.Timespan) ? DateTime.Now.ToString("HHmmssfff") : model.Timespan;
            parms[12].Value = model.Sort;
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        #endregion        

        #region == ListWithoutPage ==
        public static List<ArticleInfo> ListWithoutPage(int categoryId, int topCount)
        {
            return ListWithoutPage(categoryId, topCount, false);
        }
        /// <summary>
        /// 获得文章列表
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="topCount">默认10条</param>
        /// <returns></returns>
        public static List<ArticleInfo> ListWithoutPage(int categoryId, int topCount,bool isTopOneImg)
        {
            List<ArticleInfo> list = new List<ArticleInfo>();
            if (categoryId == 0) { return list; }
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(@"WITH TEP_Articles AS (
	                        SELECT TOP(50) ROW_NUMBER() OVER(ORDER BY IsTop DESC,Sort ASC,PublishDateTime DESC/*首先按置顶，再按排序，其次按发布时间*/) AS RowNumber,* FROM Articles WITH(NOLOCK) WHERE EXISTS(
                                    SELECT * FROM Categories AS AC WITH(NOLOCK)
                                    WHERE  (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)	/*获取此分类下所有的子分类*/
                                    AND IsDeleted = 0 /*获取未删除的*/
                                    /*AND IsEnabled = 1*/ /*可能某个栏目就是默认不显示*/
                                    AND Articles.CategoryId = AC.ID
	                        ) AND IsDeleted = 0 /*获取未删除的*/
	                        AND PublishDateTime < DATEADD(day,1,GETDATE()) /*小于等于当天的新闻*/
)");
            if (isTopOneImg) {
                //第一张是图片
                sbSQL.Append(@"SELECT TOP(1)* FROM TEP_Articles WHERE ImageUrl <> ''");
                //如果就取TOP(1),则不用再取剩余的
                if(topCount > 1){
                    sbSQL.AppendFormat(@"   UNION ALL 
                                            SELECT TOP({0})* FROM TEP_Articles AS P WHERE NOT EXISTS(
	                                            SELECT TOP(1)* FROM TEP_Articles AS C WHERE ImageUrl <> ''
	                                            AND P.ID = C.ID
                                            )",topCount-1);
                }
            } else {
                sbSQL.AppendFormat("SELECT TOP({0})* FROM TEP_Articles",topCount);
            }
            SqlParameter[] parms = { 
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("TopCount",SqlDbType.Int),
                                   };
            parms[0].Value = categoryId;
            parms[1].Value = topCount <= 0 ? 10 : topCount;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, sbSQL.ToString(),parms);
            ArticleInfo model = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region == ListWithPage ==
        public static IPageOfList<ArticleInfo> List(SearchSetting setting)
        {
            SqlParameter[] parms = { 
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("PublishDate",SqlDbType.NVarChar)
                                   };
            parms[0].Value = setting.CategoryId;
            parms[1].Value = setting.Title;
            parms[2].Value = setting.PublishDate;

            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Articles";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = "IsTop DESC,Sort ASC,PublishDateTime DESC";
            StringBuilder sbCondition = new StringBuilder();
            sbCondition.Append(@"EXISTS(
		                            SELECT * FROM Categories AS AC WITH(NOLOCK) 
		                            WHERE (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
		                            AND p.CategoryId = AC.ID
                                )");
            if (!setting.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 /*获取未删除的*/");
            }
            if (!string.IsNullOrEmpty(setting.Title))
            {
                sbCondition.Append("    AND CONTAINS(Title,@Title)  ");
            }
            if (Regex.IsMatch(setting.PublishDate, @"^\d{4}-\d{1,2}-\d{1,2}$", RegexOptions.IgnoreCase))
            {
                sbCondition.Append("    AND CONVERT(VARCHAR(10),PublishDateTime,120) = @PublishDate");
            }

            fp.Condition = sbCondition.ToString();
            IList<ArticleInfo> list = new List<ArticleInfo>();
            ArticleInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(), parms);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }
            int count = CountForList(setting);
            return new PageOfList<ArticleInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        private static int CountForList(SearchSetting setting)
        {
            SqlParameter[] parms = { 
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("PublishDate",SqlDbType.NVarChar)
                                   };
            parms[0].Value = setting.CategoryId;
            parms[1].Value = setting.Title;
            parms[2].Value = setting.PublishDate;
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(@"SELECT COUNT(*) AS c  FROM Articles AS p WITH(NOLOCK)
                        WHERE EXISTS(
	                        SELECT * FROM Categories AS AC WITH(NOLOCK) 
		                    WHERE (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
		                    AND p.CategoryId = AC.ID
                        )");
            sbSQL.Append("  AND IsDeleted = 0   ");
            if (!string.IsNullOrEmpty(setting.Title))
            {
                sbSQL.Append("    AND CONTAINS(Title,@Title)  ");
            }
            if (Regex.IsMatch(setting.PublishDate, @"^\d{4}-\d{1,2}-\d{1,2}$", RegexOptions.IgnoreCase))
            {
                sbSQL.Append("    AND CONVERT(VARCHAR(10),PublishDateTime,120) = @PublishDate");
            }

            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbSQL.ToString(), parms));
        }
        #endregion

        #region == 获得文章详细信息 ==
        public static ArticleInfo Get(int id) {
            if (id == 0) { return new ArticleInfo(); }
            string strSQL = "SELECT * FROM Articles WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm));
        }
        public static ArticleInfo GetByGUID(string guid)
        {
            string strSQL = "SELECT TOP(1) * FROM Articles WITH(NOLOCK) WHERE GUID = @GUID";
            SqlParameter parm = new SqlParameter("GUID", guid);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm));
        }
        private static ArticleInfo Get(DataRow dr) {
            ArticleInfo model = new ArticleInfo();
            if(dr != null){
                model.CategoryId = dr.Field<int>("CategoryId");
                model.Content = dr.Field<string>("Content");
                model.CreateDateTime = dr.Field<DateTime>("CreateDateTime");
                model.GUID = dr.Field<Guid>("GUID");
                model.Id = dr.Field<int>("Id");
                model.ImageUrl = dr.Field<string>("ImageUrl");
                model.IsDeleted = dr.Field<bool>("IsDeleted");
                model.IsTop = dr.Field<bool>("IsTop");
                model.LinkUrl = dr.Field<string>("LinkUrl");
                model.PublishDateTime = dr.Field<DateTime>("PublishDateTime");
                model.Remark = dr.Field<string>("Remark");
                model.Sort = dr.Field<int>("Sort");
                model.Tags = dr.Field<string>("Tags");
                model.Timespan = dr.Field<string>("Timespan");
                model.Title = dr.Field<string>("Title");
            }
            return model;
        }
        #endregion

        #region == 查询页面所调用方法,根据关键词获得所有的数据，暂时没用 ==
        /// <summary>
        /// 查询页面所调用方法
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<ArticleInfo> Seek(string keyCondition)
        {

            string strSQL = string.Format(@"WITH CatInfo AS /*首先获得所有启用并且未删除的分类，有可能父节点未启用，儿子节点启用了，所以在这边从上到下过滤一下，只要父节点未启用，其子节点一律不显示*/
(
	SELECT ID,ParentId,Name,ParentIdList FROM Categories  WITH(NOLOCK)
	WHERE ParentId = 0
	AND IsEnabled = 1
	AND IsDeleted = 0
	UNION ALL
	SELECT A.ID,A.ParentId,A.Name,A.ParentIdList FROM Categories AS A WITH(NOLOCK),CatInfo AS B
	WHERE A.ParentId = B.ID
	AND A.IsEnabled = 1
	AND A.IsDeleted = 0
)
SELECT * FROM Articles WITH(NOLOCK) WHERE EXISTS(
	SELECT * FROM CatInfo WITH(NOLOCK)
	WHERE Articles.CategoryId = CatInfo.ID
)
AND IsDeleted = 0 /*AND (CONTAINS(Title,@Key) OR CONTAINS(Title,'技术'))*/ {0}", keyCondition);
            List<ArticleInfo> list = new List<ArticleInfo>();
            ArticleInfo model = null;
            try
            {
                DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, strSQL);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        model = Get(dr);
                        if (model != null)
                        {
                            list.Add(model);
                        }
                    }
                }
            }
            catch{ }
            return list;
        }
        #endregion       

        #region == 获得某一站点所有文章的发布时间，主要为查询文章用，暂时没用 ==
        //        /// <summary>
        //        /// 获得某一站点所有文章的发布时间，主要为查询文章用Key:date,Value:date
        //        /// </summary>
        //        /// <param name="siteId"></param>
        //        /// <returns></returns>
        //        public static List<Tuple<string, string>> GetAllPublishDateBySiteId(int siteId) {
        //            string strSQL = @"SELECT CONVERT(VARCHAR(10),PublishDateTime,120) AS P FROM Articles WITH(NOLOCK) WHERE EXISTS(
        //	SELECT * FROM ArticleInCategories WITH(NOLOCK) WHERE SiteId = @SiteId
        //	AND Articles.ID = ArticleInCategories.ArticleId
        //)
        //GROUP BY CONVERT(VARCHAR(10),PublishDateTime,120)
        //ORDER BY P DESC";
        //            SqlParameter parm = new SqlParameter("@SiteId",siteId);
        //            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,parm);
        //            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
        //            if(dt != null){
        //                foreach(DataRow row in dt.Rows){
        //                    string date = row[0].ToString();
        //                    list.Add(Tuple.Create(date,date));
        //                }
        //            }
        //            return list;
        //        }
        #endregion
        
    }
}
