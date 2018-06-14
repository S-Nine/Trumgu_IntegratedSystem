using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;
using Trumgu_IntegratedManageSystem.Utils;

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class WorkAssistantController : Controller
    {
        /// <summary>
        /// 密码薄页面初始化
        /// <summary>
        /// <returns></returns>
        public IActionResult CipherThin()
        {
            return View();
        }

        /// <summary>
        /// 分页查询密码薄
        /// </summary>
        [HttpPost]
        public JsonResult GetCipherThinToPage(t_assets_cipher_thinSelObj sel)
        {
            int total = 0;
            t_sys_userObj user = null;
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            List<t_assets_cipher_thinObj> cipher = null;
            if (sel.page == null)
            {
                sel.page = 1;
            }
            if (sel.rows == null)
            {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            total = db.t_assets_cipher_thin.Where(rec => rec.is_delete == 0 && rec.user_id == user.id &&
                   (sel != null && !string.IsNullOrWhiteSpace(sel.title_like) ? rec.title.Contains(sel.title_like) : true))
                .Count();
            if (total > 0)
            {
                cipher = db.t_assets_cipher_thin.Where(rec => rec.is_delete == 0 && rec.user_id == user.id &&
                       (sel != null && !string.IsNullOrWhiteSpace(sel.title_like) ? rec.title.Contains(sel.title_like) : true))
                    .OrderByDescending(rec => rec.id)
                    .Skip(sel.page != 1 ? ((int)sel.page - 1) * (int)sel.rows : 0)
                    .Take((int)sel.rows)
                    .ToList();
            }
            db.Dispose();
            if (cipher == null)
            {
                cipher = new List<t_assets_cipher_thinObj>();
            }
            return Json(new { total = total, rows = cipher });
        }
    }
}