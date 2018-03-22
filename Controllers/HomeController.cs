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
        [NoFilterAttribute]
        public IActionResult Index()
        {
            HttpContext.Session.SetString(key: "name", value: "123456"); 
            return View();
        }

        [HttpPost]
        public JsonResult GetMenu()
        {
            List<object> menu_ary = new List<object>();
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext();
            List<t_sys_menuObj> list = (from e in db.t_sys_menu
                                        where e.state == 1 && e.is_delete == 0
                                        select new t_sys_menuObj
                                        {
                                            id = e.id,
                                            name = e.name,
                                            path = e.path,
                                            icon = e.icon,
                                            level = e.level,
                                            sort = e.sort,
                                            parent_id = e.parent_id
                                        }).ToList();
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
    }
}