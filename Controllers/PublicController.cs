using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trumgu_IntegratedManageSystem.Attributes;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;

namespace Trumgu_IntegratedManageSystem.Controllers
{
    public class PublicController : Controller
    {
        private IHostingEnvironment hostingEnv;

        public PublicController(IHostingEnvironment env)
        {
            this.hostingEnv = env;
        }

        public IActionResult UploadFile()
        {
            return View();
        }

        public JsonResult UploadFileStream()
        {
            var files = Request.Form.Files;
            if (files != null)
            {
                long size = 0;
                foreach (var file in files)
                {
                    var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName
                                    .Trim('"');
                    //这个hostingEnv.WebRootPath就是要存的地址可以改下
                    filename = hostingEnv.WebRootPath + $@"\{filename}";
                    size += file.Length;
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }

            return Json(new { code = 1 });
        }
    }
}