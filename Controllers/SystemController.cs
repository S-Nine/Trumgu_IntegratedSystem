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

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class SystemController : Controller
    {
        /// <summary>
        /// 菜单页面初始化
        /// <summary>
        /// <returns></returns>
        public IActionResult Menu()
        {
            return View();
        }

        /// <summary>
        /// 不分页获取全部菜单信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenuToList()
        {
            List<object> menu_ary = new List<object>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_menuObj> list = db.t_sys_menu.Where(rec => rec.is_delete == 0).ToList();
            List<t_sys_menuObj> root_ary = list.Where(rec => rec.level == 1).ToList();
            if (root_ary != null && root_ary.Count > 0)
            {
                root_ary = root_ary.OrderBy(rec => rec.sort).ToList();
                for (int i = 0; i < root_ary.Count; i++)
                {
                    menu_ary.Add(new
                    {
                        id = root_ary[i].id,
                        name = root_ary[i].name,
                        path = root_ary[i].path,
                        icon = root_ary[i].icon,
                        level = root_ary[i].level,
                        sort = root_ary[i].sort,
                        parent_id = root_ary[i].parent_id,
                        btn_state = root_ary[i].state,
                        is_delete = root_ary[i].is_delete,
                        last_modify_id = root_ary[i].last_modify_id,
                        last_modify_name = root_ary[i].last_modify_name,
                        last_modify_time = root_ary[i].last_modify_time,
                        create_time = root_ary[i].create_time,
                        children = list.Where(rec => rec.parent_id == root_ary[i].id).OrderBy(rec => rec.sort).Select(rec => new
                        {
                            id = rec.id,
                            name = rec.name,
                            path = rec.path,
                            icon = rec.icon,
                            level = rec.level,
                            sort = rec.sort,
                            parent_id = rec.parent_id,
                            btn_state = rec.state,
                            is_delete = rec.is_delete,
                            last_modify_id = rec.last_modify_id,
                            last_modify_name = rec.last_modify_name,
                            last_modify_time = rec.last_modify_time,
                            create_time = rec.create_time
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
            List<object> menu_ary = new List<object>() { new { id = 0, text = "顶级菜单" } };
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_menuObj> list = db.t_sys_menu.Where(rec => rec.is_delete == 0 && rec.level == 1).OrderBy(rec => rec.sort).Select(rec => new t_sys_menuObj()
            {
                id = rec.id,
                name = rec.name
            }).ToList();
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    menu_ary.Add(new
                    {
                        id = list[i].id,
                        text = list[i].name
                    });
                }
            }
            db.Dispose();
            return Json(menu_ary);
        }

        /// <summary>
        /// 不分页获取指定菜单id的按钮列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenuButton(int menu_id)
        {
            List<t_sys_buttonObj> btn_ary = null;
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();

            btn_ary = db.t_sys_button.Where(rec => rec.menu_id == menu_id && rec.is_delete == 0).OrderBy(rec => rec.btn_sort).ToList();

            if (btn_ary == null)
            {
                btn_ary = new List<t_sys_buttonObj>();
            }
            db.Dispose();
            return Json(btn_ary);
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddMenu(t_sys_menuObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            mdl.is_delete = 0;
            mdl.create_time = DateTime.Now;
            mdl.last_modify_time = mdl.create_time;
            mdl.last_modify_id = user.id.ToString();
            mdl.last_modify_name = user.name;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            db.t_sys_menu.Add(mdl);
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
        /// 修改菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateMenu(t_sys_menuObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_menuObj> list = db.t_sys_menu.Where(rec => rec.id == mdl.id && rec.is_delete == 0).ToList();
            if (list != null && list.Count == 1)
            {
                list[0].name = mdl.name;
                list[0].path = mdl.path;
                list[0].icon = mdl.icon;
                list[0].level = mdl.level;
                list[0].sort = mdl.sort;
                list[0].parent_id = mdl.parent_id;
                list[0].state = mdl.state;
                list[0].last_modify_time = DateTime.Now;
                list[0].last_modify_id = user.id.ToString();
                list[0].last_modify_name = user.name;

                db.t_sys_menu.Update(list[0]);
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
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            if (id > 0) // 一级菜单的parent_id为0，所以禁止删除所有一级菜单
            {
                string cUserInfo = HttpContext.Session.GetString("UserInfo");
                if (!string.IsNullOrWhiteSpace(cUserInfo))
                {
                    user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
                }

                Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
                List<t_sys_menuObj> list = db.t_sys_menu.Where(rec => (rec.id == id || rec.parent_id == id) && rec.is_delete == 0).ToList();
                List<t_sys_relation_role_menu_buttonObj> list_rel_menu = db.t_sys_relation_role_menu_button.Where(rec => rec.menu_id == id && rec.is_delete == 0).ToList();

                if (list != null && list.Count > 0)
                {
                    DateTime dtNow = DateTime.Now;
                    List<t_sys_buttonObj> list_btn = (
                        from t in db.t_sys_button where list.Select(rec => rec.id).Contains(t.menu_id) && t.is_delete == 0 select t
                    ).ToList();
                    List<t_sys_relation_role_menu_buttonObj> list_del_rel_role_menu_button = (
                        from t in db.t_sys_relation_role_menu_button where list.Select(rec => rec.id).Contains(t.menu_id) && t.is_delete == 0 select t
                    ).ToList();

                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].is_delete = 1;
                        list[i].last_modify_id = user.id.ToString();
                        list[i].last_modify_name = user.name;
                        list[i].last_modify_time = dtNow;
                    }
                    // 删除菜单下的
                    if (list_btn != null && list_btn.Count > 0)
                    {
                        for (int i = 0; i < list_btn.Count; i++)
                        {
                            list_btn[i].is_delete = 1;
                            list_btn[i].last_modify_id = user.id.ToString();
                            list_btn[i].last_modify_name = user.name;
                            list_btn[i].last_modify_time = dtNow;
                        }
                        db.t_sys_button.UpdateRange(list_btn);
                    }
                    // 删除菜单和角色关系
                    if (list_del_rel_role_menu_button != null && list_del_rel_role_menu_button.Count > 0)
                    {
                        for (int i = 0; i < list_del_rel_role_menu_button.Count; i++)
                        {
                            list_del_rel_role_menu_button[i].is_delete = 1;
                            list_del_rel_role_menu_button[i].last_modify_id = user.id.ToString();
                            list_del_rel_role_menu_button[i].last_modify_name = user.name;
                            list_del_rel_role_menu_button[i].last_modify_time = dtNow;
                        }
                        db.t_sys_relation_role_menu_button.UpdateRange(list_del_rel_role_menu_button);
                    }
                    db.t_sys_menu.UpdateRange(list);
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
        /// 添加菜单按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddButton(t_sys_buttonObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            mdl.is_delete = 0;
            mdl.create_time = DateTime.Now;
            mdl.last_modify_time = mdl.create_time;
            mdl.last_modify_id = user.id.ToString();
            mdl.last_modify_name = user.name;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            db.t_sys_button.Add(mdl);
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
        public JsonResult UpdateButton(t_sys_buttonObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_buttonObj> list = db.t_sys_button.Where(rec => rec.id == mdl.id && rec.is_delete == 0).ToList();
            if (list != null && list.Count == 1)
            {
                list[0].btn_code = mdl.btn_code;
                list[0].btn_name = mdl.btn_name;
                list[0].btn_img = mdl.btn_img;
                list[0].btn_sort = mdl.btn_sort;
                list[0].last_modify_time = DateTime.Now;
                list[0].last_modify_id = user.id.ToString();
                list[0].last_modify_name = user.name;

                db.t_sys_button.Update(list[0]);
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
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            if (id > 0) // 一级菜单的parent_id为0，所以禁止删除所有一级菜单
            {
                string cUserInfo = HttpContext.Session.GetString("UserInfo");
                if (!string.IsNullOrWhiteSpace(cUserInfo))
                {
                    user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
                }

                Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
                List<t_sys_buttonObj> list = db.t_sys_button.Where(rec => rec.id == id && rec.is_delete == 0).ToList();
                if (list != null && list.Count == 1)
                {
                    DateTime dtNow = DateTime.Now;
                    list[0].is_delete = 1;
                    list[0].last_modify_id = user.id.ToString();
                    list[0].last_modify_name = user.name;
                    list[0].last_modify_time = dtNow;

                    db.t_sys_button.Update(list[0]);
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
        /// 角色页面初始化
        /// <summary>
        /// <returns></returns>
        public IActionResult Role()
        {
            return View();
        }

        /// <summary>
        /// 不分页获取全部角色信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRoleToList(t_sys_roleSelObj sel)
        {
            List<object> role_ary = new List<object>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_roleObj> list = db.t_sys_role.Where(
                rec => rec.is_delete == 0 &&
                (string.IsNullOrWhiteSpace(sel.name_like) ? true : rec.name.Contains(sel.name_like))
            ).ToList();

            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    role_ary.Add(new
                    {
                        id = list[i].id,
                        name = list[i].name,
                        data_permiss = list[i].data_permiss,
                        role_state = list[i].state,
                        remarks = list[i].remarks,
                        is_delete = list[i].is_delete,
                        last_modify_id = list[i].last_modify_id,
                        last_modify_name = list[i].last_modify_name,
                        last_modify_time = list[i].last_modify_time,
                        create_time = list[i].create_time
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
        public JsonResult AddRole(t_sys_roleObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            mdl.is_delete = 0;
            mdl.create_time = DateTime.Now;
            mdl.last_modify_time = mdl.create_time;
            mdl.last_modify_id = user.id.ToString();
            mdl.last_modify_name = user.name;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            db.t_sys_role.Add(mdl);
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
        public JsonResult UpdateRole(t_sys_roleObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_roleObj> list = db.t_sys_role.Where(rec => rec.id == mdl.id && rec.is_delete == 0).ToList();
            if (list != null && list.Count == 1)
            {
                list[0].name = mdl.name;
                list[0].data_permiss = mdl.data_permiss;
                list[0].remarks = mdl.remarks;
                list[0].state = mdl.state;
                list[0].last_modify_time = DateTime.Now;
                list[0].last_modify_id = user.id.ToString();
                list[0].last_modify_name = user.name;

                db.t_sys_role.Update(list[0]);
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
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };

            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_roleObj> list = db.t_sys_role.Where(rec => rec.id == id && rec.is_delete == 0).ToList();
            List<t_sys_relation_role_menu_buttonObj> list_rel_menu = db.t_sys_relation_role_menu_button.Where(rec => rec.role_id == id && rec.is_delete == 0).ToList();
            List<t_sys_relation_role_userObj> list_rel_user = db.t_sys_relation_role_user.Where(rec => rec.role_id == id && rec.is_delete == 0).ToList();

            if (list != null && list.Count == 1)
            {
                DateTime dtNow = DateTime.Now;

                list[0].is_delete = 1;
                list[0].last_modify_id = user.id.ToString();
                list[0].last_modify_name = user.name;
                list[0].last_modify_time = dtNow;
                // 删除角色菜单关系
                if (list_rel_menu != null && list_rel_menu.Count > 0)
                {
                    for (int i = 0; i < list_rel_menu.Count; i++)
                    {
                        list_rel_menu[i].is_delete = 1;
                        list_rel_menu[i].last_modify_id = user.id.ToString();
                        list_rel_menu[i].last_modify_name = user.name;
                        list_rel_menu[i].last_modify_time = dtNow;
                    }
                    db.t_sys_relation_role_menu_button.UpdateRange(list_rel_menu);
                }
                // 删除角色和用户关系
                if (list_rel_user != null && list_rel_user.Count > 0)
                {
                    for (int i = 0; i < list_rel_user.Count; i++)
                    {
                        list_rel_user[i].is_delete = 1;
                        list_rel_user[i].last_modify_id = user.id.ToString();
                        list_rel_user[i].last_modify_name = user.name;
                        list_rel_user[i].last_modify_time = dtNow;
                    }
                    db.t_sys_relation_role_user.UpdateRange(list_rel_user);
                }
                db.t_sys_role.Update(list[0]);
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
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            var query1 = db.MenuTreeData.FromSql(@"SELECT CONCAT('m_',a.id) as 'id',a.id as 'key',a.parent_id,a.name as 'text',a.icon as 'iconCls',a.`level`,(SELECT CASE WHEN COUNT(0) > 0 THEN 'true' ELSE 'false' END FROM t_sys_relation_role_menu_button r WHERE r.role_id = " + id + " AND is_delete=0 AND r.menu_id=a.id) as 'check','menu' as 'type',a.sort as 'sort' FROM t_sys_menu a WHERE a.is_delete = 0 ");
            List<MenuTreeDataObj> list1 = query1.ToList();
            var query2 = db.MenuTreeData.FromSql(@"SELECT CONCAT('b_',a.id) as 'id',a.id as 'key',a.menu_id as 'parent_id',a.btn_name as 'text',a.btn_img as 'iconCls',null as 'level',(SELECT CASE WHEN COUNT(0) > 0 THEN 'true' ELSE 'false' END FROM t_sys_relation_role_menu_button r WHERE r.role_id = " + id + " AND is_delete=0 AND r.btn_id=a.id AND menu_id=a.menu_id) as 'check','button' as 'type',a.btn_sort as 'sort' FROM t_sys_button a WHERE a.is_delete = 0 ");
            List<MenuTreeDataObj> list2 = query2.ToList();
            List<MenuTreeDataObj> list = new List<MenuTreeDataObj>();
            list.AddRange(list1);   // EF使用UNION ALL 如果ID重复则数据会出现错误
            list.AddRange(list2);

            if (list != null && list.Count > 0)
            {
                List<MenuTreeDataObj> root = list.Where(rec => rec.parent_id == 0 && rec.type == "menu").OrderBy(rec => rec.sort).ToList();
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
                        r.attributes = root[i].parent_id.ToString();
                        r.key = root[i].key;
                        r.children = list.Where(rec => rec.parent_id == root[i].key).Select(rec => new TreeDataObj()  // 二级菜单及一级按钮
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
                                r.children[j].children = list.Where(rec => rec.parent_id == r.children[j].key).Select(rec => new TreeDataObj()  // 二级菜单及一级按钮
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
        public JsonResult DistributionRight(int role_id, List<t_sys_relation_role_menu_buttonObj> list)
        {
            DateTime dtNow = DateTime.Now;
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };

            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_relation_role_menu_buttonObj> list_del_rel = db.t_sys_relation_role_menu_button.Where(rec => rec.is_delete == 0 && rec.role_id == role_id).ToList();

            if (list_del_rel != null && list_del_rel.Count > 0)
            {
                for (int i = 0; i < list_del_rel.Count; i++)
                {
                    list_del_rel[i].is_delete = 1;
                    list_del_rel[i].last_modify_id = user.id.ToString();
                    list_del_rel[i].last_modify_name = user.name;
                    list_del_rel[i].last_modify_time = dtNow;
                }
                db.t_sys_relation_role_menu_button.UpdateRange(list_del_rel);
            }
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].is_delete = 0;
                    list[i].last_modify_id = user.id.ToString();
                    list[i].last_modify_name = user.name;
                    list[i].last_modify_time = dtNow;
                    list[i].create_time = dtNow;
                }
                db.t_sys_relation_role_menu_button.AddRange(list);
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

        /// <summary>
        /// 部门页面初始化
        /// <summary>
        /// <returns></returns>
        public IActionResult Department()
        {
            return View();
        }

        /// <summary>
        /// 不分页查询部门列表
        /// </summary>
        public JsonResult GetDepartmentToList()
        {
            List<object> department_ary = new List<object>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_delpartmentObj> list = db.t_sys_delpartment.Where(rec => rec.is_delete == 0).ToList();
            if (list != null)
            {
                List<t_sys_delpartmentObj> root_ary = list.Where(rec => rec.parent_id == 0).ToList();
                if (root_ary != null)
                {
                    for (int i = 0; i < root_ary.Count; i++)
                    {
                        department_ary.Add(new
                        {
                            id = root_ary[i].id,
                            name = root_ary[i].name,
                            parent_id = root_ary[i].parent_id,
                            is_delete = root_ary[i].is_delete,
                            last_modify_id = root_ary[i].last_modify_id,
                            last_modify_name = root_ary[i].last_modify_name,
                            last_modify_time = root_ary[i].last_modify_time,
                            create_time = root_ary[i].create_time,
                            remarks = root_ary[i].remarks,
                            children = GetChildrenDepartment((int)root_ary[i].id, list)
                        });
                    }
                }
            }
            db.Dispose();
            return Json(department_ary);
        }

        private List<object> GetChildrenDepartment(int parent_id, List<t_sys_delpartmentObj> list)
        {
            List<object> o_ary = new List<object>();
            List<t_sys_delpartmentObj> c_ary = list.Where(rec => rec.parent_id == parent_id).ToList();
            if (c_ary != null && c_ary.Count > 0)
            {
                for (int i = 0; i < c_ary.Count; i++)
                {
                    o_ary.Add(new
                    {
                        id = c_ary[i].id,
                        name = c_ary[i].name,
                        parent_id = c_ary[i].parent_id,
                        is_delete = c_ary[i].is_delete,
                        last_modify_id = c_ary[i].last_modify_id,
                        last_modify_name = c_ary[i].last_modify_name,
                        last_modify_time = c_ary[i].last_modify_time,
                        create_time = c_ary[i].create_time,
                        remarks = c_ary[i].remarks,
                        children = GetChildrenDepartment((int)c_ary[i].id, list)
                    });
                }
            }
            return o_ary;
        }

        /// <summary>
        /// 不分页获取可选上级部门列表
        /// </summary>
        public JsonResult GetRootDepartmentToList()
        {
            List<object> department_ary = new List<object>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_delpartmentObj> list = db.t_sys_delpartment.Where(rec => rec.is_delete == 0).ToList();
            if (list != null)
            {
                List<t_sys_delpartmentObj> root_ary = list.Where(rec => rec.parent_id == 0).ToList();
                if (root_ary != null)
                {
                    for (int i = 0; i < root_ary.Count; i++)
                    {
                        department_ary.Add(new
                        {
                            id = root_ary[i].id,
                            text = root_ary[i].name,
                            children = GetRootChildrenDepartment((int)root_ary[i].id, list)
                        });
                    }
                }
            }
            db.Dispose();
            return Json(new List<object>(){new
            {
                id = 0,
                text = "根",
                children = department_ary
                }
            });
        }

        private List<object> GetRootChildrenDepartment(int parent_id, List<t_sys_delpartmentObj> list)
        {
            List<object> o_ary = new List<object>();
            List<t_sys_delpartmentObj> c_ary = list.Where(rec => rec.parent_id == parent_id).ToList();
            if (c_ary != null && c_ary.Count > 0)
            {
                for (int i = 0; i < c_ary.Count; i++)
                {
                    o_ary.Add(new
                    {
                        id = c_ary[i].id,
                        text = c_ary[i].name,
                        children = GetRootChildrenDepartment((int)c_ary[i].id, list)
                    });
                }
            }
            return o_ary;
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        public JsonResult AddDepartment(t_sys_delpartmentObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            mdl.is_delete = 0;
            mdl.create_time = DateTime.Now;
            mdl.last_modify_time = mdl.create_time;
            mdl.last_modify_id = user.id.ToString();
            mdl.last_modify_name = user.name;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            if (mdl.parent_id == 0 || db.t_sys_delpartment.Where(rec => rec.is_delete == 0 && rec.id == mdl.parent_id).Count() > 0)
            {
                db.t_sys_delpartment.Add(mdl);
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
        /// 修改部门
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateDepartment(t_sys_delpartmentObj mdl)
        {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_delpartmentObj> list = db.t_sys_delpartment.Where(rec => rec.id == mdl.id && rec.is_delete == 0).ToList();
            if (list != null && list.Count == 1)
            {
                if (mdl.parent_id == 0 || db.t_sys_delpartment.Where(rec => rec.is_delete == 0 && rec.id == mdl.parent_id).Count() > 0)
                {
                    list[0].parent_id = mdl.parent_id;
                    list[0].name = mdl.name;
                    list[0].remarks = mdl.remarks;
                    list[0].last_modify_time = DateTime.Now;
                    list[0].last_modify_id = user.id.ToString();
                    list[0].last_modify_name = user.name;

                    db.t_sys_delpartment.Update(list[0]);
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
                ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString();
            }
            db.Dispose();
            return Json(ro);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteDepartment(int id)
        {
            t_sys_userObj user = null;
            DateTime dtNow = DateTime.Now;
            ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString() };
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_delpartmentObj> list = db.t_sys_delpartment.Where(rec => rec.id == id && rec.is_delete == 0).ToList();
            int isubCount = db.t_sys_delpartment.Where(rec => rec.parent_id == id && rec.is_delete == 0).Count();

            if (list != null && list.Count == 1)
            {
                if (isubCount <= 0)
                {
                    list[0].is_delete = 1;
                    list[0].last_modify_id = user.id.ToString();
                    list[0].last_modify_name = user.name;
                    list[0].last_modify_time = dtNow;

                    List<t_sys_relation_delpartment_userObj> rel_ary = db.t_sys_relation_delpartment_user.Where(rec => rec.is_delete == 0 && rec.delpartment_id == id).ToList();
                    if (rel_ary != null && rel_ary.Count > 0)
                    {
                        for (int i = 0; i < rel_ary.Count; i++)
                        {
                            rel_ary[i].is_delete = 0;
                            rel_ary[i].last_modify_id = user.id.ToString();
                            rel_ary[i].last_modify_name = user.name;
                            rel_ary[i].last_modify_time = dtNow;
                        }
                        db.t_sys_relation_delpartment_user.UpdateRange(rel_ary);
                    }
                    db.t_sys_delpartment.Update(list[0]);

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
                    ro.code = (int)EResponseState.TRUMGU_IMS_ERROR_FORMAT;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_FORMAT.ToString();
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