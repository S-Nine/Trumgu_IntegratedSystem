using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trumgu_IntegratedManageSystem.Attributes;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// 未经授权
        /// </summary>

        public IActionResult Error401()
        {
            return View();
        }

        /// <summary>
        /// 页面丢失
        /// </summary>

        public IActionResult ErrorJump()
        {
            int statusCode = Response.StatusCode;
            if (statusCode == 404)
            {
                Response.Redirect("/Error/Error404", true);
            }
            else if (statusCode == 500)
            {
                Response.Redirect("/Error/Error500", true);
            }
            else
            {
                Response.Redirect("/Error/Error401", true);
            }
            return View();
        }

        /// <summary>
        /// 服务器内部错误
        /// </summary>
        public IActionResult Error404()
        {
            return View();
        }

        /// <summary>
        /// 服务器内部错误
        /// </summary>
        public IActionResult Error500()
        {
            return View();
        }
    }
}