using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
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

        /// <summary>
        /// 用户管理（机构版）
        /// </summary>
        public IActionResult UserManager()
        {
            return View();
        }

        /// <summary>
        /// 分页用户列表
        /// </summary>
        public JsonResult GetXFundUserListToPage(xfund_t_sys_userSelObj sel)
        {
            int total = 0;
            List<xfund_t_sys_userExObj> users = null;
            if (sel.page == null)
            {
                sel.page = 1;
            }
            if (sel.rows == null)
            {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.trumgu_bi_db_connstr);
            MySqlParameter[] pms = new MySqlParameter[4];
            string sql_where = "";
            string sql_total = "SELECT id FROM fund.t_sys_user WHERE 1=1 ";
            string sql_list = "SELECT id,name,userid, "
                            + " password,status,lastlogin, "
                            + " loginip,loginsum,islogin, "
                            + " color,macaddr,expiretime, "
                            + " person_liable,telephone,company_name, "
                            + " hpcompany_id,parents_id,customertype_name, "
                            + " create_time,create_user_name,create_user_id, "
                            + " is_person_liable,person_liable_id,is_pay, "
                            + " mailbox,department,iscompany_show, "
                            + " (SELECT COUNT(0) FROM trumgu_bi_db.t_xfund_user_login_info WHERE login_name=userid AND login_time > DATE_ADD(CURDATE(),INTERVAL -14 DAY)) as 'sum_week_login_num', "
                            + " (SELECT COUNT(0) FROM trumgu_bi_db.t_xfund_user_login_info WHERE login_name=userid AND login_time > DATE_ADD(CURDATE(),INTERVAL -37 DAY)) as 'sum_year_login_num', "
                            + " (SELECT COUNT(0) FROM trumgu_bi_db.t_xfund_user_login_info WHERE login_name=userid) as 'sum_history_login_num' "
                            + " FROM fund.t_sys_user WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace(sel.name_like))
            {
                pms[0] = new MySqlParameter("@name_like", MySqlDbType.VarChar) { Value = "%" + sel.name_like + "%" };
                sql_where += " AND name LIKE @name_like";
            }
            if (!string.IsNullOrWhiteSpace(sel.company_name_like))
            {
                pms[1] = new MySqlParameter("@company_name_like", MySqlDbType.VarChar) { Value = "%" + sel.company_name_like + "%" };
                sql_where += " AND company_name LIKE @company_name_like";
            }
            if (!string.IsNullOrWhiteSpace(sel.person_liable_like))
            {
                pms[2] = new MySqlParameter("@person_liable_like", MySqlDbType.VarChar) { Value = "%" + sel.person_liable_like + "%" };
                sql_where += " AND person_liable LIKE @person_liable_like";
            }
            if (!string.IsNullOrWhiteSpace(sel.userid_like))
            {
                pms[3] = new MySqlParameter("@userid_like", MySqlDbType.VarChar) { Value = "%" + sel.userid_like + "%" };
                sql_where += " AND userid LIKE @userid_like";
            }

            sql_total += sql_where;
            sql_list += sql_where + " ORDER BY id DESC LIMIT " + ((sel.page - 1) * sel.rows) + "," + sel.rows;

            total = db.xfund_t_sys_user.FromSql(sql_total, pms).Count();
            if (total > 0)
            {
                users = db.xfund_t_sys_user.FromSql(sql_list, pms).ToList();
            }



            db.Dispose();
            if (users == null)
            {
                users = new List<xfund_t_sys_userExObj>();
            }
            return Json(new { total = total, rows = users });
        }

        /// <summary>
        /// 私募公司尽调上传页面
        /// </summary>
        public IActionResult PrivateCompanyInvestigation()
        {
            return View();
        }

        /// <summary>
        /// 分页获取私募公司尽调上传
        /// </summary>
        public JsonResult GetPrivateCompanyInvestigationToPage(xfund_t_fund_companySelObj sel)
        {
            int total = 0;
            List<xfund_t_fund_companyExObj> company = null;
            if (sel.page == null)
            {
                sel.page = 1;
            }
            if (sel.rows == null)
            {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            total = (from t1 in db.xfund_t_fund_company
                     join t2 in db.xfund_t_due_jdxx
                     on t1.regis_code equals t2.gsbh into t3
                     from t4 in t3.DefaultIfEmpty()
                     where t1.type_of_funds == "证券投资基金" && (!string.IsNullOrWhiteSpace(sel.cn_name_like) ? t1.cn_name.Contains(sel.cn_name_like) : true)
                     && (sel.isIntroduction != null ? (sel.isIntroduction == true ? !string.IsNullOrWhiteSpace(t4.gsjs) : string.IsNullOrWhiteSpace(t4.gsjs)) : true)
                     && (sel.isInvestigation != null ? (sel.isInvestigation == true ? !string.IsNullOrWhiteSpace(t4.jdjl) : string.IsNullOrWhiteSpace(t4.jdjl)) : true)
                     select new xfund_t_fund_companyExObj()
                     {
                         cn_name = t1.cn_name,
                         gsjs = t4.gsjs,
                         regis_code = t1.regis_code,
                         jdjl = t4.jdjl
                     }).Count();
            if (total > 0)
            {
                company = (from t1 in db.xfund_t_fund_company
                           join t2 in db.xfund_t_due_jdxx
                           on t1.regis_code equals t2.gsbh into t3
                           from t4 in t3.DefaultIfEmpty()
                           where t1.type_of_funds == "证券投资基金" && (!string.IsNullOrWhiteSpace(sel.cn_name_like) ? t1.cn_name.Contains(sel.cn_name_like) : true)
                           && (sel.isIntroduction != null ? (sel.isIntroduction == true ? !string.IsNullOrWhiteSpace(t4.gsjs) : string.IsNullOrWhiteSpace(t4.gsjs)) : true)
                     && (sel.isInvestigation != null ? (sel.isInvestigation == true ? !string.IsNullOrWhiteSpace(t4.jdjl) : string.IsNullOrWhiteSpace(t4.jdjl)) : true)
                           select new xfund_t_fund_companyExObj()
                           {
                               cn_name = t1.cn_name,
                               gsjs = t4.gsjs,
                               regis_code = t1.regis_code,
                               jdjl = t4.jdjl,
                               jdcs = t4.jdcs
                           })
                    .Skip(sel.page != 1 ? ((int)sel.page - 1) * (int)sel.rows : 0)
                    .Take((int)sel.rows)
                    .ToList();
            }
            db.Dispose();
            if (company == null)
            {
                company = new List<xfund_t_fund_companyExObj>();
            }
            return Json(new { total = total, rows = company });
        }

        /// <summary>
        /// 根据私募公司注册编号清空公司介绍附件
        /// </summary>
        [HttpPost]
        public JsonResult CleanPrivateCompanyIntroduce(string regis_code)
        {
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            if (!string.IsNullOrWhiteSpace(regis_code))
            {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                List<xfund_t_due_jdxxObj> list = db.xfund_t_due_jdxx.Where(rec => rec.gsbh == regis_code).ToList();
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].gsjs = "";
                    }
                    db.xfund_t_due_jdxx.UpdateRange(list);
                    if (db.SaveChanges() > 0)
                    {
                        ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                        ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                    }
                    else
                    {
                        ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_SAVE;
                        ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString();
                    }
                }
                else
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                }
                db.Dispose();
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }
            return Json(ro);
        }

        /// <summary>
        /// 根据私募公司注册编号清空公司尽调附件
        /// </summary>
        [HttpPost]
        public JsonResult CleanPrivateCompanyInvestigation(string regis_code)
        {
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            if (!string.IsNullOrWhiteSpace(regis_code))
            {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                List<xfund_t_due_jdxxObj> list = db.xfund_t_due_jdxx.Where(rec => rec.gsbh == regis_code).ToList();
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].jdjl = "";
                        list[i].jdcs = 0;
                    }
                    db.xfund_t_due_jdxx.UpdateRange(list);
                    if (db.SaveChanges() > 0)
                    {
                        ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                        ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                    }
                    else
                    {
                        ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_SAVE;
                        ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString();
                    }
                }
                else
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                }
                db.Dispose();
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }
            return Json(ro);
        }

        /// <summary>
        /// 添加私募公司公司介绍
        /// </summary>
        [HttpPost]
        public JsonResult AddIntroduce(string regis_code, List<FileInfoObj> files)
        {
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            if (!string.IsNullOrWhiteSpace(regis_code) && files != null && files.Count == 1)
            {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                List<xfund_t_due_jdxxObj> list = db.xfund_t_due_jdxx.Where(rec => rec.gsbh == regis_code).ToList();
                if (list == null || list.Count == 0)
                {
                    xfund_t_due_jdxxObj m = new xfund_t_due_jdxxObj();
                    m.gsbh = regis_code;
                    m.gsjs = files[0].fileUrl;
                }
                else if (list.Count == 1)
                {
                    list[0].gsjs = files[0].fileUrl;
                    db.UpdateRange(list[0]);
                }
                else
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_FORMAT;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_FORMAT.ToString();
                    ro.data = "尽调信息不唯一！";
                }

                if (db.SaveChanges() > 0)
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                }
                else
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_SAVE;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString();
                }

                db.Dispose();
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }
            return Json(ro);
        }

        /// <summary>
        /// 添加私募公司尽调记录
        /// </summary>
        [HttpPost]
        public JsonResult AddInvestigation(string regis_code, List<FileInfoObj> files)
        {
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            if (!string.IsNullOrWhiteSpace(regis_code) && files != null && files.Count == 1)
            {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                List<xfund_t_due_jdxxObj> list = db.xfund_t_due_jdxx.Where(rec => rec.gsbh == regis_code).ToList();
                if (list == null || list.Count == 0)
                {
                    xfund_t_due_jdxxObj m = new xfund_t_due_jdxxObj();
                    m.gsbh = regis_code;
                    m.jdjl = files[0].fileUrl;
                    m.jdcs = 1;
                }
                else if (list.Count == 1)
                {
                    list[0].jdjl = files[0].fileUrl;
                    list[0].jdcs += 1;
                    db.UpdateRange(list[0]);
                }
                else
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_FORMAT;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_FORMAT.ToString();
                    ro.data = "尽调信息不唯一！";
                }

                if (db.SaveChanges() > 0)
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                }
                else
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_SAVE;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString();
                }

                db.Dispose();
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }
            return Json(ro);
        }
    }
}