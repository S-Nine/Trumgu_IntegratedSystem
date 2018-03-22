using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class SystemController : Controller
    {
        public IActionResult Menu()
        {
            return View();
        }
    }
}