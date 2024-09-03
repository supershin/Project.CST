using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MCompanyModel;
using static Project.ConstructionTracking.Web.Models.ChatInBoxModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public interface IChatInBoxRepo
    {
        List<ChatInBoxModel.GetlistChatInBox> GetListChatInBox(ChatInBoxModel.GetlistChatInBox filterData);
        void InsertUnitFormInbox(ChatInBoxModel.insertInBox En);
    }
}
