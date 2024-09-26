using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Services;
using System.Text.RegularExpressions;
using static Project.ConstructionTracking.Web.Models.ChatInBoxModel;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class ChatInBoxController : BaseController
    {
        private readonly IChatInBoxService _IChatInBoxService;

        public ChatInBoxController(IChatInBoxService ChatInBoxService)
        {
            _IChatInBoxService = ChatInBoxService;
        }

        [HttpPost]
        public JsonResult Getchatlist(Guid UnitFormID , int FormID , int RoleID)
        {
            try
            {
                var filterData = new GetlistChatInBox
                {
                    UnitFormID = UnitFormID,
                    FormID = FormID,
                };

                List<ChatInBoxModel.GetlistChatInBox> listChatInBox = _IChatInBoxService.GetListChatInBox(filterData);
                return Json(new { success = true , data = listChatInBox });  
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "โหลดข้อมูลไม่สำเร็จ" });
            }
        }

        [HttpPost]
        public JsonResult InsertChatInbox(int FormID, int RoleID, string TextInbox, Guid UnitFormID, Guid UserID)
        {
            try
            {
                var En = new insertInBox
                {
                    UnitFormID = UnitFormID,
                    TextInbox = TextInbox,
                    FormID = FormID,
                    RoleID = RoleID,
                    UserID = UserID,
                };

                _IChatInBoxService.InsertUnitFormInbox(En);
                return Json(new { success = true, message = "บันทึกสำเร็จ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "บันทึกข้อมูลไม่สำเร็จ: " + ex.Message });
            }
        }

    }
}
