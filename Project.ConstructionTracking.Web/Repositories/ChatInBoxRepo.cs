using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using System;
using System.Globalization;
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
                              join t5 in _context.tm_Form on new { t3Join.FormID } equals new { FormID = (int?)t5.ID } into t5Joins
                              from t5Join in t5Joins.DefaultIfEmpty()
                              join t6 in _context.tm_Role on new {t1.RoleID } equals new { RoleID = (int?)t6.ID } into t6Joins
                              from t6Join in t6Joins.DefaultIfEmpty()
                              where t1.FlagActive == true
                                 && t1.UnitFormID == filterData.UnitFormID
                                 && t1.FormID == filterData.FormID
                              orderby t1.ID
                              select new GetlistChatInBox
                              {
                                  UnitFormID = t3Join.ID,
                                  UnitCode = t4Join.UnitCode,
                                  FormID = t5Join.ID,
                                  FormName = t5Join.Name,
                                  UserID = t1.ActionBy,
                                  RoleName = t6Join.Name,
                                  UserName = t2Join.FirstName + " " + t2Join.LastName,
                                  TextInbox = t1.TextInbox,
                                  Actiondate = t1.ActionDate.ToStringDateTime()
                              };

            return vendorQuery.ToList();
        }

        public void InsertUnitFormInbox(ChatInBoxModel.insertInBox En)
        {
            var insertFormInbox = new tr_UnitFormInbox
            {
                UnitFormID = En.UnitFormID,
                FormID = En.FormID,
                RoleID = En.RoleID,
                TextInbox = En.TextInbox,
                ActionDate = DateTime.Now,
                FlagActive = true,
                ActionBy = En.UserID,
                CreateDate = DateTime.Now,
                CraeteBy = En.UserID,
                UpdateDate = DateTime.Now,
                UpdateBy = En.UserID
            };

            _context.tr_UnitFormInbox.Add(insertFormInbox);
            _context.SaveChanges();
        }

    }
}
