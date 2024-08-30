using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Models.MUserModel;
using Project.ConstructionTracking.Web.Services;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IMasterUserService _masterUserService;

        public UserController(IMasterUserService masterUserService)
        {
            _masterUserService = masterUserService;
        }

        public IActionResult Detail(string param)
        {
            string decode = HashExtension.DecodeFrom64(param);
            Guid userID = Guid.Parse(decode);

            DetailUserResp detailResp = _masterUserService.DetailUser(userID);
            detailResp.respModel = _masterUserService.GetUnitResp();

            return View(detailResp);
        }
    }
}

