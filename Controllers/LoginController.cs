using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trumgu_IntegratedManageSystem.Utils;

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            DBHelper db = new DBHelper();
            return View();
        }
    }
}