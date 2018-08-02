using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Trumgu_IntegratedManageSystem.Attributes;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;
using Trumgu_IntegratedManageSystem.Models.xfund;
using Trumgu_IntegratedManageSystem.Utils;

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class PFXFundOrganizationController : Controller
    {
        private const string PF_USER_PWD = "abcdefgabcdefg12";

        /// <summary>
        /// 用户管理（私募版）
        /// </summary>
        public IActionResult UserManager()
        {
            return View();
        }

        /// <summary>
        /// 分页用户列表
        /// </summary>
        public JsonResult GetPFXFundUserListToPage(xfund_t_pf_sys_userSelObj sel)
        {
            int total = 0;
            List<xfund_t_pf_sys_userExObj> users = null;
            if (sel.page == null)
            {
                sel.page = 1;
            }

            if (sel.rows == null)
            {
                sel.rows = 15;
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
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
                              " is_person_liable,person_liable_id,is_pay, " +
                              " mailbox,department,iscompany_show,isagree,ispass,special_id, " +
                              " (SELECT name FROM fund.t_pf_sys_user U WHERE U.id = T.parents_id) AS 'parents_name'," +
                              " (SELECT COUNT(0) FROM trumgu_bi_db.t_pf_xfund_user_login_info WHERE login_name=userid AND login_time > DATE_ADD(CURDATE(),INTERVAL -14 DAY)) as 'sum_week_login_num', " +
                              " (SELECT COUNT(0) FROM trumgu_bi_db.t_pf_xfund_user_login_info WHERE login_name=userid AND login_time > DATE_ADD(CURDATE(),INTERVAL -37 DAY)) as 'sum_year_login_num', " +
                              " (SELECT COUNT(0) FROM trumgu_bi_db.t_pf_xfund_user_login_info WHERE login_name=userid) as 'sum_history_login_num', " +
                              " (SELECT group_concat(A.roleid) FROM fund.t_pf_sys_role_user A INNER JOIN fund.t_pf_sys_role B ON A.roleid = B.id WHERE A.userid=T.id ORDER BY A.roleid) as 'role_id_str', " +
                              " (SELECT group_concat(B.role) FROM fund.t_pf_sys_role_user A INNER JOIN fund.t_pf_sys_role B ON A.roleid = B.id WHERE A.userid=T.id ORDER BY B.role) as 'role_str' " +
                              " FROM fund.t_pf_sys_user as T WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace(sel.name_like))
            {
                pms[0] = new MySqlParameter("@name_like", MySqlDbType.VarChar) { Value = "%" + sel.name_like + "%" };
                sql_where += " AND name LIKE @name_like";
            }

            if (!string.IsNullOrWhiteSpace(sel.company_name_like))
            {
                pms[1] = new MySqlParameter("@company_name_like", MySqlDbType.VarChar)
                {
                    Value = "%" + sel.company_name_like + "%"
                };
                sql_where += " AND company_name LIKE @company_name_like";
            }

            if (!string.IsNullOrWhiteSpace(sel.person_liable_like))
            {
                pms[2] = new MySqlParameter("@person_liable_like", MySqlDbType.VarChar)
                {
                    Value = "%" + sel.person_liable_like + "%"
                };
                sql_where += " AND person_liable LIKE @person_liable_like";
            }

            if (!string.IsNullOrWhiteSpace(sel.userid_like))
            {
                pms[3] = new MySqlParameter("@userid_like", MySqlDbType.VarChar) { Value = "%" + sel.userid_like + "%" };
                sql_where += " AND userid LIKE @userid_like";
            }

            if (sel.is_pay != null)
            {
                if (sel.is_pay == 1)
                {
                    sql_where += " AND is_pay = 1";
                }
                else if (sel.is_pay == 0)
                {
                    sql_where += " AND (is_pay <> 1 OR is_pay IS NULL) ";
                }
            }

            sql_total += sql_where;
            sql_list += sql_where + " ORDER BY id DESC LIMIT " + ((sel.page - 1) * sel.rows) + "," + sel.rows;

            total = db.xfund_t_pf_sys_userEx.FromSql(sql_total, pms).Count();
            if (total > 0)
            {
                users = db.xfund_t_pf_sys_userEx.FromSql(sql_list, pms).ToList();
            }

            db.Dispose();
            if (users == null)
            {
                users = new List<xfund_t_pf_sys_userExObj>();
            }

            return Json(new { total = total, rows = users });
        }

        /// <summary>
        /// 添加用户列表
        /// </summary>
        public JsonResult AddPFXFundUser(xfund_t_pf_sys_userExObj mdl, List<int> role_id_ary)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            xfund_t_pf_sys_userObj m = new xfund_t_pf_sys_userObj();

            m.name = mdl.name;
            m.userid = mdl.userid;
            m.password = AESHelper.GetMD5(mdl.password);
            m.status = mdl.status;
            m.loginsum = mdl.loginsum;
            m.islogin = 1;
            m.expiretime = mdl.expiretime;
            m.person_liable = mdl.person_liable;
            m.telephone = mdl.telephone;
            m.company_name = mdl.company_name;
            m.hpcompany_id = mdl.hpcompany_id;
            m.parents_id = mdl.parents_id;
            m.is_pay = 0;
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

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            if (db.xfund_t_pf_sys_user.Where(rec => rec.userid == m.userid).Count() <= 0)
            {
                db.xfund_t_pf_sys_user.Add(m);
                //查询公司入驻状态,如果为0 改成1
                var companyModel = db.xfund_t_fund_company.FirstOrDefault(c => c.hpcompany_id == m.hpcompany_id);
                if (companyModel == null)
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
                    return Json(ro);
                }
                if (companyModel.settled == 0)
                {
                    companyModel.settled = 1;
                    companyModel.settled_date = DateTime.Now;
                    db.xfund_t_fund_company.Update(companyModel);
                }

                if (db.SaveChanges() > 0)
                {
                    
                    if (role_id_ary != null && role_id_ary.Count > 0)
                    {
                        List<xfund_t_pf_sys_role_userObj> list_role = new List<xfund_t_pf_sys_role_userObj>();
                        for (int i = 0; i < role_id_ary.Count; i++)
                        {
                            list_role.Add(new xfund_t_pf_sys_role_userObj()
                            {
                                roleid = role_id_ary[i],
                                userid = m.id
                            });
                        }

                        db.xfund_t_pf_sys_role_user.AddRange(list_role);
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
        /// 修改用户列表
        /// </summary>
        public JsonResult UpdatePFXFundUser(xfund_t_pf_sys_userExObj mdl, List<int> role_id_ary)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            xfund_t_pf_sys_userObj m = null;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            m = db.xfund_t_pf_sys_user.Where(rec => rec.id == mdl.id).FirstOrDefault();
            if (m != null)
            {
                m.name = mdl.name;
                m.status = mdl.status;
                m.islogin = mdl.islogin;
                m.expiretime = mdl.expiretime;
                m.person_liable = mdl.person_liable;
                m.telephone = mdl.telephone;
                m.company_name = mdl.company_name;
                m.hpcompany_id = mdl.hpcompany_id;
                m.create_time = DateTime.Now;
                m.create_user_name = user.name;
                m.create_user_id = user.id;
                m.is_person_liable = mdl.is_person_liable;
                m.person_liable_id = mdl.person_liable_id;
                m.mailbox = mdl.mailbox;
                m.department = mdl.department;
                m.iscompany_show = mdl.iscompany_show;

                db.xfund_t_pf_sys_user.Update(m);

                // 删除旧角色关系
                List<xfund_t_pf_sys_role_userObj> list_role_del =
                    db.xfund_t_pf_sys_role_user.Where(rec => rec.userid == m.id).ToList();
                db.xfund_t_pf_sys_role_user.RemoveRange(list_role_del);
                // 添加新角色关系
                if (role_id_ary != null && role_id_ary.Count > 0)
                {
                    List<xfund_t_pf_sys_role_userObj> list_role = new List<xfund_t_pf_sys_role_userObj>();
                    for (int i = 0; i < role_id_ary.Count; i++)
                    {
                        list_role.Add(new xfund_t_pf_sys_role_userObj()
                        {
                            roleid = role_id_ary[i],
                            userid = m.id
                        });
                    }

                    db.xfund_t_pf_sys_role_user.AddRange(list_role);
                }

                //查询公司入驻状态,如果为0 改成1
                var companyModel = db.xfund_t_fund_company.FirstOrDefault(c => c.hpcompany_id == m.hpcompany_id);
                if (companyModel == null)
                {
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
                    return Json(ro);
                }
                if (companyModel.settled == 0)
                {
                    companyModel.settled = 1;
                    companyModel.settled_date = DateTime.Now;
                    db.xfund_t_fund_company.Update(companyModel);
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
        /// 获取私募版系统推荐账号
        /// </summary>
        [HttpPost]
        public JsonResult GetPFXFundNewUserid()
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_sys_dictionariesObj> list = db.xfund_t_sys_dictionaries.Where(rec => rec.code == "S0000000")
                .ToList();

            if (list != null && list.Count == 1)
            {
                string v = list[0].value;
                string nextLoginNum = "S";
                int iNum = 0;
                if (!string.IsNullOrWhiteSpace(v) && int.TryParse(v.Substring(1), out iNum))
                {
                    for (int i = iNum.ToString().Length; i < 7; i++)
                    {
                        nextLoginNum += "0";
                    }

                    list[0].value = nextLoginNum + (iNum + 1).ToString();
                    db.xfund_t_sys_dictionaries.Update(list[0]);
                    db.SaveChanges();
                    ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                    ro.data = nextLoginNum + (iNum + 1).ToString();
                }
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 获取私募公司cn_name&hpcompany_id
        /// </summary>
        [HttpPost]
        public JsonResult GetVaguePrivateCompanyName(string q)
        {
            List<xfund_t_fund_companyObj> list = null;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            list = db.xfund_t_fund_company
                .Where(rec =>
                    (!string.IsNullOrWhiteSpace(q) ? rec.cn_name.Contains(q) : true) && rec.isdelete == 0 &&
                    rec.hpcompany_id != null)
                .Select(rec => new xfund_t_fund_companyObj() { hpcompany_id = rec.hpcompany_id, cn_name = rec.cn_name })
                .Take(15)
                .ToList();
            db.Dispose();
            if (list == null)
            {
                list = new List<xfund_t_fund_companyObj>();
            }

            return Json(list);
        }

        /// <summary>
        /// 根据hpcompany_id获取公司信息
        /// </summary>
        [HttpPost]
        public JsonResult GetPrivateCompanyNameByHPCompanyID(string hpcompany_id)
        {
            xfund_t_fund_companyObj m = null;
            if (!string.IsNullOrWhiteSpace(hpcompany_id))
            {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                m = db.xfund_t_fund_company.FirstOrDefault(rec => rec.hpcompany_id == hpcompany_id);
                db.Dispose();
            }

            return Json(m);
        }

        /// <summary>
        /// 不分页获取私募角色
        /// </summary>
        [HttpPost]
        public JsonResult GetPrivateRoleToList()
        {
            List<xfund_t_pf_sys_roleObj> list = null;
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            list = db.xfund_t_pf_sys_role.ToList();
            db.Dispose();

            return Json(list);
        }

        /// <summary>
        /// 物理删除私募用户信息
        /// </summary>
        [HttpPost]
        public JsonResult DeletePFXFundUser(int id)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            xfund_t_pf_sys_userObj del = db.xfund_t_pf_sys_user.Where(rec => rec.id == id).FirstOrDefault();
            if (del != null)
            {
                //查询公司内的用户数量 如果>1 不做操作 如果==1 将公司入驻状态改为0
                var userCount = db.xfund_t_pf_sys_user.Count(m => m.hpcompany_id == del.hpcompany_id);
                var companyModel = db.xfund_t_fund_company.FirstOrDefault(c => c.hpcompany_id == del.hpcompany_id);
                if (companyModel != null)
                {
                    var settledVal = companyModel.settled;
                    if (userCount > 1)
                    {
                        if (settledVal == 0)
                        {
                            //按理说不会出现这种情况,但出现了应该做出修改
                            companyModel.settled = 1;
                            companyModel.settled_date = DateTime.Now;
                        }
                        //else==1的情况 就不做处理即可
                    }
                    else
                    {
                        if (settledVal == 1)
                        {
                            companyModel.settled = 0;
                            companyModel.settled_date = DateTime.MinValue;
                        }
                        //else==0的情况不做处理
                    }
                    db.xfund_t_fund_company.Update(companyModel);
                }

                db.xfund_t_pf_sys_user.Remove(del);
                List<xfund_t_pf_sys_role_userObj> list_role =
                    db.xfund_t_pf_sys_role_user.Where(rec => rec.userid == id).ToList();
                if (list_role != null && list_role.Count > 0)
                {
                    db.xfund_t_pf_sys_role_user.RemoveRange(list_role);
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
        /// 重置密码
        /// </summary>
        public JsonResult UPWDPFXFundUser(int id, string p)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            xfund_t_pf_sys_userObj del = db.xfund_t_pf_sys_user.Where(rec => rec.id == id).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(p))
            {
                List<xfund_t_pf_sys_userObj> list = db.xfund_t_pf_sys_user.Where(rec => rec.id == id).ToList();
                if (list != null && list.Count > 0)
                {
                    list[0].password = AESHelper.GetMD5(p);
                    db.xfund_t_pf_sys_user.Update(list[0]);

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
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        public JsonResult GetUserFiles(int id)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            if (db.xfund_t_pf_sys_user.Where(rec => rec.id == id).Count() == 1)
            {
                List<t_sys_fileObj> files = db.t_sys_file.Where(rec =>
                    rec.belong_modular == "t_pf_sys_user" && rec.belong_modular_id == id.ToString()).ToList();
                ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
                ro.data = files;
            }
            else
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        public JsonResult GetUserFileList(int id)
        {
            List<t_sys_fileObj> files = null;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            if (db.xfund_t_pf_sys_user.Where(rec => rec.id == id).Count() == 1)
            {
                files = db.t_sys_file.Where(rec =>
                    rec.belong_modular == "t_pf_sys_user" && rec.belong_modular_id == id.ToString()).ToList();
            }

            db.Dispose();

            if (files == null)
            {
                files = new List<t_sys_fileObj>();
            }

            return Json(files);
        }

        /// <summary>
        /// 保存用户附件
        /// </summary>
        public JsonResult UploadFilesPFXFundUser(int id, List<FileInfoObj> files)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_userObj> list = db.xfund_t_pf_sys_user.Where(rec => rec.id == id).ToList();
            if (list != null && list.Count > 0)
            {
                List<t_sys_fileObj> list_file_del = db.t_sys_file.Where(rec =>
                    rec.belong_modular == "t_pf_sys_user" && rec.belong_modular_id == id.ToString()).ToList();
                // 删除历史文件
                if (list_file_del != null && list_file_del.Count > 0)
                {
                    list_file_del.ForEach(f =>
                    {
                        db.t_sys_file.Attach(f);
                        db.t_sys_file.Remove(f);
                    });
                }

                // 新增文件
                if (files != null)
                {
                    DateTime dtNow = DateTime.Now;
                    List<t_sys_fileObj> list_file = new List<t_sys_fileObj>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        list_file.Add(new t_sys_fileObj()
                        {
                            file_name = files[i].fileName,
                            file_type = string.IsNullOrWhiteSpace(files[i].fileName)
                                ? ""
                                : files[i].fileName.Substring(files[i].fileName.LastIndexOf(".") + 1),
                            file_path = files[i].fileUrl,
                            file_size = files[i].fileSize,
                            upload_time = dtNow,
                            belong_modular = "t_pf_sys_user",
                            belong_modular_id = id.ToString()
                        });
                    }

                    db.t_sys_file.AddRange(list_file);
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
        /// 延期试用
        /// </summary>
        [HttpPost]
        public JsonResult DelayPFXFundUser(int id, DateTime t)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_userObj> users = db.xfund_t_pf_sys_user.Where(rec => rec.id == id).ToList();
            if (users != null && users.Count > 0)
            {
                users[0].expiretime = t;
                users[0].create_user_name = user.name;
                users[0].create_user_id = user.id;
                db.xfund_t_pf_sys_user.Update(users[0]);

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

            db.Dispose();

            return Json(ro);
        }

        /// <summary>
        /// 转为正式用户
        /// </summary>
        [HttpPost]
        public JsonResult FormalPFXFundUser(int id, DateTime t)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_userObj> users = db.xfund_t_pf_sys_user.Where(rec => rec.id == id && rec.is_pay != 1)
                .ToList();
            if (users != null && users.Count > 0)
            {
                users[0].expiretime = t;
                users[0].create_user_name = user.name;
                users[0].create_user_id = user.id;
                users[0].is_pay = 1;
                db.xfund_t_pf_sys_user.Update(users[0]);

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

            db.Dispose();

            return Json(ro);
        }

        /// <summary>
        /// 菜单管理（私募）
        /// </summary>
        public IActionResult MenuManager()
        {
            return View();
        }

        /// <summary>
        /// 不分页获取全部菜单管理（私募）列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenuToList()
        {
            List<object> menu_ary = new List<object>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_menuObj> list = db.xfund_t_pf_sys_menu.ToList();
            List<xfund_t_pf_sys_menuObj> root_ary = list.Where(rec => rec.menu_level == 1).ToList();
            if (root_ary != null && root_ary.Count > 0)
            {
                root_ary = root_ary.OrderBy(rec => rec.seq).ToList();
                for (int i = 0; i < root_ary.Count; i++)
                {
                    menu_ary.Add(new
                    {
                        id = root_ary[i].id,
                        menu = root_ary[i].menu,
                        menu_level = root_ary[i].menu_level,
                        classes = root_ary[i].classes,
                        path = root_ary[i].path,
                        seq = root_ary[i].seq,
                        status = root_ary[i].status,
                        pathweb = root_ary[i].pathweb,
                        code = root_ary[i].code,
                        children = list
                            .Where(rec =>
                                rec.classes != null && rec.classes != root_ary[i].id.ToString() &&
                                rec.classes.Split(new char[] { '.' }).Contains(root_ary[i].id.ToString()))
                            .OrderBy(rec => rec.seq).Select(rec => new
                            {
                                id = rec.id,
                                menu = rec.menu,
                                menu_level = rec.menu_level,
                                classes = rec.classes,
                                path = rec.path,
                                seq = rec.seq,
                                status = rec.status,
                                pathweb = rec.pathweb,
                                code = rec.code,
                            }).ToList()
                    });
                }

            }

            db.Dispose();
            return Json(menu_ary);
        }

        /// <summary>
        /// 不分页获取一级菜单信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRootMenuToList()
        {
            List<object> menu_ary = new List<object>() { new { id = 0, menu = "顶级菜单" } };
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_menuObj> list = db.xfund_t_pf_sys_menu.Where(rec => rec.menu_level == 1)
                .OrderBy(rec => rec.seq).Select(rec => new xfund_t_pf_sys_menuObj()
                {
                    id = rec.id,
                    menu = rec.menu
                }).ToList();
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    menu_ary.Add(new
                    {
                        id = list[i].id,
                        menu = list[i].menu
                    });
                }
            }

            db.Dispose();
            return Json(menu_ary);
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddMenu(xfund_t_pf_sys_menuExObj mdl)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            xfund_t_pf_sys_menuObj m = new xfund_t_pf_sys_menuObj();
            m.menu = mdl.menu;
            m.menu_level = mdl.menu_level;
            m.path = mdl.path;
            m.seq = mdl.seq;
            m.status = mdl.status;
            m.pathweb = mdl.pathweb;
            m.code = mdl.code;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            db.xfund_t_pf_sys_menu.Add(m);
            if (db.SaveChanges() > 0)
            {
                m.classes = mdl.parent_id != null && mdl.parent_id != 0 ? mdl.parent_id + "." + m.id : m.id.ToString();
                db.xfund_t_pf_sys_menu.Update(m);
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
        public JsonResult UpdateMenu(xfund_t_pf_sys_menuExObj mdl)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_menuObj> list = db.xfund_t_pf_sys_menu.Where(rec => rec.id == mdl.id).ToList();
            if (list != null && list.Count == 1)
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

                db.xfund_t_pf_sys_menu.Update(list[0]);
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
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            if (id > 0) // 一级菜单的parent_id为0，所以禁止删除所有一级菜单
            {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                List<xfund_t_pf_sys_menuObj> list = db.xfund_t_pf_sys_menu
                    .Where(rec => rec.id == id || rec.classes.Contains(id.ToString() + ".")).ToList();

                if (list != null && list.Count > 0)
                {
                    // 删除菜单下的按钮
                    List<xfund_t_pf_sys_buttonObj> list_btn_del = db.xfund_t_pf_sys_button
                        .Where(rec => list.Where(r => r.id == rec.menu_id).Count() > 0).ToList();
                    if (list_btn_del != null && list_btn_del.Count > 0)
                    {
                        db.xfund_t_pf_sys_button.RemoveRange(list_btn_del);
                    }

                    // 删除菜单、按钮、角色管理
                    List<xfund_t_pf_sys_button_rightObj> list_rle_del = db.xfund_t_pf_sys_button_right.Where(rec =>
                        list.Where(r => r.id == rec.menu_id).Count() > 0 ||
                        list_btn_del.Where(r => r.id == rec.btn_id).Count() > 0).ToList();
                    if (list_rle_del != null && list_rle_del.Count > 0)
                    {
                        db.xfund_t_pf_sys_button_right.RemoveRange(list_rle_del);
                    }

                    db.xfund_t_pf_sys_menu.RemoveRange(list);
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
        public JsonResult GetMenuButton(int menu_id)
        {
            List<xfund_t_pf_sys_buttonObj> btn_ary = null;
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);

            btn_ary = db.xfund_t_pf_sys_button.Where(rec => rec.menu_id == menu_id).OrderBy(rec => rec.sort).ToList();

            if (btn_ary == null)
            {
                btn_ary = new List<xfund_t_pf_sys_buttonObj>();
            }

            db.Dispose();
            return Json(btn_ary);
        }

        /// <summary>
        /// 添加菜单按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddButton(xfund_t_pf_sys_buttonObj mdl)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            db.xfund_t_pf_sys_button.Add(mdl);
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
        public JsonResult UpdateButton(xfund_t_pf_sys_buttonObj mdl)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_buttonObj> list = db.xfund_t_pf_sys_button.Where(rec => rec.id == mdl.id).ToList();
            if (list != null && list.Count == 1)
            {
                list[0].btn_js_id = mdl.btn_js_id;
                list[0].btn_name = mdl.btn_name;
                list[0].menu_id = mdl.menu_id;
                list[0].sort = mdl.sort;

                db.xfund_t_pf_sys_button.Update(list[0]);
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
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            if (id > 0) // 一级菜单的parent_id为0，所以禁止删除所有一级菜单
            {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                List<xfund_t_pf_sys_buttonObj> list = db.xfund_t_pf_sys_button.Where(rec => rec.id == id).ToList();
                if (list != null && list.Count == 1)
                {
                    db.xfund_t_pf_sys_button.Remove(list[0]);
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

        /// <summary>
        /// 角色管理（私）
        /// </summary>
        /// <returns></returns>
        public IActionResult RoleManager()
        {
            return View();
        }

        /// <summary>
        /// 不分页获取全部角色信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRoleToList(xfund_t_pf_sys_roleSelObj sel)
        {
            List<object> role_ary = new List<object>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_roleObj> list = db.xfund_t_pf_sys_role.Where(
                rec => (string.IsNullOrWhiteSpace(sel.name_like) ? true : rec.role.Contains(sel.name_like))
            ).ToList();

            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    role_ary.Add(new
                    {
                        id = list[i].id,
                        role = list[i].role,
                        rolecode = list[i].rolecode,
                        status = list[i].status,
                        data_authority = list[i].data_authority
                    });
                }
            }

            db.Dispose();
            return Json(role_ary);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddRole(xfund_t_pf_sys_roleObj mdl)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            db.xfund_t_pf_sys_role.Add(mdl);
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
        public JsonResult UpdateRole(xfund_t_pf_sys_roleObj mdl)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_roleObj> list = db.xfund_t_pf_sys_role.Where(rec => rec.id == mdl.id).ToList();
            if (list != null && list.Count == 1)
            {
                list[0].role = mdl.role;
                list[0].rolecode = mdl.rolecode;
                list[0].status = mdl.status;
                list[0].data_authority = mdl.data_authority;

                db.xfund_t_pf_sys_role.Update(list[0]);
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
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_roleObj> list = db.xfund_t_pf_sys_role.Where(rec => rec.id == id).ToList();
            List<xfund_t_pf_sys_role_userObj> list_rel_user =
                db.xfund_t_pf_sys_role_user.Where(rec => rec.roleid == id).ToList();
            List<xfund_t_pf_sys_button_rightObj> list_rel_menu =
                db.xfund_t_pf_sys_button_right.Where(rec => rec.role_id == id).ToList();

            if (list != null && list.Count == 1)
            {
                // 删除角色菜单关系
                if (list_rel_menu != null && list_rel_menu.Count > 0)
                {
                    db.xfund_t_pf_sys_button_right.RemoveRange(list_rel_menu);
                }

                // 删除角色和用户关系
                if (list_rel_user != null && list_rel_user.Count > 0)
                {
                    db.xfund_t_pf_sys_role_user.UpdateRange(list_rel_user);
                }

                db.xfund_t_pf_sys_role.Remove(list[0]);
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
            List<TreeDataObj> tree_data = new List<TreeDataObj>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var query1 = db.MenuTreeData.FromSql(
                @"SELECT CONCAT('m_', a.id) AS 'id',a.id AS 'key',a.classes AS 'parent_id',a.menu AS 'text',a.menu_level AS 'level',(SELECT CASE WHEN COUNT(0) > 0 THEN 'true' ELSE 'false' END FROM t_pf_sys_button_right r WHERE r.role_id = " +
                id +
                " AND r.menu_id = a.id ) AS 'check', 'menu' AS 'type', a.seq AS 'sort','' AS 'iconCls' FROM t_pf_sys_menu a ");
            List<MenuTreeDataObj> list1 = query1.ToList();
            var query2 = db.MenuTreeData.FromSql(
                @"SELECT CONCAT('b_', a.id) AS 'id', a.id AS 'key', a.menu_id AS 'parent_id', a.btn_name AS 'text', NULL AS 'level', ( SELECT CASE WHEN COUNT(0) > 0 THEN 'true' ELSE 'false' END FROM t_pf_sys_button_right r WHERE r.role_id = " +
                id +
                " AND r.btn_id = a.id ) AS 'check', 'button' AS 'type', a.sort AS 'sort','' AS 'iconCls' FROM t_pf_sys_button a ");
            List<MenuTreeDataObj> list2 = query2.ToList();
            List<MenuTreeDataObj> list = new List<MenuTreeDataObj>();
            if (list1 != null)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    list1[i].parent_id = list1[i].parent_id != null
                        ? list1[i].parent_id.Replace("." + list1[i].key, "")
                        : null;
                    if (list1[i].parent_id == list1[i].key.ToString())
                    {
                        list1[i].parent_id = "";
                    }
                }
            }

            list.AddRange(list1); // EF使用UNION ALL 如果ID重复则数据会出现错误
            list.AddRange(list2);

            if (list != null && list.Count > 0)
            {
                List<MenuTreeDataObj> root = list.Where(rec => rec.level == 1 && rec.type == "menu")
                    .OrderBy(rec => rec.sort).ToList();
                if (root != null && root.Count > 0)
                {
                    for (int i = 0; i < root.Count; i++)
                    {
                        TreeDataObj r = new TreeDataObj();
                        r.id = root[i].type + "_" + root[i].key;
                        r.text = root[i].text;
                        r.state = "";
                        r.check = root[i].check;
                        r.iconCls = root[i].iconCls;
                        r.attributes = root[i].parent_id != null ? root[i].parent_id.ToString() : "";
                        r.key = root[i].key;
                        r.children = list.Where(rec => rec.parent_id == root[i].key.ToString()).Select(rec =>
                            new TreeDataObj() // 二级菜单及一级按钮
                            {
                                id = rec.type + "_" + rec.key,
                                text = rec.text,
                                state = "",
                                check = rec.check,
                                iconCls = rec.iconCls,
                                key = rec.key,
                                attributes = rec.parent_id.ToString()
                            }).ToList();

                        if (r.children != null && r.children.Count > 0)
                        {
                            for (int j = 0; j < r.children.Count; j++)
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

                        tree_data.Add(r);
                    }
                }
            }

            db.Dispose();
            return Json(tree_data);
        }

        /// <summary>
        /// 根据角色ID分配权限 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DistributionRight(int role_id, List<xfund_t_pf_sys_button_rightObj> list)
        {
            ResultObj ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            List<xfund_t_pf_sys_button_rightObj> list_del_rel =
                db.xfund_t_pf_sys_button_right.Where(rec => rec.role_id == role_id).ToList();

            if (list_del_rel != null && list_del_rel.Count > 0)
            {
                db.xfund_t_pf_sys_button_right.RemoveRange(list_del_rel);
            }

            if (list != null && list.Count > 0)
            {
                db.xfund_t_pf_sys_button_right.AddRange(list);
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

        //------------------------热点投票--------------------------
        public IActionResult VoteManager()
        {
            return View();
        }

        //获取投票列表
        [HttpPost]
        public JsonResult GetVoteToList(voteSelObj sel)
        {
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.t_vote.Where(m =>
                string.IsNullOrWhiteSpace(sel.title_like) || m.vote_title.Contains(sel.title_like)).ToList();
            db.Dispose();
            return Json(list);
        }

        /// <summary>
        /// 添加投票
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddVote(voteObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            t_sys_userObj user = null;
            var cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            if (user == null)
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_Unauthorized;
                ro.msg = EResponseState.TRUMGU_IMS_Unauthorized.ToString();
                return Json(ro);
            }

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            mdl.vote_createdate = DateTime.Now;
            mdl.vote_createuser = user.id;
            db.t_vote.Add(mdl);
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
        /// 修改投票
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateVote(voteObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            t_sys_userObj user = null;
            var cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            if (user == null)
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_Unauthorized;
                ro.msg = EResponseState.TRUMGU_IMS_Unauthorized.ToString();
                return Json(ro);
            }



            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.t_vote.FirstOrDefault(m => m.id == mdl.id);
            if (model != null)
            {
                model.vote_title = mdl.vote_title;
                model.vote_content = mdl.vote_content;
                model.vote_checktype = mdl.vote_checktype;
                model.vote_createdate = DateTime.Now;
                model.vote_createuser = user.id;
                model.vote_enddate = mdl.vote_enddate;
                model.vote_startdate = mdl.vote_startdate;
                model.vote_isclosed = mdl.vote_isclosed;
                db.t_vote.Update(model);
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
        /// 删除投票
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteVote(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.t_vote.FirstOrDefault(m => m.id == id);
            if (model != null)
            {
                db.t_vote.Remove(model);
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
        /// 根据vote_id获取投票选项
        /// </summary>
        /// <param name="sel">必须传入vote_id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetVoteOptionToList(voteOptionSelObj sel)
        {
            if (sel.vote_id <= 0)
            {
                return null;
            }
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.t_voteOption.Where(m => m.vote_id == sel.vote_id).ToList();
            db.Dispose();
            return Json(list);
        }

        /// <summary>
        /// 添加选项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddVoteOption(voteOptionObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            db.t_voteOption.Add(mdl);
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
        /// 修改选项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateVoteOption(voteOptionObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.t_voteOption.FirstOrDefault(m => m.id == mdl.id);
            if (model != null)
            {
                model.option_header = mdl.option_header;
                model.option_title = mdl.option_title;
                model.vote_id = mdl.vote_id;
                db.t_voteOption.Update(model);
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
        /// 删除选项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteVoteOption(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.t_voteOption.FirstOrDefault(m => m.id == id);
            if (model != null)
            {
                db.t_voteOption.Remove(model);
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

        //----------------------------用户选项统计----------------
        public IActionResult VoteStat()
        {
            return View();
        }

        /// <summary>
        /// 获取统计列表
        /// </summary>
        /// <param name="voteId"></param>
        /// <returns></returns>
        public JsonResult GetStatToList(int voteId)
        {
            var voteStatList = new List<object>();
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            //根据用户分组
            var list = db.t_voteAnswer.Where(m => m.vote_id == voteId).GroupBy(m => m.user_id).ToList();
            foreach (var item in list)
            {
                //得到私募用户实体
                var pfuser = db.xfund_t_pf_sys_user.FirstOrDefault(m => m.id == item.Key);
                //得到用户选项ID的集合
                var optionList = item.Select(m => m.option_id).ToList();
                //创建选项字符串
                var optionStr = "";
                //获取用户投票时间
                var answerTime = DateTime.MinValue;
                // ReSharper disable once GenericEnumeratorNotDisposed
                var answerList = item.GetEnumerator();
                while (answerList.MoveNext())
                {
                    //循环用户所有投票时间
                    var answerTimeItem = answerList.Current.answer_createdate;
                    if (answerTimeItem > answerTime)
                    {
                        //得到最后的投票时间 复制给answertime
                        answerTime = answerTimeItem;
                    }
                }
                foreach (var optionItem in optionList)
                {
                    //根据选项ID 得到选项实体
                    var optionModel = db.t_voteOption.FirstOrDefault(m => m.vote_id == voteId && m.id == optionItem);
                    if (optionModel != null)
                    {
                        optionStr += optionModel.option_header + ",";
                    }

                }

                if (pfuser != null)
                {
                    //构建前端需要的list
                    voteStatList.Add(new
                    {
                        stat_user = pfuser.name,
                        stat_option = optionStr.TrimEnd(','),
                        stat_account = pfuser.userid,
                        stat_date = answerTime

                    });
                }
            }
            db.Dispose();
            return Json(voteStatList);
        }

        //----------------------用户留言管理------------------
        public IActionResult VoteLeave()
        {
            ViewBag.Select = GetVoteDropList();
            return View();
        }

        /// <summary>
        /// 获取留言页面 投票下拉框的值
        /// </summary>
        /// <returns></returns>
        private static List<SelectListItem> GetVoteDropList()
        {
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.t_vote.ToList();
            var voteList = new List<SelectListItem>();
            foreach (var voteObj in list)
            {
                var item = new SelectListItem
                {
                    Text = voteObj.vote_title,
                    Value = voteObj.id.ToString()
                };
                voteList.Add(item);
            }
            db.Dispose();
            return voteList;
        }

        //获取留言列表
        public JsonResult GetLeaveToList(voteLeaveSelObj sel)
        {
            if (sel.vote_id <= 0)
            {
                return Json(null);
            }

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.t_voteLeave.Join(
                db.xfund_t_pf_sys_user,
                m => m.user_id,
                n => n.id,
                (m, n) => new
                {
                    id = m.id,
                    comment_content = m.comment_content,
                    comment_date = m.comment_date,
                    user_id = m.user_id,
                    vote_id = m.vote_id,
                    user_name = n.name
                }).ToList();
            list = list.Where(m => m.vote_id == sel.vote_id).ToList();
            if (sel.key != null)
            {
                list = list.Where(m => m.comment_content.Contains(sel.key)).ToList();
            }
            db.Dispose();
            return Json(list);
        }

        public JsonResult DeleteLeave(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.t_voteLeave.FirstOrDefault(m => m.id == id);
            if (model != null)
            {
                db.t_voteLeave.Remove(model);
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

        //----------------------Banner图列表-----------------
        public IActionResult Banner()
        {
            return View();
        }

        /// <summary>
        /// 获取图片列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBannerToList()
        {
            var resultList = new List<t_pf_bannerEXObj>();
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var list = db.t_pf_banner.OrderBy(m => m.banner_sort).ToList();
            foreach (var item in list)
            {
                var model = new t_pf_bannerEXObj
                {
                    id = item.id,
                    banner_link = item.banner_link,
                    banner_sort = item.banner_sort,
                    banner_target = item.banner_target,
                    banner_title = item.banner_title,
                    banner_url = item.banner_url,
                    is_enable = item.is_enable,
                    create_userid = item.create_userid,
                    create_time = item.create_time,
                    modify_userid = item.modify_userid,
                    modify_time = item.modify_time,
                    web_banner_url = string.IsNullOrWhiteSpace(item.banner_url)
                        ?
                        string.Empty
                        : 
                        ConfigConstantHelper.PFWebUrl+ item.banner_url
                };
                resultList.Add(model);
            }
            return Json(resultList);
        }


        /// <summary>
        /// 添加轮播图列表
        /// </summary>
        /// <returns></returns>
        public JsonResult AddBanner(t_pf_bannerObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            t_sys_userObj user = null;
            var cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            if (user == null)
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_Unauthorized;
                ro.msg = EResponseState.TRUMGU_IMS_Unauthorized.ToString();
                return Json(ro);
            }
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            mdl.create_time = DateTime.Now;
            mdl.create_userid = user.id;
            mdl.modify_time = DateTime.Now;
            mdl.modify_userid = user.id;
            db.t_pf_banner.Add(mdl);
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
        /// 修改轮播图列表
        /// </summary>
        /// <param name="mdl"></param>
        /// <returns></returns>
        public JsonResult UpdateBanner(t_pf_bannerObj mdl)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            t_sys_userObj user = null;
            var cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            if (user == null)
            {
                ro.code = (int)EResponseState.TRUMGU_IMS_Unauthorized;
                ro.msg = EResponseState.TRUMGU_IMS_Unauthorized.ToString();
                return Json(ro);
            }
            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.t_pf_banner.FirstOrDefault(m => m.id == mdl.id);
            if (model != null)
            {
                model.banner_title = mdl.banner_title;
                model.banner_sort = mdl.banner_sort;
                model.banner_target = mdl.banner_target;
                model.banner_link = mdl.banner_link;
                model.is_enable = mdl.is_enable;
                model.modify_userid = user.id;
                model.modify_time = DateTime.Now;
                db.t_pf_banner.Update(model);
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
        /// 删除轮播图列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteBanner(int id)
        {
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };

            var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
            var model = db.t_pf_banner.FirstOrDefault(m => m.id == id);
            if (model != null)
            {
                db.t_pf_banner.Remove(model);
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


        //--------------------上传banner轮播图----------------------

        public PartialViewResult BanneView()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult AddBannerImg(IFormCollection files)
        {
            var id = Convert.ToInt32(files["uid"].ToString());
            var file = Request.Form.Files[0];
            
            var ro = new ResultObj()
            {
                code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL,
                msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString()
            };
            if (id > 0 && file != null)
            {
                var savePath = ConfigConstantHelper.PFUploadBannerRootPath;
                var uploadState = ImageUpload.Upload(file, savePath, out var imgPath);
                if (uploadState)
                {
                    var db = DBHelper.CreateContext(ConfigConstantHelper.fund_connstr);
                    var model = db.t_pf_banner.FirstOrDefault(m => m.id == id);
                    if (model == null)
                    {
                        ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                        ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
                        return Json(ro);
                    }
                    var index = imgPath.IndexOf(@"\Content\", StringComparison.Ordinal);
                    imgPath = imgPath.Substring(index, imgPath.Length - index);
                    imgPath = imgPath.Replace('\\','/');
                    model.banner_url = imgPath;
                    db.t_pf_banner.Update(model);
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
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_SAVE;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString();
                }
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