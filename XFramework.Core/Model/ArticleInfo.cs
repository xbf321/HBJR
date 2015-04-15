using System;

namespace XFramework.Model
{
    public class ArticleInfo
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string LinkUrl { get; set; }
        public string Tags { get; set; }
        public int Sort { get; set; }
        public bool IsTop { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Timespan { get; set; }        
        public Guid GUID { get; set; }

        /// <summary>
        /// 扩展属性，自动填充
        /// </summary>
        public string Url { get; set; }

        public ArticleInfo() {
            ImageUrl = "###";
            Timespan = DateTime.Now.ToString("HHmmssfff");
            PublishDateTime = CreateDateTime = DateTime.Now;
            Sort = 999999;
        }
    }
}
