﻿using Microsoft.AspNetCore.Mvc;

namespace Project.ConstructionTracking.Web.Controllers
{
    public class QCChecklistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
