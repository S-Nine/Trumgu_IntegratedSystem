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

        public IActionResult A()
        {
            return View();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public JsonResult UploadFile()
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
                    filename = hostingEnv.WebRootPath + "\\upload\\" + $@"\{filename}";
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

        [HttpPost]
        public async Task<ActionResult> UploadSliceFile()
        {
            var data = Request.Form.Files["data"];
            string lastModified = Request.Form["lastModified"].ToString();
            var total = Request.Form["total"].ToString();
            var fileName = Request.Form["fileName"].ToString();
            var index = Request.Form["index"].ToString();
            string file_url = Guid.NewGuid().ToString();
            Dictionary<string, object> result = new Dictionary<string, object>();

            string temporary = Path.Combine(hostingEnv.WebRootPath + "\\upload\\", lastModified); //临时保存分块的目录
            try
            {
                if (!Directory.Exists(temporary))
                    Directory.CreateDirectory(temporary);
                string filePath = Path.Combine(temporary, index.ToString());
                if (!Convert.IsDBNull(data))
                {
                    await Task.Run(() =>
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        data.CopyTo(fs);
                        fs.Flush();
                        fs.Dispose();
                        fs.Close();
                    });
                }
                bool mergeOk = false;

                if (total == index)
                {
                    file_url = DateTime.Now.ToString("yyyyMM") + "\\\\" + Guid.NewGuid().ToString() + (fileName.LastIndexOf('.') >= 0 ? fileName.Substring(fileName.LastIndexOf('.')) : "");
                    mergeOk = await FileMerge(lastModified, file_url);
                    result.Add("fileUrl", file_url);
                }

                result.Add("number", index);
                result.Add("mergeOk", mergeOk);
                return Json(result);

            }
            catch (Exception ex)
            {
                Directory.Delete(temporary); //删除文件夹
                throw ex;
            }
        }

        public async Task<bool> FileMerge(string lastModified, string fileName)
        {
            bool ok = false;
            try
            {
                var temporary = Path.Combine(hostingEnv.WebRootPath + "\\upload\\", lastModified); //临时文件夹
                var files = Directory.GetFiles(temporary); //获得下面的所有文件
                var finalPath = Path.Combine(hostingEnv.WebRootPath + "\\upload\\", fileName); //最终的文件名（demo中保存的是它上传时候的文件名，实际操作肯定不能这样）
                var parentPath = finalPath.Substring(0, finalPath.LastIndexOf("\\"));
                if (!Directory.Exists(parentPath))
                {
                    Directory.CreateDirectory(parentPath);
                }
                var fs = new FileStream(finalPath, FileMode.Create);
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x)) //排一下序，保证从0-N Write
                {
                    var bytes = System.IO.File.ReadAllBytes(part);
                    await fs.WriteAsync(bytes, 0, bytes.Length);
                    bytes = null;
                    System.IO.File.Delete(part); //删除分块
                }
                fs.Flush();
                fs.Dispose();
                fs.Close();
                Directory.Delete(temporary); //删除文件夹
                ok = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ok;
        }

        public IActionResult DownloadFile(string filePath)
        {
            FileStream stream = null;
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                filePath = hostingEnv.WebRootPath + "\\upload\\" + filePath;
                if (System.IO.File.Exists(filePath))
                {
                    stream = System.IO.File.OpenRead(filePath);
                }
            }
            if (stream == null)
            {
                 HttpContext.Response.Redirect("/Error/Error404?t=" + DateTime.Now.ToFileTimeUtc() , true);
                 return View();
            }
            else
            {
                return File(stream, "application/octet-stream", Path.GetFileName(filePath));
            }
        }
    }
}