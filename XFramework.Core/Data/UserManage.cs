using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using XFramework.Model;

namespace XFramework.Data
{
    public static class UserManage
    {
        public static bool Validate(string userName,string userPwd) {
            return false;
        }
        public static UserInfo Get(string userName) {
            string strSQL = "SELECT TOP(1) * FROM Users WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter parm = new SqlParameter("UserName", userName);
            DataRow dr = Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm);
            return Get(dr);
        }
        public static UserInfo Get(int userId) {
            string strSQL = "SELECT TOP(1) * FROM Users WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",userId);
            DataRow dr = Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm);
            return Get(dr);
        }
        public static IList<UserInfo> List() {
            string strSQL = "SELECT * FROM Users WITH(NOLOCK) ORDER BY CreateDateTime DESC";
            IList<UserInfo> list = new List<UserInfo>();
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(Get(dr));
                }
            }
            return list;
        }
        private static UserInfo Get(DataRow dr) {
            UserInfo model = new UserInfo();
            if(dr!= null){
                model.Id = dr.Field<int>("Id");
                model.UserPwd = dr.Field<string>("UserPwd");
                model.UserName = dr.Field<string>("UserName");
                model.CreateDateTime = dr.Field<DateTime>("CreateDateTime");
            }
            return model;
        }
        public static int Add(UserInfo model) {
            string strSQL = "INSERT INTO Users(UserName,UserPwd,CreateDateTime) VALUES(@UserName,@UserPwd,GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = { 
                                    new SqlParameter("UserName",SqlDbType.VarChar),
                                    new SqlParameter("UserPwd",SqlDbType.VarChar)
                                   };
            parms[0].Value = model.UserName;
            parms[1].Value = model.UserPwd;

            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        public static void Update(UserInfo model) {
            string strSQL = "UPDATE Users SET UserPwd = @UserPwd WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("UserPwd",SqlDbType.VarChar)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.UserPwd;
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
    }
}
