using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Trumgu_IntegratedManageSystem.Models;

namespace Trumgu_IntegratedManageSystem.Filters
{
    public class ActionFilter : IActionFilter
    {
        /// <summary>
        /// 免验证Controller列表
        /// </summary>
        private List<string> No_Verification_Controller = new List<string>() { "Error", "Login" };
        /// <summary>
        /// 免验证Action列表（格式：ControllerName/ActionName）
        /// </summary>
        private List<string> No_Verification_Action = new List<string>() { };

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            // throw new NotImplementedException();
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                string controllerName = context.ActionDescriptor.RouteValues["controller"];
                string actionName = context.ActionDescriptor.RouteValues["action"];

                #region 免验证Controller
                if (!string.IsNullOrWhiteSpace(controllerName) && No_Verification_Controller.Where(rec => rec.ToLower() == controllerName.ToLower()).Count() > 0)
                {
                    return;
                }
                #endregion

                #region 免验证Action
                if (!string.IsNullOrWhiteSpace(controllerName) && !string.IsNullOrWhiteSpace(actionName) && No_Verification_Action.Where(rec => rec.ToLower() == "/" + controllerName.ToLower() + "/" + actionName.ToLower()).Count() > 0)
                {
                    return;
                }
                #endregion
                string cUserInfo = context.HttpContext.Session.GetString("UserInfo");
                if (string.IsNullOrWhiteSpace(cUserInfo))
                {
                    throw new Exception("用户身份验证失败！");
                }
            }
            catch (Exception ex)
            {
                // 判断是否为ajax
                bool isAjax = false;
                var dic = context.HttpContext.Request.Headers;
                foreach (var o in dic)
                {
                    if (o.Key == "X-Requested-With")
                    {
                        var v = o.Value[0];
                        for (int i = 0; i < v.Length; i++)
                        {
                            if (v[i].ToString().ToLower() == "XMLHttpRequest".ToLower())
                            {
                                isAjax = true;
                            }
                        }
                    }
                }

                if (isAjax)
                {
                    context.HttpContext.Response.ContentType = "text/html";
                    ResultObj ro = new ResultObj() { code = (int)EResponseState.TRUMGU_IMS_Unauthorized, msg = EResponseState.TRUMGU_IMS_Unauthorized.ToString() };
                    context.HttpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(ro));
                }
                else
                {
                    context.HttpContext.Response.Redirect("/Error/Error401?t=" + DateTime.Now.ToFileTimeUtc() , true);
                }
                throw ex;
            }
        }
    }
}