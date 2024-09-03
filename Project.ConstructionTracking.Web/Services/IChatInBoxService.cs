using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MCompanyModel;
using static Project.ConstructionTracking.Web.Models.ChatInBoxModel;

namespace Project.ConstructionTracking.Web.Services
{
    public interface IChatInBoxService
    {
        List<ChatInBoxModel.GetlistChatInBox> GetListChatInBox(ChatInBoxModel.GetlistChatInBox filterData);
        insertInBox InsertUnitFormInbox(insertInBox En);
    }
}
