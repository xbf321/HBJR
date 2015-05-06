using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;


using XFramework.Model;
using XFramework.Common;
using System.Text.RegularExpressions;

namespace XFramework.Data
{
    public static class ProductManage
    {
        #region == Add ==
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(ProductInfo model)
        {
            string strSQL = "INSERT INTO Products(CategoryId,Title,Content,SImageUrl,BImageUrl,PublishDateTime,IsDeleted) VALUES(@CategoryId,@Title,@Content,@SImageUrl,@BImageUrl,@PublishDateTime,@IsDeleted);SELECT @@IDENTITY;";

            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Content",SqlDbType.NVarChar),
                                    new SqlParameter("SImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("BImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("PublishDateTime",SqlDbType.DateTime),
                                    new SqlParameter("IsDeleted",SqlDbType.Bit)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.CategoryId;
            parms[2].Value = string.IsNullOrEmpty(model.Title) ? string.Empty : model.Title;
            parms[3].Value = string.IsNullOrEmpty(model.Content) ? string.Empty : model.Content;
            parms[4].Value = string.IsNullOrEmpty(model.SImageUrl) ? string.Empty : model.SImageUrl;
            parms[5].Value = string.IsNullOrEmpty(model.BImageUrl) ? string.Empty : model.BImageUrl;
            parms[6].Value = model.PublishDateTime <= DateTime.MinValue ? DateTime.Now : model.PublishDateTime;
            parms[7].Value = model.IsDeleted;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        #endregion

        #region == Update ==
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(ProductInfo model)
        {
            string strSQL = "UPDATE Products SET CategoryId = @CategoryId,Title = @Title,Content = @Content,SImageUrl = @SImageUrl,BImageUrl = @BImageUrl,PublishDateTime = @PublishDateTime,IsDeleted = @IsDeleted  WHERE ID = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Content",SqlDbType.NVarChar),
                                    new SqlParameter("SImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("BImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("PublishDateTime",SqlDbType.DateTime),
                                    new SqlParameter("IsDeleted",SqlDbType.Bit)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.CategoryId;
            parms[2].Value = string.IsNullOrEmpty(model.Title) ? string.Empty : model.Title;
            parms[3].Value = string.IsNullOrEmpty(model.Content) ? string.Empty : model.Content;
            parms[4].Value = string.IsNullOrEmpty(model.SImageUrl) ? string.Empty : model.SImageUrl;
            parms[5].Value = string.IsNullOrEmpty(model.BImageUrl) ? string.Empty : model.BImageUrl;
            parms[6].Value = model.PublishDateTime <= DateTime.MinValue ? DateTime.Now : model.PublishDateTime;
            parms[7].Value = model.IsDeleted;

            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        #endregion

        #region == ListWithPage ==
        public static IPageOfList<ProductInfo> List(SearchSetting setting)
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
            fp.TableName = "Products";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = "PublishDateTime DESC";
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

            IList<ProductInfo> list = new List<ProductInfo>();
            ProductInfo model = null;
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
            return new PageOfList<ProductInfo>(list, setting.PageIndex, setting.PageSize, count);
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
            sbSQL.Append(@"SELECT COUNT(*) AS c  FROM Products AS p WITH(NOLOCK)
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

        #region == 获得详细信息 ==
        public static ProductInfo Get(int id)
        {
            if (id == 0) { return new ProductInfo(); }
            string strSQL = "SELECT * FROM Products WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", id);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm));
        }
        public static ProductInfo GetByGUID(string guid)
        {
            string strSQL = "SELECT TOP(1) * FROM Products WITH(NOLOCK) WHERE GUID = @GUID";
            SqlParameter parm = new SqlParameter("GUID", guid);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm));
        }
        private static ProductInfo Get(DataRow dr)
        {
            ProductInfo model = new ProductInfo();
            if (dr != null)
            {
                model.CategoryId = dr.Field<int>("CategoryId");
                model.Content = dr.Field<string>("Content");
                model.CreateDateTime = dr.Field<DateTime>("CreateDateTime");
                model.GUID = dr.Field<Guid>("GUID");
                model.Id = dr.Field<int>("Id");
                model.SImageUrl = dr.Field<string>("SImageUrl");
                model.BImageUrl = dr.Field<string>("BImageUrl");
                model.ViewCount = dr.Field<int>("ViewCount");
                model.Title = dr.Field<string>("Title");
                model.PublishDateTime = dr.Field<DateTime>("PublishDateTime");
                model.IsDeleted = dr.Field<bool>("IsDeleted");
            }
            return model;
        }
        #endregion

        #region
        public static void UpdateViewCount(int id) {
            string strSQL = "UPDATE Products SET ViewCount = ViewCount + 1 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id", id);

            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        #endregion
    }
}
