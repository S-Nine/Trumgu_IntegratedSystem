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
            else
            {
                for (int i = 0; i < cipher.Count; i++)
                {
                    cipher[i].account_number = AESHelper.DecryptText(cipher[i].account_number);
                    cipher[i].account_pwd = AESHelper.DecryptText(cipher[i].account_pwd);
                }
            }
            return Json(new { total = total, rows = cipher });
        }

        /// <summary>
        /// 添加密码薄
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddCipher(t_assets_cipher_thinExObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }
            t_assets_cipher_thinObj m = new t_assets_cipher_thinObj();
            m.user_id = user.id;
            m.title = mdl.title;
            m.account_number = mdl.account_number != null ? AESHelper.EncryptText(mdl.account_number) : null;
            m.account_pwd = mdl.account_pwd != null ? AESHelper.EncryptText(mdl.account_pwd) : null;
            m.account_email = mdl.account_email;
            m.account_tel = mdl.account_tel;
            m.account_url = mdl.account_url;
            m.account_register_date = mdl.account_register_date;
            m.remarks = mdl.remarks;
            m.is_delete = 0;
            m.create_time = DateTime.Now;
            m.last_modify_time = mdl.create_time;
            m.last_modify_id = user.id.ToString();
            m.last_modify_name = user.name;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            db.t_assets_cipher_thin.Add(m);
            if (db.SaveChanges() > 0)
            {
                if (mdl.qa != null && mdl.qa.Count > 0)
                {
                    for (int i = 0; i < mdl.qa.Count; i++)
                    {
                        mdl.qa[i].order_id = m.id;
                        mdl.qa[i].is_delete = 0;
                        mdl.qa[i].create_time = mdl.create_time;
                        mdl.qa[i].last_modify_time = mdl.create_time;
                        mdl.qa[i].last_modify_id = user.id.ToString();
                        mdl.qa[i].last_modify_name = user.name;
                    }
                    db.t_assets_cipher_thin_serurity.AddRange(mdl.qa);
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
            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 根据密码薄主键获取密保
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCipherThinSecurity(int id)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            if (db.t_assets_cipher_thin.Where(rec => rec.is_delete == 0 && rec.user_id == user.id && rec.id == id).Count() == 1)
            {
                ro.data = db.t_assets_cipher_thin_serurity.Where(rec => rec.is_delete == 0 && rec.order_id == id).OrderBy(rec => rec.id).ToList();
                ro.code = (int)EResponseState.TRUMGU_IMS_SUCCESS;
                ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString();
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
        /// 根据密码薄主键获取密保
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCipherThinSecurityToList(int id)
        {
            t_sys_userObj user = null;
            List<t_assets_cipher_thin_serurityObj> list = null;
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            if (db.t_assets_cipher_thin.Where(rec => rec.is_delete == 0 && rec.user_id == user.id && rec.id == id).Count() == 1)
            {
                list = db.t_assets_cipher_thin_serurity.Where(rec => rec.is_delete == 0 && rec.order_id == id).OrderBy(rec => rec.id).ToList();
            }
            db.Dispose();

            if (list == null)
            {
                list = new List<t_assets_cipher_thin_serurityObj>();
            }
            return Json(list);
        }

        /// <summary>
        /// 修改密码薄
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCipher(t_assets_cipher_thinExObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_assets_cipher_thinObj> list = db.t_assets_cipher_thin.Where(rec => rec.is_delete == 0 && rec.user_id == user.id && rec.id == mdl.id).ToList();
            if (list != null && list.Count > 0)
            {
                list[0].title = mdl.title;
                list[0].account_number = mdl.account_number != null ? AESHelper.EncryptText(mdl.account_number) : null;
                list[0].account_pwd = mdl.account_pwd != null ? AESHelper.EncryptText(mdl.account_pwd) : null;
                list[0].account_email = mdl.account_email;
                list[0].account_tel = mdl.account_tel;
                list[0].account_url = mdl.account_url;
                list[0].account_register_date = mdl.account_register_date;
                list[0].remarks = mdl.remarks;
                list[0].last_modify_time = DateTime.Now;
                list[0].last_modify_id = user.id.ToString();
                list[0].last_modify_name = user.name;

                db.t_assets_cipher_thin.Update(list[0]);
                List<t_assets_cipher_thin_serurityObj> serurity_ary = db.t_assets_cipher_thin_serurity.Where(rec => rec.is_delete == 0 && rec.order_id == list[0].id).ToList();
                if (serurity_ary != null && serurity_ary.Count > 0)
                {
                    for (int i = 0; i < serurity_ary.Count; i++)
                    {
                        serurity_ary[i].is_delete = 1;
                        serurity_ary[i].last_modify_time = list[0].last_modify_time;
                        serurity_ary[i].last_modify_id = user.id.ToString();
                        serurity_ary[i].last_modify_name = user.name;
                    }
                    db.t_assets_cipher_thin_serurity.UpdateRange(serurity_ary);
                }
                if (mdl.qa != null && mdl.qa.Count > 0)
                {
                    for (int i = 0; i < mdl.qa.Count; i++)
                    {
                        mdl.qa[i].order_id = list[0].id;
                        mdl.qa[i].is_delete = 0;
                        mdl.qa[i].create_time = mdl.last_modify_time;
                        mdl.qa[i].last_modify_time = mdl.last_modify_time;
                        mdl.qa[i].last_modify_id = user.id.ToString();
                        mdl.qa[i].last_modify_name = user.name;
                    }
                    db.t_assets_cipher_thin_serurity.AddRange(mdl.qa);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }

            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 删除密码薄
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteCipher(int id)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_assets_cipher_thinObj> list = db.t_assets_cipher_thin.Where(rec => rec.is_delete == 0 && rec.user_id == user.id && rec.id == id).ToList();
            if (list != null && list.Count > 0)
            {
                list[0].is_delete = 1;
                list[0].last_modify_time = DateTime.Now;
                list[0].last_modify_id = user.id.ToString();
                list[0].last_modify_name = user.name;
                List<t_assets_cipher_thin_serurityObj> serurity_ary = db.t_assets_cipher_thin_serurity.Where(rec => rec.is_delete == 0 && rec.order_id == list[0].id).ToList();
                if (serurity_ary != null && serurity_ary.Count > 0)
                {
                    for (int i = 0; i < serurity_ary.Count; i++)
                    {
                        serurity_ary[i].is_delete = 1;
                        serurity_ary[i].last_modify_time = list[0].last_modify_time;
                        serurity_ary[i].last_modify_id = user.id.ToString();
                        serurity_ary[i].last_modify_name = user.name;
                    }
                    db.t_assets_cipher_thin_serurity.UpdateRange(serurity_ary);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }

            db.Dispose();
            return Json(ro);
        }
    }
}