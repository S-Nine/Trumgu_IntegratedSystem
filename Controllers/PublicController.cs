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
using Trumgu_IntegratedManageSystem.Utils;

namespace Trumgu_IntegratedManageSystem.Controllers {
    public class PublicController : Controller {
        private IHostingEnvironment hostingEnv;

        public PublicController (IHostingEnvironment env) {
            this.hostingEnv = env;
        }

        public IActionResult A () {
            return View ();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public JsonResult UploadFile () {
            var files = Request.Form.Files;
            if (files != null) {
                long size = 0;
                foreach (var file in files) {
                    var filename = ContentDispositionHeaderValue
                        .Parse (file.ContentDisposition)
                        .FileName
                        .Trim ('"');
                    //这个hostingEnv.WebRootPath就是要存的地址可以改下
                    filename = hostingEnv.WebRootPath + "\\upload\\" + $@"\{filename}";
                    size += file.Length;
                    using (FileStream fs = System.IO.File.Create (filename)) {
                        file.CopyTo (fs);
                        fs.Flush ();
                    }
                }
            }

            return Json (new { code = 1 });
        }

        /// <summary>
        /// 上传XFund文件
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> UploadXFundSliceFile () {
            var data = Request.Form.Files["data"];
            string lastModified = Request.Form["lastModified"].ToString ();
            var total = Request.Form["total"].ToString ();
            var fileName = Request.Form["fileName"].ToString ();
            var index = Request.Form["index"].ToString ();
            string file_url = Guid.NewGuid ().ToString ();
            string fileType = Request.Form["type"].ToString ();
            if (!string.IsNullOrWhiteSpace (fileType)) {
                switch (fileType) {
                    case "PrivateCompanyIntroduction":
                        fileType = "PrivateCompanyIntroduction";
                        break;
                    case "PrivateCompanyInvestigation":
                        fileType = "PrivateCompanyInvestigation";
                        break;
                }
            } else {
                fileType = "Other";
            }
            Dictionary<string, object> result = new Dictionary<string, object> ();

            string temporary = Path.Combine (ConfigConstantHelper.XFundUploadFileRootPath + fileType + "\\", lastModified); //临时保存分块的目录
            try {
                if (!Directory.Exists (temporary))
                    Directory.CreateDirectory (temporary);
                string filePath = Path.Combine (temporary, index.ToString ());
                if (!Convert.IsDBNull (data)) {
                    await Task.Run (() => {
                        if (System.IO.File.Exists (filePath)) {
                            System.IO.File.Delete (filePath);
                        }
                        FileStream fs = new FileStream (filePath, FileMode.Create);
                        data.CopyTo (fs);
                        fs.Flush ();
                        fs.Dispose ();
                        fs.Close ();
                    });
                }
                bool mergeOk = false;

                if (total == index) {
                    file_url = fileType + "\\\\" + Guid.NewGuid ().ToString () + (fileName.LastIndexOf ('.') >= 0 ? fileName.Substring (fileName.LastIndexOf ('.')) : "");
                    mergeOk = await XFundFileMerge (fileType + "\\\\" + lastModified, file_url);
                    result.Add ("fileUrl", file_url);
                }

                result.Add ("number", index);
                result.Add ("mergeOk", mergeOk);
                return Json (result);

            } catch (Exception ex) {
                Directory.Delete (temporary); //删除文件夹
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadSliceFile () {
            var data = Request.Form.Files["data"];
            string lastModified = Request.Form["lastModified"].ToString ();
            var total = Request.Form["total"].ToString ();
            var fileName = Request.Form["fileName"].ToString ();
            var index = Request.Form["index"].ToString ();
            string file_url = Guid.NewGuid ().ToString ();
            Dictionary<string, object> result = new Dictionary<string, object> ();

            string temporary = Path.Combine (hostingEnv.WebRootPath + "\\upload\\", lastModified); //临时保存分块的目录
            try {
                if (!Directory.Exists (temporary))
                    Directory.CreateDirectory (temporary);
                string filePath = Path.Combine (temporary, index.ToString ());
                if (!Convert.IsDBNull (data)) {
                    await Task.Run (() => {
                        if (System.IO.File.Exists (filePath)) {
                            System.IO.File.Delete (filePath);
                        }
                        FileStream fs = new FileStream (filePath, FileMode.Create);
                        data.CopyTo (fs);
                        fs.Flush ();
                        fs.Dispose ();
                        fs.Close ();
                    });
                }
                bool mergeOk = false;

                if (total == index) {
                    file_url = DateTime.Now.ToString ("yyyyMM") + "\\\\" + Guid.NewGuid ().ToString () + (fileName.LastIndexOf ('.') >= 0 ? fileName.Substring (fileName.LastIndexOf ('.')) : "");
                    mergeOk = await FileMerge (lastModified, file_url);
                    result.Add ("fileUrl", file_url);
                }

                result.Add ("number", index);
                result.Add ("mergeOk", mergeOk);
                return Json (result);

            } catch (Exception ex) {
                Directory.Delete (temporary); //删除文件夹
                throw ex;
            }
        }

        public async Task<bool> FileMerge (string lastModified, string fileName) {
            bool ok = false;
            try {
                var temporary = Path.Combine (hostingEnv.WebRootPath + "\\upload\\", lastModified); //临时文件夹
                var files = Directory.GetFiles (temporary); //获得下面的所有文件
                var finalPath = Path.Combine (hostingEnv.WebRootPath + "\\upload\\", fileName); //最终的文件名（demo中保存的是它上传时候的文件名，实际操作肯定不能这样）
                var parentPath = finalPath.Substring (0, finalPath.LastIndexOf ("\\"));
                if (!Directory.Exists (parentPath)) {
                    Directory.CreateDirectory (parentPath);
                }
                var fs = new FileStream (finalPath, FileMode.Create);
                foreach (var part in files.OrderBy (x => x.Length).ThenBy (x => x)) //排一下序，保证从0-N Write
                {
                    var bytes = System.IO.File.ReadAllBytes (part);
                    await fs.WriteAsync (bytes, 0, bytes.Length);
                    bytes = null;
                    System.IO.File.Delete (part); //删除分块
                }
                fs.Flush ();
                fs.Dispose ();
                fs.Close ();
                Directory.Delete (temporary); //删除文件夹
                ok = true;
            } catch (Exception ex) {
                throw ex;
            }
            return ok;
        }

        public async Task<bool> XFundFileMerge (string lastModified, string fileName) {
            bool ok = false;
            try {
                var temporary = Path.Combine (ConfigConstantHelper.XFundUploadFileRootPath, lastModified); //临时文件夹
                var files = Directory.GetFiles (temporary); //获得下面的所有文件
                var finalPath = Path.Combine (ConfigConstantHelper.XFundUploadFileRootPath, fileName); //最终的文件名（demo中保存的是它上传时候的文件名，实际操作肯定不能这样）
                var parentPath = finalPath.Substring (0, finalPath.LastIndexOf ("\\"));
                if (!Directory.Exists (parentPath)) {
                    Directory.CreateDirectory (parentPath);
                }
                var fs = new FileStream (finalPath, FileMode.Create);
                foreach (var part in files.OrderBy (x => x.Length).ThenBy (x => x)) //排一下序，保证从0-N Write
                {
                    var bytes = System.IO.File.ReadAllBytes (part);
                    await fs.WriteAsync (bytes, 0, bytes.Length);
                    bytes = null;
                    System.IO.File.Delete (part); //删除分块
                }
                fs.Flush ();
                fs.Dispose ();
                fs.Close ();
                Directory.Delete (temporary); //删除文件夹
                ok = true;
            } catch (Exception ex) {
                throw ex;
            }
            return ok;
        }

        public IActionResult DownloadFile (string filePath) {
            FileStream stream = null;
            if (!string.IsNullOrWhiteSpace (filePath)) {
                filePath = hostingEnv.WebRootPath + "\\upload\\" + filePath;
                if (System.IO.File.Exists (filePath)) {
                    stream = System.IO.File.OpenRead (filePath);
                }
            }
            if (stream == null) {
                HttpContext.Response.Redirect ("/Error/Error404?t=" + DateTime.Now.ToFileTimeUtc (), true);
                return View ();
            } else {
                return File (stream, "application/octet-stream", Path.GetFileName (filePath));
            }
        }

        public IActionResult DownloadXFundFile (string filePath) {
            FileStream stream = null;
            if (!string.IsNullOrWhiteSpace (filePath)) {
                filePath = ConfigConstantHelper.XFundUploadFileRootPath + filePath;
                if (System.IO.File.Exists (filePath)) {
                    stream = System.IO.File.OpenRead (filePath);
                }
            }
            if (stream == null) {
                HttpContext.Response.Redirect ("/Error/Error404?t=" + DateTime.Now.ToFileTimeUtc (), true);
                return View ();
            } else {
                return File (stream, "application/octet-stream", Path.GetFileName (filePath));
            }
        }

        public IActionResult PreviewXFundPDF (string filePath) {
            FileStream stream = null;
            if (!string.IsNullOrWhiteSpace (filePath)) {
                filePath = ConfigConstantHelper.XFundUploadFileRootPath + filePath;
                if (System.IO.File.Exists (filePath)) {
                    stream = System.IO.File.OpenRead (filePath);
                }
            }
            if (stream == null) {
                HttpContext.Response.Redirect ("/Error/Error404?t=" + DateTime.Now.ToFileTimeUtc (), true);
                return View ();
            } else {
                return File (stream, "application/pdf");
            }
        }

        /// <summary>
        /// 根据Code获取所有未删除的子项
        /// </summary>
        [HttpPost]
        public JsonResult GetEnumToList (string code) {
            List<t_sys_dictionariesObj> list = new List<t_sys_dictionariesObj> ();
            if (!string.IsNullOrWhiteSpace (code)) {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
                List<t_sys_dictionariesObj> root = db.t_sys_dictionaries.Where (rec => rec.is_delete == 0 && rec.code == code).ToList ();
                if (root != null && root.Count >= 0) {
                    var temp = db.t_sys_dictionaries.Where (rec => rec.is_delete == 0 && rec.parent_id == root[0].id).ToList ();
                    if (temp != null && temp.Count > 0) {
                        list.AddRange (temp);
                    }
                }
                db.Dispose ();
            }
            return Json (list);
        }

        /// <summary>
        /// 根据Code获取所有未删除的子项
        /// </summary>
        [HttpPost]
        public JsonResult GetEnumAndAllToList (string code) {
            List<t_sys_dictionariesObj> list = new List<t_sys_dictionariesObj> { new t_sys_dictionariesObj () { name = "全部", code = "0" } };
            if (!string.IsNullOrWhiteSpace (code)) {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
                List<t_sys_dictionariesObj> root = db.t_sys_dictionaries.Where (rec => rec.is_delete == 0 && rec.code == code).ToList ();
                if (root != null && root.Count >= 0) {
                    var temp = db.t_sys_dictionaries.Where (rec => rec.is_delete == 0 && rec.parent_id == root[0].id).ToList ();
                    if (temp != null && temp.Count > 0) {
                        list.AddRange (temp);
                    }
                }
                db.Dispose ();
            }
            return Json (list);
        }

        /// <summary>
        /// 根据id和belong_modular获取附件
        /// </summary>
        [HttpPost]
        public JsonResult GetFiles (int id, string belong_modular) {
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            List<t_sys_fileObj> files = db.t_sys_file.Where (rec => rec.belong_modular == belong_modular && rec.belong_modular_id == id.ToString ()).ToList ();
            ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
            ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
            ro.data = files;
            db.Dispose ();
            return Json (ro);
        }

        /// <summary>
        /// 根据id和belong_modular获取附件
        /// </summary>
        public JsonResult GetFilesToList (int id, string belong_modular) {
            List<t_sys_fileObj> files = null;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            files = db.t_sys_file.Where (rec => rec.belong_modular == belong_modular && rec.belong_modular_id == id.ToString ()).ToList ();
            db.Dispose ();

            if (files == null) {
                files = new List<t_sys_fileObj> ();
            }
            return Json (files);
        }
    }
}