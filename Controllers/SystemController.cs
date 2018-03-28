using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            List<t_sys_menuObj> list = db.t_sys_menu.Where(rec => rec.is_delete == 0 && rec.level == 1).Select(rec => new t_sys_menuObj()
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

            btn_ary = db.t_sys_button.Where(rec => rec.menu_id == menu_id && rec.is_delete == 0).ToList();

            if (btn_ary == null)
            {
                btn_ary = new List<t_sys_buttonObj>();
            }
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
            return Json(ro);
        }
    }
}