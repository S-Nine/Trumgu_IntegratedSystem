using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trumgu_IntegratedManageSystem.Attributes;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 框架页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            t_sys_userObj user = null;
            List<t_sys_delpartmentObj> del_ary = null;
            string cDelNames = "";
            string cUserInfo = HttpContext.Session.GetString("UserInfo");
            string cDelpartmentInfo = HttpContext.Session.GetString("DelpartmentInfo");
            if (!string.IsNullOrWhiteSpace(cUserInfo))
            {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj>(cUserInfo);
            }
            if (!string.IsNullOrWhiteSpace(cDelpartmentInfo))
            {
                del_ary = Newtonsoft.Json.JsonConvert.DeserializeObject<List<t_sys_delpartmentObj>>(cDelpartmentInfo);
                if (del_ary != null && del_ary.Count > 0)
                {
                    for (int i = 0; i < del_ary.Count; i++)
                    {
                        cDelNames += (i != 0 && i != del_ary.Count - 1) ? "，"+del_ary[i].name : del_ary[i].name;
                    }
                }
            }
            ViewData["name"] = user != null ? user.name : "";
            ViewData["departments"] = cDelNames;
            return View();
        }

        /// <summary>
        /// 欢迎页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Welcome()
        {
            return View();
        }

        /// <summary>
        /// 获取用户权限菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenu()
        {
            List<object> menu_ary = new List<Object>();
            List<t_sys_menuObj> list = null;

            string cMenuInfo = HttpContext.Session.GetString("MenuInfo");
            if (!string.IsNullOrWhiteSpace(cMenuInfo))
            {
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<t_sys_menuObj>>(cMenuInfo);
            }

            if (list != null && list.Count > 0)
            {
                var temp = list.Where(rec => rec.level == 1).OrderBy(o => o.sort).ToList();
                for (int i = 0; i < temp.Count(); i++)
                {
                    menu_ary.Add(new
                    {
                        id = temp[i].id,
                        name = temp[i].name,
                        path = temp[i].path,
                        icon = temp[i].icon,
                        level = temp[i].level,
                        sort = temp[i].sort,
                        children = list.Where(rec => rec.parent_id == temp[i].id).OrderBy(o => o.sort).ToList()
                    });
                }
            }

            return Json(menu_ary);
        }

        /// <summary>
        /// 根据菜单id获取权限按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMenuButton(int menu_id)
        {
            List<t_sys_buttonObj> button_ary = null;
            List<t_sys_buttonObj> list = null;

            string cButtonInfo = HttpContext.Session.GetString("ButtonInfo");
            if (!string.IsNullOrWhiteSpace(cButtonInfo))
            {
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<t_sys_buttonObj>>(cButtonInfo);
            }
            if (list != null && list.Count > 0)
            {
                button_ary = list.Where(rec => rec.menu_id == menu_id).ToList();
            }
            else
            {
                button_ary = new List<t_sys_buttonObj>();
            }

            return Json(button_ary.OrderBy(rec => rec.btn_sort));
        }
    }
}