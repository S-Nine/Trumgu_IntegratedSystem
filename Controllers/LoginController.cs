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
    public class LoginController : Controller {
        public IActionResult Index () {
            // string n = "administrator";
            // string p = AESHelper.EncryptText("123@abc");

            // try
            // {
            //     Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            //     // 用户信息
            //     List<t_sys_userObj> user_ary = (
            //         from t in db.t_sys_user
            //         where t.login_name == n && t.login_pwd == p && t.is_delete == 0
            //         select t
            //     ).ToList();
            //     // 所属部门信息
            //     List<t_sys_delpartmentObj> del_ary = (
            //         from t in db.t_sys_relation_delpartment_user
            //         join t_del in db.t_sys_delpartment
            //         on t.delpartment_id equals t_del.id
            //         where t.user_id == user_ary[0].id && t.is_delete == 0 && t_del.is_delete == 0
            //         select t_del
            //     ).ToList();
            //     // 所属角色信息
            //     List<t_sys_roleObj> role_ary = (
            //         from t in db.t_sys_relation_role_user
            //         join t_role in db.t_sys_role
            //         on t.role_id equals t_role.id
            //         where t.user_id == user_ary[0].id && t.is_delete == 0 && t_role.is_delete == 0
            //         select t_role
            //     ).ToList();
            //     // 所属角色菜单信息
            //     List<t_sys_menuObj> menu_ary = (
            //         from t in db.t_sys_relation_role_user
            //         join t_role in db.t_sys_role
            //         on t.role_id equals t_role.id
            //         join t_rmb in db.t_sys_relation_role_menu_button
            //         on t.role_id equals t_rmb.role_id
            //         join t_menu in db.t_sys_menu
            //         on t_rmb.menu_id equals t_menu.id
            //         where t.user_id == user_ary[0].id && t.is_delete == 0 && t_role.is_delete == 0 && t_rmb.is_delete == 0 && t_menu.is_delete == 0 && t_menu.state == 1
            //         select t_menu
            //     ).Distinct().ToList();
            //     // 所属角色菜单功能按钮信息
            //     List<t_sys_buttonObj> btn_ary = (
            //         from t in db.t_sys_relation_role_user
            //         join t_role in db.t_sys_role
            //         on t.role_id equals t_role.id
            //         join t_rmb in db.t_sys_relation_role_menu_button
            //         on t.role_id equals t_rmb.role_id
            //         join t_btn in db.t_sys_button
            //         on t_rmb.btn_id equals t_btn.id
            //         where t.user_id == user_ary[0].id && t.is_delete == 0 && t_role.is_delete == 0 && t_rmb.is_delete == 0 && t_btn.is_delete == 0
            //         select t_btn
            //     ).Distinct().ToList();

            //     // 登录信息存入Session
            //     HttpContext.Session.SetString("UserInfo", Newtonsoft.Json.JsonConvert.SerializeObject(user_ary[0]));    // 用户信息 1-1
            //     HttpContext.Session.SetString("DelpartmentInfo", Newtonsoft.Json.JsonConvert.SerializeObject(del_ary)); // 部门信息 1-*
            //     HttpContext.Session.SetString("RoleInfo", Newtonsoft.Json.JsonConvert.SerializeObject(role_ary)); // 角色信息 1-*
            //     HttpContext.Session.SetString("MenuInfo", Newtonsoft.Json.JsonConvert.SerializeObject(menu_ary)); // 用户菜单信息 1-*
            //     HttpContext.Session.SetString("ButtonInfo", Newtonsoft.Json.JsonConvert.SerializeObject(btn_ary)); // 用户按钮信息 1-*
            //     db.Dispose();
            //     HttpContext.Response.Redirect("/Home/Index?t=" + DateTime.Now.ToFileTimeUtc(), true);
            // }
            // catch (Exception ex)
            // {
            //     string s = ex.Message;
            // }
            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                HttpContext.Response.Redirect ("/Home/Index?t=" + DateTime.Now.ToFileTimeUtc (), true);
            }
            return View ();
        }

        /// <summary>
        /// 验证登录信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult VerificationLogin (string n, string p, string c) {
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };
            if (!string.IsNullOrWhiteSpace (n) && !string.IsNullOrWhiteSpace (p) && !string.IsNullOrWhiteSpace (c)) {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
                string pwd = AESHelper.EncryptText (p);
                string VCode = HttpContext.Session.GetString ("VCode");
                if (VCode != null && c != null && VCode.ToLower () == c.Trim ().ToLower ()) {
                    List<t_sys_userObj> user_ary = (
                        from t in db.t_sys_user where t.login_name == n && t.login_pwd == pwd && t.is_delete == 0 select t
                    ).ToList ();
                    if (user_ary != null && user_ary.Count == 1) {
                        if (user_ary[0].state != 1) {
                            ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                            ro.msg = "账号已禁用、请联系系统管理员！";
                            return Json (ro);
                        }
                        // 所属部门信息
                        List<t_sys_delpartmentObj> del_ary = (
                            from t in db.t_sys_relation_delpartment_user join t_del in db.t_sys_delpartment on t.delpartment_id equals t_del.id where t.user_id == user_ary[0].id && t.is_delete == 0 && t_del.is_delete == 0 select t_del
                        ).ToList ();
                        // 所属角色信息
                        List<t_sys_roleObj> role_ary = (
                            from t in db.t_sys_relation_role_user join t_role in db.t_sys_role on t.role_id equals t_role.id where t.user_id == user_ary[0].id && t.is_delete == 0 && t_role.is_delete == 0 && t_role.state == 1 select t_role
                        ).ToList ();
                        // 所属角色菜单信息
                        List<t_sys_menuObj> menu_ary = (
                            from t in db.t_sys_relation_role_user join t_role in db.t_sys_role on t.role_id equals t_role.id join t_rmb in db.t_sys_relation_role_menu_button on t.role_id equals t_rmb.role_id join t_menu in db.t_sys_menu on t_rmb.menu_id equals t_menu.id where t.user_id == user_ary[0].id && t.is_delete == 0 && t_role.is_delete == 0 && t_rmb.is_delete == 0 && t_menu.is_delete == 0 && t_menu.state == 1 && t_role.state == 1 select t_menu
                        ).Distinct ().ToList ();
                        // 所属角色菜单功能按钮信息
                        List<t_sys_buttonObj> btn_ary = (
                            from t in db.t_sys_relation_role_user join t_role in db.t_sys_role on t.role_id equals t_role.id join t_rmb in db.t_sys_relation_role_menu_button on t.role_id equals t_rmb.role_id join t_btn in db.t_sys_button on t_rmb.btn_id equals t_btn.id where t.user_id == user_ary[0].id && t.is_delete == 0 && t_role.is_delete == 0 && t_rmb.is_delete == 0 && t_btn.is_delete == 0 && t_role.state == 1 select t_btn
                        ).Distinct ().ToList ();

                        // 登录信息存入Session
                        HttpContext.Session.SetString ("UserInfo", Newtonsoft.Json.JsonConvert.SerializeObject (user_ary[0])); // 用户信息 1-1
                        HttpContext.Session.SetString ("DelpartmentInfo", Newtonsoft.Json.JsonConvert.SerializeObject (del_ary)); // 部门信息 1-*
                        HttpContext.Session.SetString ("RoleInfo", Newtonsoft.Json.JsonConvert.SerializeObject (role_ary)); // 角色信息 1-*
                        HttpContext.Session.SetString ("MenuInfo", Newtonsoft.Json.JsonConvert.SerializeObject (menu_ary)); // 用户菜单信息 1-*
                        HttpContext.Session.SetString ("ButtonInfo", Newtonsoft.Json.JsonConvert.SerializeObject (btn_ary)); // 用户按钮信息 1-*
                        db.Dispose ();

                        ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                        ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
                    } else {
                        ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                        ro.msg = "账号或密码错误！";
                    }
                } else {
                    ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                    ro.msg = "验证码输入错误！";
                }
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString ();
            }
            return Json (ro);
        }

        public IActionResult GetVerificationCode () {
            string vcode = "";
            MemoryStream ms = VerificationCodeHelper.Create (out vcode, 4);
            HttpContext.Session.SetString ("VCode", vcode);
            return File (ms.ToArray (), @"image/png");
        }
    }
}