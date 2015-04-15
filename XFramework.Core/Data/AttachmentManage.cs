using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFramework.Model;
using XFramework.Common;
using System.Data;
using System.Data.SqlClient;

namespace XFramework.Data
{
    public static class AttachmentManage
    {
        public static int Add(AttachmentInfo model) {
            string strSQL = "INSERT INTO Attachments(Title,Size,FileType,Url,CreateDateTime) VALUES(@Title,@size,@FileType,@Url,GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = { 
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Size",SqlDbType.Int),
                                    new SqlParameter("FileType",SqlDbType.NVarChar),
                                    new SqlParameter("Url",SqlDbType.NVarChar),
                                   };
            parms[0].Value = model.Title;
            parms[1].Value = model.Size;
            parms[2].Value = model.FileType;
            parms[3].Value = model.Url;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        public static void Edit(AttachmentInfo model) {
            string strSQL = "UPDATE Attachments SET Title = @Title,Size = @Size,FileType = @FileType,Url = @Url,IsDeleted = @IsDeleted WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Size",SqlDbType.Int),
                                    new SqlParameter("FileType",SqlDbType.NVarChar),
                                    new SqlParameter("Url",SqlDbType.NVarChar),
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("IsDeleted",SqlDbType.Int)
                                   };
            parms[0].Value = model.Title;
            parms[1].Value = model.Size;
            parms[2].Value = model.FileType;
            parms[3].Value = model.Url;
            parms[4].Value = model.Id;
            parms[5].Value = model.IsDeleted ? 1 : 0;
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        public static AttachmentInfo Get(int id) {
            string strSQL = "SELECT * FROM Attachments WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);
            DataRow dr = Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm);
            AttachmentInfo model = new AttachmentInfo();
            if(dr != null){
                model = new AttachmentInfo()
                {
                    Id = dr.Field<int>("ID"),
                    Title = dr.Field<string>("Title"),
                    CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                    FileType = dr.Field<string>("FileType"),
                    Size = dr.Field<int>("Size"),
                    Url = dr.Field<string>("Url"),
                    IsDeleted = dr.Field<bool>("IsDeleted")
                };
            }
            return model;
        }
        public static IPageOfList<AttachmentInfo> List(SearchSetting setting)
        {

            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Attachments";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.WithOptions = " WITH(NOLOCK)";
            fp.OverOrderBy = " CreateDateTime DESC";
            if(!setting.ShowDeleted){
                fp.Condition += " IsDeleted = 0";
            }
            IList<AttachmentInfo> list = new List<AttachmentInfo>();
            AttachmentInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = new AttachmentInfo()
                    {
                        Id = dr.Field<int>("ID"),
                        Title = dr.Field<string>("Title"),
                        CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                        FileType = dr.Field<string>("FileType"),
                        Size = dr.Field<int>("Size"),
                        Url = dr.Field<string>("Url"),
                        IsDeleted = dr.Field<bool>("IsDeleted")
                    };
                    list.Add(model);
                }
            }
            int count = Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, "SELECT COUNT(*) FROM Attachments"));
            return new PageOfList<AttachmentInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
    }
}
