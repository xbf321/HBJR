using XFramework.Model;
using XFramework.Data;

namespace XFramework.Services
{
    public static class AttachmentService
    {
        public static AttachmentInfo Create(AttachmentInfo model) {
            if (model.Id == 0)
            {
                model.Id = AttachmentManage.Add(model);
            }
            else {
                AttachmentManage.Edit(model);
            }
            return model;
        }
        public static AttachmentInfo Get(int id) {
            return AttachmentManage.Get(id);
        }
        public static IPageOfList<AttachmentInfo> List(SearchSetting setting)
        {
            return AttachmentManage.List(setting);
        }
    }
}
