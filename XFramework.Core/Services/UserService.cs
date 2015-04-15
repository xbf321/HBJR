using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFramework.Model;
using XFramework.Data;

namespace XFramework.Services
{
    public static class UserService
    {
        public static UserInfo Get(string userName) {
            return UserManage.Get(userName);
        }

        public static UserInfo Get(int userId) {
            return UserManage.Get(userId);
        }
        public static IList<UserInfo> List() {
            return UserManage.List();
        }
        public static UserInfo Create(UserInfo model) {
            if (model.Id == 0)
            {
                model.Id = UserManage.Add(model);
            }
            else {
                UserManage.Update(model);
            }
            return model;
        }

    }
}
