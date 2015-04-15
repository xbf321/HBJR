using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFramework.Model;
using XFramework.Data;

namespace XFramework.Services
{
    public class FeedbackService
    {
        public static int Post(FeedbackInfo model) {
            return FeedbackManage.Add(model);
        }
        public static IPageOfList<FeedbackInfo> List(int pageIndex, int pageSize) {
            return FeedbackManage.List(pageIndex,pageSize);
        }
    }
}
