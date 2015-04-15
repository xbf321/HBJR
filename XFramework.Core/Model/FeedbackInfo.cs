using System;

namespace XFramework.Model
{
    public class FeedbackInfo
    {
        public int Id { get; set; }
        public string Realname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string FeedbackType { get; set; }
        public string IP { get; set; }
    }
}
