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

namespace Trumgu_IntegratedManageSystem.Controllers {
    public class PFXFundOrganizationController : Controller {
        /// <summary>
        /// 用户管理（私募版）
        /// </summary>
        public IActionResult UserManager () {
            return View ();
        }

        /// <summary>
        /// 分页用户列表
        /// </summary>
        public JsonResult GetPFXFundUserListToPage (xfund_t_pf_sys_userSelObj sel) {
            int total = 0;
            List<xfund_t_pf_sys_userExObj> users = null;
            if (sel.page == null) {
                sel.page = 1;
            }
            if (sel.rows == null) {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext (ConfigConstantHelper.trumgu_bi_db_connstr);
            MySqlParameter[] pms = new MySqlParameter[4];
            string sql_where = "";
            string sql_total = "SELECT id FROM fund.t_pf_sys_user WHERE 1=1 ";
            string sql_list = "SELECT id,name,userid, " +
                " password,status,lastlogin, " +
                " loginip,loginsum,islogin, " +
                " color,macaddr,expiretime, " +
                " person_liable,telephone,company_name, " +
                " hpcompany_id,parents_id, " +
                " create_time,create_user_name,create_user_id, " +
                " is_person_liable,person_liable_id, " +
                " mailbox,department,iscompany_show,isagree,ispass,special_id, " +
                " (SELECT COUNT(0) FROM trumgu_bi_db.t_pf_xfund_user_login_info WHERE login_name=userid AND login_time > DATE_ADD(CURDATE(),INTERVAL -14 DAY)) as 'sum_week_login_num', " +
                " (SELECT COUNT(0) FROM trumgu_bi_db.t_pf_xfund_user_login_info WHERE login_name=userid AND login_time > DATE_ADD(CURDATE(),INTERVAL -37 DAY)) as 'sum_year_login_num', " +
                " (SELECT COUNT(0) FROM trumgu_bi_db.t_pf_xfund_user_login_info WHERE login_name=userid) as 'sum_history_login_num' " +
                " FROM fund.t_pf_sys_user WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace (sel.name_like)) {
                pms[0] = new MySqlParameter ("@name_like", MySqlDbType.VarChar) { Value = "%" + sel.name_like + "%" };
                sql_where += " AND name LIKE @name_like";
            }
            if (!string.IsNullOrWhiteSpace (sel.company_name_like)) {
                pms[1] = new MySqlParameter ("@company_name_like", MySqlDbType.VarChar) { Value = "%" + sel.company_name_like + "%" };
                sql_where += " AND company_name LIKE @company_name_like";
            }
            if (!string.IsNullOrWhiteSpace (sel.person_liable_like)) {
                pms[2] = new MySqlParameter ("@person_liable_like", MySqlDbType.VarChar) { Value = "%" + sel.person_liable_like + "%" };
                sql_where += " AND person_liable LIKE @person_liable_like";
            }
            if (!string.IsNullOrWhiteSpace (sel.userid_like)) {
                pms[3] = new MySqlParameter ("@userid_like", MySqlDbType.VarChar) { Value = "%" + sel.userid_like + "%" };
                sql_where += " AND userid LIKE @userid_like";
            }

            sql_total += sql_where;
            sql_list += sql_where + " ORDER BY id DESC LIMIT " + ((sel.page - 1) * sel.rows) + "," + sel.rows;

            total = db.xfund_t_pf_sys_user.FromSql (sql_total, pms).Count ();
            if (total > 0) {
                users = db.xfund_t_pf_sys_user.FromSql (sql_list, pms).ToList ();
            }

            db.Dispose ();
            if (users == null) {
                users = new List<xfund_t_pf_sys_userExObj> ();
            }
            return Json (new { total = total, rows = users });
        }

        /// <summary>
        /// 添加用户列表
        /// </summary>
        public JsonResult AddPFXFundUser (xfund_t_pf_sys_userObj mdl) {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };
            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }
            xfund_t_pf_sys_userExObj m = new xfund_t_pf_sys_userExObj ();

            m.name = mdl.name;
            m.userid = mdl.userid;
            m.password = mdl.password;
            m.status = 1;
            m.loginsum = 0;
            m.islogin = 1;
            m.expiretime = mdl.expiretime;
            m.person_liable = mdl.person_liable;
            m.telephone = mdl.telephone;
            m.company_name = mdl.company_name;
            m.hpcompany_id = mdl.hpcompany_id;
            m.parents_id = mdl.parents_id;
            m.ispass = 0;
            m.isagree = 0;
            m.create_time = DateTime.Now;
            m.create_user_name = user.name;
            m.create_user_id = user.id;
            m.is_person_liable = mdl.is_person_liable;
            m.person_liable_id = mdl.person_liable_id;
            m.mailbox = mdl.mailbox;
            m.department = mdl.department;
            m.iscompany_show = mdl.iscompany_show;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            db.xfund_t_pf_sys_user.Add (m);
            if (db.SaveChanges () > 0) {
                ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_SAVE;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString ();
            }

            db.Dispose ();
            return Json (ro);
        }

        /// <summary>
        /// 获取私募版系统推荐账号
        /// </summary>
        [HttpPost]
        public JsonResult GetPFXFundNewUserid () {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };
            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext (ConfigConstantHelper.fund_connstr);
            List<xfund_t_sys_dictionariesObj> list = db.xfund_t_sys_dictionaries.Where (rec => rec.code == "J0000000").ToList ();

            if (list != null && list.Count == 1) {
                string v = list[0].value;
                string nextLoginNum = "S";
                int iNum = 0;
                if (!string.IsNullOrWhiteSpace (v) && int.TryParse (v.Substring (1), out iNum)) {
                    for (int i = iNum.ToString ().Length; i < 7; i++) {
                        nextLoginNum += "0";
                    }
                    list[0].value = nextLoginNum + (iNum + 1).ToString ();
                    db.xfund_t_sys_dictionaries.Update (list[0]);
                    db.SaveChanges ();
                    ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
                    ro.data = nextLoginNum + (iNum + 1).ToString ();
                }
            }
            db.Dispose ();
            return Json (ro);
        }

        public JsonResult GetVaguePrivateCompanyName(string q)
        {
            
        }
    }
}