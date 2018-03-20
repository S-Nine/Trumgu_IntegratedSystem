using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;

namespace Trumgu_IntegratedManageSystem.Controllers {
    public class HomeController : Controller {
        public IActionResult Index () {
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            var i = db.t_sys_menu.ToList();
            return View ();
        }
    }
}