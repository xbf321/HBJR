using System;

namespace XFramework.Model
{
    public class AttachmentInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Size { get; set; }
        public string FileType { get; set; }
        public string Url { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public AttachmentInfo() {
            Size = 0;
            FileType = "未知";
        }
    }
}
