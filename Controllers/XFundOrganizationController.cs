using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trumgu_IntegratedManageSystem.Attributes;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;
using Trumgu_IntegratedManageSystem.Models.xfund;
using Trumgu_IntegratedManageSystem.Utils;

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class XFundOrganizationController : Controller
    {
        /// <summary>
        /// 用户登录统计（机构版）
        /// </summary>
        public IActionResult UserLoginStatistics()
        {
            return View();
        }

        /// <summary>
        /// 分页获取用户登录统计
        /// </summary>
        public JsonResult GetXFundUserToPage(t_xfund_user_login_infoSelObj sel)
        {
            int total = 0;
            List<t_xfund_user_login_infoExObj> users = null;
            if (sel.page == null)
            {
                sel.page = 1;
            }
            if (sel.rows == null)
            {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.trumgu_bi_db_connstr);
            total = db.t_xfund_user_login_info.Where(rec =>
                   (sel != null
                   && (!string.IsNullOrWhiteSpace(sel.user_name_like) ? rec.user_name.Contains(sel.user_name_like) : true)
                   && (!string.IsNullOrWhiteSpace(sel.login_name_like) ? rec.login_name.Contains(sel.login_name_like) : true)
                   && (sel.login_time_min != null ? rec.login_time >= sel.login_time_min : true)
                   && (sel.login_time_max != null ? rec.login_time <= sel.login_time_max : true)))
                  .GroupBy(rec => rec.login_name.Trim())
                  .Count();
            if (total > 0)
            {
                users = db.t_xfund_user_login_info.Where(rec => (sel != null
                   && (!string.IsNullOrWhiteSpace(sel.user_name_like) ? rec.user_name.Contains(sel.user_name_like) : true)
                   && (!string.IsNullOrWhiteSpace(sel.login_name_like) ? rec.login_name.Contains(sel.login_name_like) : true)
                   && (sel.login_time_min != null ? rec.login_time >= sel.login_time_min : true)
                   && (sel.login_time_max != null ? rec.login_time <= sel.login_time_max : true)))
                    .GroupBy(rec => rec.login_name.Trim())
                    .Select(s => new t_xfund_user_login_infoExObj()
                    {
                        id = null,
                        user_name = s.Max(rec => rec.user_name),
                        person_liable = s.Max(rec => rec.person_liable),
                        login_time = null,
                        login_name = s.Key,
                        token = null,
                        call_num = s.Sum(rec => rec.call_num),
                        create_date = null,
                        sum_login_num = s.Count(),
                        login_time_min = s.Min(rec => rec.login_time),
                        login_time_max = s.Max(rec => rec.login_time)
                    })
                    .OrderByDescending(rec => rec.sum_login_num)
                    .Skip(sel.page != 1 ? ((int)sel.page - 1) * (int)sel.rows : 0)
                    .Take((int)sel.rows)
                    .ToList();
            }
            db.Dispose();
            if (users == null)
            {
                users = new List<t_xfund_user_login_infoExObj>();
            }
            return Json(new { total = total, rows = users });
        }

        /// <summary>
        /// 根据登录账号查询月份登录信息
        /// </summary>
        [HttpPost]
        public JsonResult GetXFundUserLoginDetailToList(string login_name, DateTime? login_time_min, DateTime? login_time_max)
        {
            List<t_xfund_user_login_infoExObj> list = null;
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.trumgu_bi_db_connstr);
            list = db.t_xfund_user_login_info.Where(rec => rec.login_name == login_name
                && (login_time_min != null ? rec.login_time >= login_time_min : true)
                && (login_time_max != null ? rec.login_time <= login_time_max : true))
                .GroupBy(rec => rec.login_time != null ? ((DateTime)rec.login_time).ToString("yyyy-MM") : null)
                .Select(s => new t_xfund_user_login_infoExObj()
                {
                    year_month = s.Key,
                    sum_login_num = s.Count(),
                    login_time_min = s.Min(rec => rec.login_time),
                    call_num = s.Sum(rec => rec.call_num)
                })
                .OrderBy(rec => rec.login_time_min)
                .ToList();
            if (list == null)
            {
                list = new List<t_xfund_user_login_infoExObj>();
            }
            return Json(list);
        }

        /// <summary>
        /// 根据登录账号菜单使用信息
        /// </summary>
        [HttpPost]
        public JsonResult GetXFundUserMenuDetailToList(string login_name, DateTime? call_time_min, DateTime? call_time_max)
        {
            List<t_xfund_user_call_logExObj> list = null;
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.trumgu_bi_db_connstr);
            list = db.t_xfund_user_call_log.Where(rec => rec.operation_code == "RecordLog" && rec.login_name == login_name
                && (call_time_min != null ? rec.call_time >= call_time_min : true)
                && (call_time_max != null ? rec.call_time <= call_time_max : true))
                .GroupBy(rec => rec.menu_name)
                .Select(s => new t_xfund_user_call_logExObj()
                {
                    menu_name = s.Key,
                    sum_login_num = s.Count(),
                })
                .ToList();
            if (list == null)
            {
                list = new List<t_xfund_user_call_logExObj>();
            }
            return Json(list);
        }

        /// <summary>
        /// 根据登录账号功能使用信息
        /// </summary>
        [HttpPost]
        public JsonResult GetXFundUserFunDetailToList(string login_name, DateTime? call_time_min, DateTime? call_time_max)
        {
            List<OperationStatisticsObj> list = null;
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.trumgu_bi_db_connstr);
            var query1 = db.OperationStatistics.FromSql("SELECT A.id,A.operation_code,B.operation_desc ,A.sum_login_num "
                        + " FROM ( "
                            + " SELECT  MIN(a.id) as 'id',operation_code,page_title,COUNT(0) AS 'sum_login_num',B.menu "
                            + " FROM trumgu_bi_db.t_xfund_user_call_log A "
                            + " INNER JOIN fund.t_sys_menu B "
                            + " ON A.page_title = B.`code` "
                            + " WHERE operation_code <> 'RecordLog' "
                            + " AND A.login_name = '" + login_name + "' "
                            + (call_time_min != null ? " AND A.call_time >= '" + ((DateTime)call_time_min).ToString("yyyy-MM-dd HH:mm:ss.ffff") + "'" : "")
                            + (call_time_max != null ? " AND A.call_time <= '" + ((DateTime)call_time_max).ToString("yyyy-MM-dd HH:mm:ss.ffff") + "'" : "")
                            + " GROUP BY operation_code "
                        + " ) A "
                        + " INNER JOIN trumgu_bi_db.t_xfund_sys_service_info B "
                        + " ON A.operation_code = B.operation_code "
                        + " ORDER BY A.sum_login_num DESC;");
            list = query1.ToList();
            if (list == null)
            {
                list = new List<OperationStatisticsObj>();
            }
            return Json(list);
        }
    }
}