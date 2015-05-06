using System;

namespace XFramework.Model
{
    public class ProductInfo
    {
        public int Id { get; set; }
        public Guid GUID { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SImageUrl { get; set; }
        public string BImageUrl { get; set; }
        public int ViewCount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }

        //扩展字段
        public string LinkUrl { get; set; }

        public ProductInfo() {
            SImageUrl = "###";
            BImageUrl = "###";
            PublishDateTime = CreateDateTime = DateTime.Now;
        }
    }
}
