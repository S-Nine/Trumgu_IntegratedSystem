using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
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

        //--------------------------------用户管理----------------------------
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
            if (sel.page == null)
            {
                sel.page = 1;
            }
            if (sel.rows == null)
            {
                sel.rows = 15;
            }
            var db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_user.Where(m => true); //延迟加载,并不会去数据库查询
            //条件查询
            if (!string.IsNullOrWhiteSpace(sel.name_like))
            {
                list = list.Where(m => m.name.Contains(sel.name_like));
            }

            if (!string.IsNullOrWhiteSpace(sel.userid_like))
            {
                list = list.Where(m => m.userid.Contains(sel.userid_like));
            }

            if (!string.IsNullOrWhiteSpace(sel.company_name_like))
            {
                list = list.Where(m => m.company_name.Contains(sel.company_name_like));
            }

            if (!string.IsNullOrWhiteSpace(sel.person_liable_like))
            {
                list = list.Where(m => m.person_liable.Contains(sel.person_liable_like));
            }
            var total = list.Count();
            //分页
            list = list.OrderByDescending(m => m.id)
                .Skip(Convert.ToInt32((sel.rows * (sel.page - 1))))
                .Take(Convert.ToInt32(sel.rows));
            var users = list.ToList();
            foreach (var item in users)
            {
                //根据userid得到关系表List
                var listRoleUser = db.xfund_t_sys_role_user.Where(m => m.userid == item.id).ToList();
                var roleId = new StringBuilder();
                var roleName = new StringBuilder();
                foreach (var roleUser in listRoleUser)
                {
                    var role = db.xfund_t_sys_role.FirstOrDefault(m => m.id == roleUser.roleid);
                    roleId.Append(roleUser.roleid + ",");
                    if (role != null) roleName.Append(role.role + ",");
                }

                item.role_str = roleName.ToString().TrimEnd(',');
                item.role_id_str = roleId.ToString().TrimEnd(',');

                var parentModel = db.xfund_t_sys_user.FirstOrDefault(m => m.id == item.parents_id);
                if (parentModel != null) item.parents_name = parentModel.name;
            }

            db.Dispose();
            return Json(new { total, rows = users });

        }

        /// <summary>
        /// 获取机构版推荐用户名
        /// </summary>
        /// <returns></returns>
        public JsonResult GetXFundNewUserid()
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_dictionaries.Where(rec => rec.code == "J0000000")
                .ToList();

            if (list.Count == 1)
            {
                var v = list[0].value;
                var nextLoginNum = "J";
                if (!string.IsNullOrWhiteSpace(v) && int.TryParse(v.Substring(1), out var iNum))
                {
                    for (var i = iNum.ToString().Length; i < 7; i++)
                    {
                        nextLoginNum += "0";
                    }

                    list[0].value = nextLoginNum + (iNum + 1);
                    db.xfund_t_sys_dictionaries.Update(list[0]);
                    db.SaveChanges();
                    ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                    ro.data = nextLoginNum + (iNum + 1);
                }
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 获取机构版角色列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetXFundRoleToList()
        {
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_role.ToList();
            db.Dispose();
            return Json(list);
        }

        /// <summary>
        /// 添加XFund用户
        /// </summary>
        /// <returns></returns>
        public JsonResult AddXFundUser(xfund_t_sys_userObj mdl, List<int> role_id_ary)
        {
            t_sys_userObj user = null;
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            var cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            mdl.password = AESHelper.GetMD5(mdl.password);
            mdl.islogin = 1;
            mdl.loginsum = 0;
            mdl.create_time = DateTime.Now;
            if (user != null) mdl.create_user_name = user.name;
            if (user != null) mdl.create_user_id = user.id;

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            if (db.xfund_t_sys_user.Count(m => m.userid == mdl.userid) <= 0)
            {
                db.xfund_t_sys_user.Add(mdl);
                if (db.SaveChanges() > 0)
                {
                    if (role_id_ary != null && role_id_ary.Count > 0)
                    {
                        var listRole = new List<xfund_t_sys_role_userObj>();
                        foreach (var t in role_id_ary)
                        {
                            listRole.Add(new xfund_t_sys_role_userObj
                            {
                                roleid = t,
                                userid = mdl.id
                            });
                        }
                        db.xfund_t_sys_role_user.AddRange(listRole);
                        db.SaveChanges();
                    }
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
                ro.data = "该账号已经存在！";
            }

            db.Dispose();
            return Json(ro);

        }

        /// <summary>
        /// 修改XFund用户
        /// </summary>
        /// <param name="mdl"></param>
        /// <param name="role_id_ary"></param>
        /// <returns></returns>
        public JsonResult UpdateXFundUser(xfund_t_sys_userObj mdl, List<int> role_id_ary)
        {
            t_sys_userObj user = null;
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            var cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var m = db.xfund_t_sys_user.FirstOrDefault(rec => rec.id == mdl.id);
            if (m != null)
            {
                m.name = mdl.name;
                m.status = mdl.status;
                m.islogin = mdl.islogin;
                m.expiretime = mdl.expiretime;
                m.person_liable = mdl.person_liable;
                m.telephone = mdl.telephone;
                m.company_name = mdl.company_name;
                m.create_time = DateTime.Now;
                if (user != null)
                {
                    m.create_user_name = user.name;
                    m.create_user_id = user.id;
                }

                m.customertype_name = mdl.customertype_name;

                m.is_person_liable = mdl.is_person_liable;
                m.person_liable_id = mdl.person_liable_id;
                m.mailbox = mdl.mailbox;
                m.department = mdl.department;
                m.is_pay = mdl.is_pay;

                db.xfund_t_sys_user.Update(m);

                // 删除旧角色关系
                var listRoleDel =
                    db.xfund_t_sys_role_user.Where(rec => rec.userid == m.id).ToList();
                db.xfund_t_sys_role_user.RemoveRange(listRoleDel);
                // 添加新角色关系
                if (role_id_ary != null && role_id_ary.Count > 0)
                {
                    var listRole = new List<xfund_t_sys_role_userObj>();
                    foreach (var t in role_id_ary)
                    {
                        listRole.Add(new xfund_t_sys_role_userObj()
                        {
                            roleid = t,
                            userid = m.id
                        });
                    }

                    db.xfund_t_sys_role_user.AddRange(listRole);
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
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
                ro.data = "该账号不存在！";
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 删除XFund用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteXFundUser(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var del = db.xfund_t_sys_user.FirstOrDefault(rec => rec.id == id);
            if (del != null)
            {


                db.xfund_t_sys_user.Remove(del);
                var listRole =
                    db.xfund_t_sys_role_user.Where(rec => rec.userid == id).ToList();
                if (listRole.Count > 0)
                {
                    db.xfund_t_sys_role_user.RemoveRange(listRole);
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
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="mdls"></param>
        /// <param name="role_id"></param>
        /// <returns></returns>
        public JsonResult AddBatchXFundUser(xfund_t_sys_userObj mdls, int role_id)
        {
            t_sys_userObj user = null;
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            var cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            if (mdls.startlist != null && mdls.endlist != null)
            {
                var list = new List<xfund_t_sys_userObj>();
                var listRole = new List<xfund_t_sys_role_userObj>();
                for (var i = mdls.startlist; i < mdls.endlist + 1; i++)
                {
                    var mdl = new xfund_t_sys_userObj
                    {
                        name = mdls.userid + i,
                        userid = mdls.userid + i,
                        password = AESHelper.GetMD5(mdls.password),
                        person_liable = mdls.person_liable,
                        expiretime = mdls.expiretime,
                        company_name = mdls.company_name,
                        islogin = 1,
                        loginsum = 0,
                        create_time = DateTime.Now
                    };
                    if (user != null) mdl.create_user_name = user.name;
                    if (user != null) mdl.create_user_id = user.id;

                    var existCount = db.xfund_t_sys_user.Count(m => m.userid == mdl.userid);
                    if (existCount > 0)
                    {
                        //如果存在相同账号,直接return
                        ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                        ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
                        ro.data = "该账号已经存在！";
                        return Json(ro);

                    }

                    list.Add(mdl);
                    var roleMdl = new xfund_t_sys_role_userObj
                    {
                        userid = mdl.id,
                        roleid = role_id
                    };
                    listRole.Add(roleMdl);

                }

                db.xfund_t_sys_user.AddRange(list);
                db.xfund_t_sys_role_user.AddRange(listRole);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_SAVE;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 修改登陆状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult UpdateUserState(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.xfund_t_sys_user.FirstOrDefault(rec => rec.id == id);
            if (model != null)
            {
                model.islogin = model.islogin == 0 ? 1 : 0;
                db.xfund_t_sys_user.Update(model);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        public JsonResult GetCusType()
        {
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_dictionaries.Where(m => m.parentCode == "CustomerType").ToList();
            db.Dispose();
            return Json(list);
        }



        //---------------------------------尽调上传---------------------------------
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


        /// <summary>
        /// 根据公司编码得到回显数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult GetCompanyInfo(string code)
        {

            var ro = new xfund_t_fund_companyExObj();
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.xfund_t_fund_company.FirstOrDefault(m => m.regis_code == code);

            if (model == null)
            {
                return null;
            }

            ro.id = model.id;
            ro.cn_name = model.cn_name;
            ro.regis_code = model.regis_code;
            ro.setup_date = model.setup_date;
            ro.en_name = model.en_name;
            ro.legal_repre = model.legal_repre;
            ro.corp_property = model.corp_property;
            ro.org_code = model.org_code;
            ro.regis_capital = model.regis_capital;
            ro.regis_date = model.regis_date;
            ro.paidin_capital = model.paidin_capital;
            ro.fund_count = model.fund_count;
            ro.if_member = model.if_member;
            ro.company_scale = model.company_scale;
            ro.staff_count = model.staff_count;
            ro.website = model.website;
            ro.regis_address = model.regis_address;
            ro.address = model.address;

            var modelEx = db.t_companyInfo.FirstOrDefault(m => m.id.ToString() == model.hpcompany_id);
            if (modelEx == null)
            {
                return Json(ro);
            }

            ro.company_intro = modelEx.company_intro;
            ro.invest_idea = modelEx.invest_idea;
            ro.core_person = modelEx.core_person;


            return Json(ro);
        }

        /// <summary>
        /// 修改公司详细信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult UpdateCompanyInfo(xfund_t_fund_companyExObj obj)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.xfund_t_fund_company.FirstOrDefault(m => m.id == obj.id);
            if (model != null)
            {
                model.regis_code = obj.regis_code;
                model.setup_date = obj.setup_date;
                model.en_name = obj.en_name;
                model.legal_repre = obj.legal_repre;
                model.corp_property = obj.corp_property;
                model.org_code = obj.org_code;
                model.regis_capital = obj.regis_capital;
                model.regis_date = obj.regis_date;
                model.paidin_capital = obj.paidin_capital;
                model.fund_count = obj.fund_count;
                model.if_member = obj.if_member;
                model.company_scale = obj.company_scale;
                model.staff_count = obj.staff_count;
                model.website = obj.website;
                model.regis_address = obj.regis_address;
                model.address = obj.address;
                var modelEx = db.t_companyInfo.FirstOrDefault(m => m.id.ToString() == model.hpcompany_id);
                if (modelEx != null)
                {
                    modelEx.company_intro = obj.company_intro;
                    modelEx.invest_idea = obj.invest_idea;
                    modelEx.core_person = obj.core_person;
                    db.t_companyInfo.Update(modelEx);
                }

                db.xfund_t_fund_company.Update(model);
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
                return Json(ro);
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
            return Json(ro);

        }

        //-----------------------------菜单管理---------------------------
        public IActionResult MenuManager()
        {
            return View();
        }

        /// <summary>
        /// 不分页获取全部菜单管理（机构）列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenuToList()
        {
            var menuAry = new List<object>();
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_menu.ToList();
            var rootAry = list.Where(rec => rec.menu_level == 1).ToList();
            if (rootAry.Count > 0)
            {
                rootAry = rootAry.OrderBy(rec => rec.seq).ToList();
                foreach (var t in rootAry)
                {
                    menuAry.Add(new
                    {
                        t.id,
                        t.menu,
                        t.menu_level,
                        t.classes,
                        t.path,
                        t.seq,
                        t.status,
                        t.pathweb,
                        t.code,
                        children = list
                            .Where(rec =>
                                rec.classes != null && rec.classes != t.id.ToString() &&
                                rec.classes.Split(new[] { '.' }).Contains(t.id.ToString()))
                            .OrderBy(rec => rec.seq).Select(rec => new
                            {
                                rec.id,
                                rec.menu,
                                rec.menu_level,
                                rec.classes,
                                rec.path,
                                rec.seq,
                                rec.status,
                                rec.pathweb,
                                rec.code,
                            }).ToList()
                    });
                }
            }

            db.Dispose();
            return Json(menuAry);
        }

        /// <summary>
        /// 不分页获取一级菜单信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRootMenuToList()
        {
            var menuAry = new List<object> { new { id = (int?) 0, menu = "顶级菜单" } };
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_menu.Where(rec => rec.menu_level == 1)
                .OrderBy(rec => rec.seq).Select(rec => new xfund_t_sys_menuObj()
                {
                    id = rec.id,
                    menu = rec.menu
                }).ToList();
            if (list.Count > 0)
            {
                foreach (var t in list)
                {
                    menuAry.Add(new
                    {
                        t.id,
                        t.menu
                    });
                }
            }

            db.Dispose();
            return Json(menuAry);
        }


        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddMenu(xfund_t_sys_menuExObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var m = new xfund_t_sys_menuObj
            {
                menu = mdl.menu,
                menu_level = mdl.menu_level,
                path = mdl.path,
                seq = mdl.seq,
                status = mdl.status,
                pathweb = mdl.pathweb,
                code = mdl.code
            };

            var db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            db.xfund_t_sys_menu.Add(m);
            if (db.SaveChanges() > 0)
            {
                m.classes = mdl.parent_id != null && mdl.parent_id != 0 ? mdl.parent_id + "." + m.id : m.id.ToString();
                db.xfund_t_sys_menu.Update(m);
                db.SaveChanges();
                ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_SAVE;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateMenu(xfund_t_sys_menuExObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_menu.Where(rec => rec.id == mdl.id).ToList();
            if (list.Count == 1)
            {
                list[0].menu = mdl.menu;
                list[0].menu_level = mdl.menu_level;
                list[0].classes = mdl.parent_id != null && mdl.parent_id != 0
                    ? mdl.parent_id + "." + mdl.id
                    : mdl.id.ToString();
                list[0].path = mdl.path;
                list[0].seq = mdl.seq;
                list[0].status = mdl.status;
                list[0].pathweb = mdl.pathweb;
                list[0].code = mdl.code;

                db.xfund_t_sys_menu.Update(list[0]);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }

            db.Dispose();
            return Json(ro);
        }


        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteMenu(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            if (id > 0) // 一级菜单的parent_id为0，所以禁止删除所有一级菜单
            {
                var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                var list = db.xfund_t_sys_menu
                    .Where(rec => rec.id == id || rec.classes.Contains(id.ToString() + ".")).ToList();

                if (list.Count > 0)
                {
                    // 删除菜单下的按钮
                    var listBtnDel = db.xfund_t_sys_button
                        .Where(rec => list.Any(r => r.id == rec.menu_id)).ToList();
                    if (listBtnDel.Count > 0)
                    {
                        db.xfund_t_sys_button.RemoveRange(listBtnDel);
                    }

                    // 删除菜单、按钮、角色管理
                    var listRleDel = db.xfund_t_sys_button_right.Where(rec =>
                        list.Any(r => r.id == rec.menu_id) || listBtnDel.Any(r => r.id == rec.btn_id)).ToList();
                    if (listRleDel.Count > 0)
                    {
                        db.xfund_t_sys_button_right.RemoveRange(listRleDel);
                    }

                    db.xfund_t_sys_menu.RemoveRange(list);
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
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
                }
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }

            return Json(ro);
        }

        /// <summary>
        /// 不分页获取指定菜单id的按钮列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenuButton(int menuId)
        {
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);

            var btnAry = db.xfund_t_sys_button.Where(rec => rec.menu_id == menuId).OrderBy(rec => rec.sort).ToList();

            db.Dispose();
            return Json(btnAry);
        }

        /// <summary>
        /// 添加菜单按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddButton(xfund_t_sys_buttonObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            db.xfund_t_sys_button.Add(mdl);
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
            return Json(ro);
        }

        /// <summary>
        /// 修改菜单按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateButton(xfund_t_sys_buttonObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_button.Where(rec => rec.id == mdl.id).ToList();
            if (list.Count == 1)
            {
                list[0].btn_js_id = mdl.btn_js_id;
                list[0].btn_name = mdl.btn_name;
                list[0].menu_id = mdl.menu_id;
                list[0].sort = mdl.sort;

                db.xfund_t_sys_button.Update(list[0]);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 删除菜单按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteButton(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            if (id > 0) // 一级菜单的parent_id为0，所以禁止删除所有一级菜单
            {
                var db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                var list = db.xfund_t_sys_button.Where(rec => rec.id == id).ToList();
                if (list.Count == 1)
                {
                    db.xfund_t_sys_button.Remove(list[0]);
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
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
                }
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }

            return Json(ro);
        }

        //----------------------------------角色管理(机)---------------------
        public IActionResult RoleManager()
        {
            return View();
        }

        /// <summary>
        /// 不分页获取全部角色信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRoleToList(xfund_t_sys_roleSelObj sel)
        {
            var roleAry = new List<object>();
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_role.Where(
                rec => (string.IsNullOrWhiteSpace(sel.name_like) || rec.role.Contains(sel.name_like))
            ).ToList();

            if (list.Count > 0)
            {
                foreach (var t in list)
                {
                    roleAry.Add(new
                    {
                        t.id,
                        t.role,
                        t.rolecode,
                        t.status,
                        t.data_authority
                    });
                }
            }

            db.Dispose();
            return Json(roleAry);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddRole(xfund_t_sys_roleObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            DataContextHelper db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            db.xfund_t_sys_role.Add(mdl);
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
            return Json(ro);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateRole(xfund_t_sys_roleObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_role.Where(rec => rec.id == mdl.id).ToList();
            if (list.Count == 1)
            {
                list[0].role = mdl.role;
                list[0].rolecode = mdl.rolecode;
                list[0].status = mdl.status;
                list[0].data_authority = mdl.data_authority;

                db.xfund_t_sys_role.Update(list[0]);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteRole(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.xfund_t_sys_role.Where(rec => rec.id == id).ToList();
            var listRelUser =
                db.xfund_t_sys_role_user.Where(rec => rec.roleid == id).ToList();
            var listRelMenu =
                db.xfund_t_sys_button_right.Where(rec => rec.role_id == id).ToList();

            if (list.Count == 1)
            {
                // 删除角色菜单关系
                if (listRelMenu.Count > 0)
                {
                    db.xfund_t_sys_button_right.RemoveRange(listRelMenu);
                }

                // 删除角色和用户关系
                if (listRelUser.Count > 0)
                {
                    db.xfund_t_sys_role_user.UpdateRange(listRelUser);
                }

                db.xfund_t_sys_role.Remove(list[0]);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 根据角色ID获取菜单权限列表 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRoleMenuRight(int id)
        {
            var treeData = new List<TreeDataObj>();
            var db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var query1 = db.MenuTreeData.FromSql(
                @"SELECT CONCAT('m_', a.id) AS 'id',a.id AS 'key',a.classes AS 'parent_id',a.menu AS 'text',a.menu_level AS 'level',(SELECT CASE WHEN COUNT(0) > 0 THEN 'true' ELSE 'false' END FROM t_sys_button_right r WHERE r.role_id = " +
                id +
                " AND r.menu_id = a.id ) AS 'check', 'menu' AS 'type', a.seq AS 'sort','' AS 'iconCls' FROM t_sys_menu a ");
            var list1 = query1.ToList();
            var query2 = db.MenuTreeData.FromSql(
                @"SELECT CONCAT('b_', a.id) AS 'id', a.id AS 'key', a.menu_id AS 'parent_id', a.btn_name AS 'text', NULL AS 'level', ( SELECT CASE WHEN COUNT(0) > 0 THEN 'true' ELSE 'false' END FROM t_sys_button_right r WHERE r.role_id = " +
                id +
                " AND r.btn_id = a.id ) AS 'check', 'button' AS 'type', a.sort AS 'sort','' AS 'iconCls' FROM t_sys_button a ");
            var list2 = query2.ToList();
            var list = new List<MenuTreeDataObj>();
            foreach (var t in list1)
            {
                t.parent_id = t.parent_id?.Replace("." + t.key, "");
                if (t.parent_id == t.key.ToString())
                {
                    t.parent_id = "";
                }
            }

            list.AddRange(list1); // EF使用UNION ALL 如果ID重复则数据会出现错误
            list.AddRange(list2);

            if (list.Count > 0)
            {
                var root = list.Where(rec => rec.level == 1 && rec.type == "menu")
                    .OrderBy(rec => rec.sort).ToList();
                if (root.Count > 0)
                {
                    foreach (var t in root)
                    {
                        var r = new TreeDataObj
                        {
                            id = t.type + "_" + t.key,
                            text = t.text,
                            state = "",
                            check = t.check,
                            iconCls = t.iconCls,
                            attributes = t.parent_id ?? "",
                            key = t.key,
                            children = list.Where(rec => rec.parent_id == t.key.ToString()).Select(rec =>
                                new TreeDataObj() // 二级菜单及一级按钮
                                {
                                    id = rec.type + "_" + rec.key,
                                    text = rec.text,
                                    state = "",
                                    check = rec.check,
                                    iconCls = rec.iconCls,
                                    key = rec.key,
                                    attributes = rec.parent_id.ToString()
                                }).ToList()
                        };

                        if (r.children != null && r.children.Count > 0)
                        {
                            for (var j = 0; j < r.children.Count; j++)
                            {
                                r.children[j].children = list
                                    .Where(rec => rec.parent_id == r.children[j].key.ToString()).Select(rec =>
                                        new TreeDataObj() // 二级菜单及一级按钮
                                        {
                                            id = rec.type + "_" + rec.key,
                                            text = rec.text,
                                            state = "",
                                            check = rec.check,
                                            iconCls = rec.iconCls,
                                            attributes = rec.parent_id.ToString()
                                        }).ToList();
                            }
                        }

                        treeData.Add(r);
                    }
                }
            }

            db.Dispose();
            return Json(treeData);
        }


        /// <summary>
        /// 根据角色ID分配权限 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DistributionRight(int role_id, List<xfund_t_sys_button_rightObj> list)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var listDelRel =
                db.xfund_t_sys_button_right.Where(rec => rec.role_id == role_id).ToList();

            if (listDelRel.Count > 0)
            {
                db.xfund_t_sys_button_right.RemoveRange(listDelRel);
            }

            if (list != null && list.Count > 0)
            {
                db.xfund_t_sys_button_right.AddRange(list);
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
            return Json(ro);
        }
    }



}