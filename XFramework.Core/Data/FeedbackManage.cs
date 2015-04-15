using System;
using System.Data;
using System.Data.SqlClient;

using XFramework.Model;
using XFramework.Common;
using System.Collections.Generic;

namespace XFramework.Data
{
    public static class FeedbackManage
    {
        /// <summary>
        /// 添加在线留言
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add(FeedbackInfo model) {
            string strSQL = "INSERT INTO Feedbacks(Realname,Email,Phone,Title,Content,CreateDateTime,IP,FeedbackType) VALUES(@RealName,@Email,@Phone,@title,@Content,GETDATE(),@IP,@FeedbackType);SELECT @@IDENTITY;";
            SqlParameter[] parms = { 
                                    new SqlParameter("Realname",SqlDbType.NVarChar,10),
                                    new SqlParameter("Email",SqlDbType.NVarChar,50),
                                    new SqlParameter("Phone",SqlDbType.NVarChar,50),
                                    new SqlParameter("Title",SqlDbType.NVarChar,50),
                                    new SqlParameter("Content",SqlDbType.NVarChar,200),
                                    new SqlParameter("IP",SqlDbType.NVarChar),
                                    new SqlParameter("FeedbackType",SqlDbType.NVarChar)
                                   };
            parms[0].Value = model.Realname == null ? string.Empty : model.Realname;
            parms[1].Value = model.Email ==null ? string.Empty:model.Email;
            parms[2].Value = model.Phone == null ? string.Empty : model.Phone;
            parms[3].Value = model.Title == null ? string.Empty : model.Title;
            parms[4].Value = model.Content== null ? string.Empty :model.Content;
            parms[5].Value = model.IP == null ? string.Empty:model.IP;
            parms[6].Value = model.FeedbackType == null ? string.Empty : model.FeedbackType;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms)) ;
        }
        public static IPageOfList<FeedbackInfo> List(int pageIndex, int pageSize)
        {

            FastPaging fp = new FastPaging();
            fp.PageIndex = pageIndex;
            fp.PageSize = pageSize;
            fp.Ascending = false;
            fp.TableName = "Feedbacks";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = " CreateDateTime DESC";
            IList<FeedbackInfo> list = new List<FeedbackInfo>();
            FeedbackInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = new FeedbackInfo() { 
                        Id = dr.Field<int>("ID"),
                        Title = dr.Field<string>("Title"),
                        Content = dr.Field<string>("Content"),
                        Phone = dr.Field<string>("Phone"),
                        Email = dr.Field<string>("Email"),
                        CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                        IP = dr.Field<string>("IP"),
                        Realname = dr.Field<string>("Realname"),
                        FeedbackType = dr.Field<string>("FeedbackType")
                    };
                    list.Add(model);
                }
            }
            int count = Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,"SELECT COUNT(*) FROM Feedbacks"));
            return new PageOfList<FeedbackInfo>(list, pageIndex, pageSize, count);
        }
    }
}
