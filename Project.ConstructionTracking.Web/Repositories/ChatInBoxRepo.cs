using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using static Project.ConstructionTracking.Web.Models.ChatInBoxModel;

namespace Project.ConstructionTracking.Web.Repositories
{
    public class ChatInBoxRepo : IChatInBoxRepo
    {
        private readonly ContructionTrackingDbContext _context;

        public ChatInBoxRepo(ContructionTrackingDbContext context)
        {
            _context = context;
        }

        public List<ChatInBoxModel.GetlistChatInBox> GetListChatInBox(ChatInBoxModel.GetlistChatInBox filterData)
        {
            var vendorQuery = from t1 in _context.tr_UnitFormInbox
                              join t2 in _context.tm_User on new { t1.ActionBy } equals new { ActionBy = (Guid?)t2.ID } into t2Joins
                              from t2Join in t2Joins.DefaultIfEmpty()
                              join t3 in _context.tr_UnitForm on new { ID = t1.UnitFormID } equals new { t3.ID } into t3Joins
                              from t3Join in t3Joins.DefaultIfEmpty()
                              join t4 in _context.tm_Unit on new { t3Join.UnitID } equals new { UnitID = (Guid?)t4.UnitID } into t4Joins
                              from t4Join in t4Joins.DefaultIfEmpty()
                              where t1.FlagActive == true
                                 && t1.UnitFormID == filterData.UnitFormID
                                 && t1.FormID == filterData.FormID
                                 && t1.RoleID == filterData.RoleID

                              orderby t1.ID
                              select new GetlistChatInBox
                              {
                                  UserID = t1.ActionBy,
                                  UserName = t2Join.FirstName + " " + t2Join.LastName
                              };

            return vendorQuery.ToList();
        }
    }
}
