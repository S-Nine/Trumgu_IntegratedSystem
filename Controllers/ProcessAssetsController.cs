using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;
using Trumgu_IntegratedManageSystem.Utils;

namespace Trumgu_IntegratedManageSystem.Controllers {
    public class ProcessAssetsController : Controller {
        /// <summary>
        /// 公告文档
        /// </summary>
        public IActionResult NoticeDoc () {
            return View ();
        }

        /// <summary>
        /// 分页查询公告文档
        /// </summary>
        [HttpPost]
        public JsonResult GetNoticeToPage (t_assets_noticeSelObj sel) {
            int total = 0;
            List<t_assets_noticeObj> users = null;
            if (sel.page == null) {
                sel.page = 1;
            }
            if (sel.rows == null) {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            total = db.t_assets_notice.Where (rec => rec.is_delete == 0 &&
                    (sel != null && !string.IsNullOrWhiteSpace (sel.title_like) ? rec.title.Contains (sel.title_like) : true))
                .Count ();
            if (total > 0) {
                users = db.t_assets_notice.Where (rec => rec.is_delete == 0 &&
                        (sel != null && !string.IsNullOrWhiteSpace (sel.title_like) ? rec.title.Contains (sel.title_like) : true))
                    .OrderByDescending (rec => new { rec.is_settop, rec.id })
                    .Skip (sel.page != 1 ? ((int) sel.page - 1) * (int) sel.rows : 0)
                    .Take ((int) sel.rows)
                    .ToList ();
            }
            db.Dispose ();
            if (users == null) {
                users = new List<t_assets_noticeObj> ();
            }
            return Json (new { total = total, rows = users });
        }
    }
}