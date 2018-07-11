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
    public class ProcessAssetsController : Controller {
        /// <summary>
        /// 公告文档
        /// </summary>
        public IActionResult NoticeDoc () {
            return View ();
        }

        /// <summary>
        /// 分页查询公告文档
        /// </summary>
        [HttpPost]
        public JsonResult GetNoticeToPage (t_assets_noticeSelObj sel) {
            int total = 0;
            List<t_assets_noticeObj> users = null;
            if (sel.page == null) {
                sel.page = 1;
            }
            if (sel.rows == null) {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            total = db.t_assets_notice.Where (rec => rec.is_delete == 0 &&
                    (sel != null && !string.IsNullOrWhiteSpace (sel.title_like) ? rec.title.Contains (sel.title_like) : true))
                .Count ();
            if (total > 0) {
                users = db.t_assets_notice.Where (rec => rec.is_delete == 0 &&
                        (sel != null && !string.IsNullOrWhiteSpace (sel.title_like) ? rec.title.Contains (sel.title_like) : true))
                    .OrderByDescending (rec => rec.is_settop)
                    .ThenByDescending (rec => rec.last_modify_time)
                    .ThenByDescending (rec => rec.create_time)
                    .Skip (sel.page != 1 ? ((int) sel.page - 1) * (int) sel.rows : 0)
                    .Take ((int) sel.rows)
                    .ToList ();
            }
            db.Dispose ();
            if (users == null) {
                users = new List<t_assets_noticeObj> ();
            }
            return Json (new { total = total, rows = users });
        }

        /// <summary>
        /// 新增公告文档
        /// </summary>
        public JsonResult AddNotice (t_assets_noticeExObj mdl) {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };
            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }
            t_assets_noticeObj m = new t_assets_noticeObj ();

            m.title = mdl.title;
            m.remarks = mdl.remarks;
            m.is_settop = mdl.is_settop;
            m.is_delete = 0;
            m.create_time = DateTime.Now;
            m.last_modify_time = m.create_time;
            m.last_modify_id = user.id.ToString ();
            m.last_modify_name = user.name;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            db.t_assets_notice.Add (m);
            if (db.SaveChanges () > 0) {
                if (mdl.files != null) {
                    List<t_sys_fileObj> list_file = new List<t_sys_fileObj> ();
                    for (int i = 0; i < mdl.files.Count; i++) {
                        list_file.Add (new t_sys_fileObj () {
                            file_name = mdl.files[i].fileName,
                                file_type = string.IsNullOrWhiteSpace (mdl.files[i].fileName) ? "" : mdl.files[i].fileName.Substring (mdl.files[i].fileName.LastIndexOf (".") + 1),
                                file_path = mdl.files[i].fileUrl,
                                file_size = mdl.files[i].fileSize,
                                upload_time = m.create_time,
                                belong_modular = "t_assets_notice",
                                belong_modular_id = m.id.ToString ()
                        });
                    }
                    db.t_sys_file.AddRange (list_file);
                    db.SaveChanges ();
                }

                ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_SAVE;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString ();
            }

            db.Dispose ();
            return Json (ro);
        }

        /// <summary>
        /// 删除公告文档
        /// </summary>
        public JsonResult DeleteNotice (int id) {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };

            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            List<t_assets_noticeObj> list = db.t_assets_notice.Where (rec => rec.id == id).ToList ();
            if (list != null && list.Count == 1) {
                list[0].is_delete = 1;
                list[0].last_modify_id = user.id.ToString ();
                list[0].last_modify_name = user.name;
                list[0].last_modify_time = DateTime.Now;
                db.t_assets_notice.Update (list[0]);
                if (db.SaveChanges () > 0) {
                    ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
                } else {
                    ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_SAVE;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString ();
                }
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString ();
            }
            db.Dispose ();
            return Json (ro);
        }

        public JsonResult GetNoticeFiles (int id) {
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            List<t_assets_noticeObj> list = db.t_assets_notice.Where (rec => rec.id == id).ToList ();
            if (list != null && list.Count == 1) {
                List<t_sys_fileObj> files = db.t_sys_file.Where (rec => rec.belong_modular == "t_assets_notice" && rec.belong_modular_id == id.ToString ()).ToList ();
                ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
                ro.data = files;
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString ();
            }
            db.Dispose ();
            return Json (ro);
        }

        /// <summary>
        /// 更新公告文档
        /// </summary>
        [HttpPost]
        public JsonResult UpdateNotice (t_assets_noticeExObj mdl) {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };
            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }

            if (mdl != null && mdl.id != null) {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
                List<t_assets_noticeObj> list = db.t_assets_notice.Where (rec => rec.id == mdl.id && rec.is_delete == 0).ToList ();
                if (list != null && list.Count == 1) {
                    list[0].title = mdl.title;
                    list[0].remarks = mdl.remarks;
                    list[0].is_settop = mdl.is_settop;
                    list[0].last_modify_time = DateTime.Now;
                    list[0].last_modify_id = user.id.ToString ();
                    list[0].last_modify_name = user.name;

                    db.t_assets_notice.Update (list[0]);
                    List<t_sys_fileObj> list_file_del = db.t_sys_file.Where (rec => rec.belong_modular == "t_assets_notice" && rec.belong_modular_id == mdl.id.ToString ()).ToList ();
                    // 删除历史文件
                    if (list_file_del != null && list_file_del.Count > 0) {
                        list_file_del.ForEach (f => {
                            db.t_sys_file.Attach (f);
                            db.t_sys_file.Remove (f);
                        });
                    }

                    // 新增文件
                    if (mdl.files != null) {
                        List<t_sys_fileObj> list_file = new List<t_sys_fileObj> ();
                        for (int i = 0; i < mdl.files.Count; i++) {
                            list_file.Add (new t_sys_fileObj () {
                                file_name = mdl.files[i].fileName,
                                    file_type = string.IsNullOrWhiteSpace (mdl.files[i].fileName) ? "" : mdl.files[i].fileName.Substring (mdl.files[i].fileName.LastIndexOf (".") + 1),
                                    file_path = mdl.files[i].fileUrl,
                                    file_size = mdl.files[i].fileSize,
                                    upload_time = list[0].last_modify_time,
                                    belong_modular = "t_assets_notice",
                                    belong_modular_id = mdl.id.ToString ()
                            });
                        }
                        db.t_sys_file.AddRange (list_file);
                    }

                    if (db.SaveChanges () > 0) {
                        ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                        ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
                    } else {
                        ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_SAVE;
                        ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString ();
                    }

                    db.Dispose ();
                } else {
                    ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString ();
                }
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString ();
            }

            return Json (ro);
        }

        public JsonResult GetNoticeFileList (int id) {
            List<t_sys_fileObj> files = null;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            List<t_assets_noticeObj> list = db.t_assets_notice.Where (rec => rec.id == id).ToList ();
            if (list != null && list.Count == 1) {
                files = db.t_sys_file.Where (rec => rec.belong_modular == "t_assets_notice" && rec.belong_modular_id == id.ToString ()).ToList ();
            }
            db.Dispose ();

            if (files == null) {
                files = new List<t_sys_fileObj> ();
            }
            return Json (files);
        }

        /// <summary>
        /// 组织过程资产页面渲染
        /// </summary>
        public IActionResult OrganizationalProcessAssets () {
            return View ();
        }

        /// <summary>
        /// 分页查询组织过程资产
        /// </summary>
        [HttpPost]
        public JsonResult GetOrganizationalProcessAssestsToPage (t_assets_organizational_process_assetsSelObj sel) {
            int total = 0;
            List<t_assets_organizational_process_assetsExObj> assests = null;
            if (sel.page == null) {
                sel.page = 1;
            }
            if (sel.rows == null) {
                sel.rows = 15;
            }
            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            total = (from t1 in db.t_assets_organizational_process_assets join t2 in db.t_sys_dictionaries on new { c1 = t1.type, c2 = t1.is_delete } equals new { c1 = t2.code, c2 = t2.is_delete } into t3 from t4 in t3.DefaultIfEmpty () where t1.is_delete == 0 &&
                (!string.IsNullOrWhiteSpace (sel.type) ? t1.type == sel.type : true) &&
                (!string.IsNullOrWhiteSpace (sel.title_like) ? t1.title.Contains (sel.title_like) : true) select new t_assets_organizational_process_assetsExObj () {
                    id = t1.id
                }).Count ();
            if (total > 0) {
                assests = (from t1 in db.t_assets_organizational_process_assets join t2 in db.t_sys_dictionaries on new { c1 = t1.type, c2 = t1.is_delete } equals new { c1 = t2.code, c2 = t2.is_delete } into t3 from t4 in t3.DefaultIfEmpty () where t1.is_delete == 0 &&
                    (!string.IsNullOrWhiteSpace (sel.type) ? t1.type == sel.type : true) &&
                    (!string.IsNullOrWhiteSpace (sel.title_like) ? t1.title.Contains (sel.title_like) : true) select new t_assets_organizational_process_assetsExObj () {
                        id = t1.id,
                            title = t1.title,
                            type = t1.type,
                            remarks = t1.remarks,
                            is_delete = t1.is_delete,
                            create_time = t1.create_time,
                            last_modify_id = t1.last_modify_id,
                            last_modify_name = t1.last_modify_name,
                            last_modify_time = t1.last_modify_time,
                            type_name = t4.name
                    }).OrderByDescending (rec => rec.id).ToList ();
            }
            db.Dispose ();
            if (assests == null) {
                assests = new List<t_assets_organizational_process_assetsExObj> ();
            }
            return Json (new { total = total, rows = assests });
        }

        /// <summary>
        /// 新增组织过程资产
        /// </summary>
        public JsonResult AddOrganizationalProcessAssets (t_assets_organizational_process_assetsExObj mdl) {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };
            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }
            t_assets_organizational_process_assetsObj m = new t_assets_organizational_process_assetsObj ();

            m.title = mdl.title;
            m.remarks = mdl.remarks;
            m.type = mdl.type;
            m.is_delete = 0;
            m.create_time = DateTime.Now;
            m.last_modify_time = m.create_time;
            m.last_modify_id = user.id.ToString ();
            m.last_modify_name = user.name;

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            db.t_assets_organizational_process_assets.Add (m);
            if (db.SaveChanges () > 0) {
                if (mdl.files != null) {
                    List<t_sys_fileObj> list_file = new List<t_sys_fileObj> ();
                    for (int i = 0; i < mdl.files.Count; i++) {
                        list_file.Add (new t_sys_fileObj () {
                            file_name = mdl.files[i].fileName,
                                file_type = string.IsNullOrWhiteSpace (mdl.files[i].fileName) ? "" : mdl.files[i].fileName.Substring (mdl.files[i].fileName.LastIndexOf (".") + 1),
                                file_path = mdl.files[i].fileUrl,
                                file_size = mdl.files[i].fileSize,
                                upload_time = m.create_time,
                                belong_modular = "t_assets_organizational_process_assets",
                                belong_modular_id = m.id.ToString ()
                        });
                    }
                    db.t_sys_file.AddRange (list_file);
                    db.SaveChanges ();
                }

                ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_SAVE;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString ();
            }

            db.Dispose ();
            return Json (ro);
        }

        /// <summary>
        /// 更新组织过程资产
        /// </summary>
        [HttpPost]
        public JsonResult UpdateOrganizationalProcessAssets (t_assets_organizational_process_assetsExObj mdl) {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };
            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }

            if (mdl != null && mdl.id != null) {
                Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
                List<t_assets_organizational_process_assetsObj> list = db.t_assets_organizational_process_assets.Where (rec => rec.id == mdl.id && rec.is_delete == 0).ToList ();
                if (list != null && list.Count == 1) {
                    list[0].title = mdl.title;
                    list[0].remarks = mdl.remarks;
                    list[0].type = mdl.type;
                    list[0].last_modify_time = DateTime.Now;
                    list[0].last_modify_id = user.id.ToString ();
                    list[0].last_modify_name = user.name;

                    db.t_assets_organizational_process_assets.Update (list[0]);
                    List<t_sys_fileObj> list_file_del = db.t_sys_file.Where (rec => rec.belong_modular == "t_assets_organizational_process_assets" && rec.belong_modular_id == mdl.id.ToString ()).ToList ();
                    // 删除历史文件
                    if (list_file_del != null && list_file_del.Count > 0) {
                        list_file_del.ForEach (f => {
                            db.t_sys_file.Attach (f);
                            db.t_sys_file.Remove (f);
                        });
                    }

                    // 新增文件
                    if (mdl.files != null) {
                        List<t_sys_fileObj> list_file = new List<t_sys_fileObj> ();
                        for (int i = 0; i < mdl.files.Count; i++) {
                            list_file.Add (new t_sys_fileObj () {
                                file_name = mdl.files[i].fileName,
                                    file_type = string.IsNullOrWhiteSpace (mdl.files[i].fileName) ? "" : mdl.files[i].fileName.Substring (mdl.files[i].fileName.LastIndexOf (".") + 1),
                                    file_path = mdl.files[i].fileUrl,
                                    file_size = mdl.files[i].fileSize,
                                    upload_time = list[0].last_modify_time,
                                    belong_modular = "t_assets_organizational_process_assets",
                                    belong_modular_id = mdl.id.ToString ()
                            });
                        }
                        db.t_sys_file.AddRange (list_file);
                    }

                    if (db.SaveChanges () > 0) {
                        ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                        ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
                    } else {
                        ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_SAVE;
                        ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString ();
                    }

                    db.Dispose ();
                } else {
                    ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString ();
                }
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_PARAMETER;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_PARAMETER.ToString ();
            }

            return Json (ro);
        }

        /// <summary>
        /// 删除组织过程资产
        /// </summary>
        public JsonResult DeleteOrganizationalProcessAssets (int id) {
            t_sys_userObj user = null;
            ResultObj ro = new ResultObj () { code = (int) EResponseState.TRUMGU_IMS_ERROR_INTERNAL, msg = EResponseState.TRUMGU_IMS_ERROR_INTERNAL.ToString () };

            string cUserInfo = HttpContext.Session.GetString ("UserInfo");
            if (!string.IsNullOrWhiteSpace (cUserInfo)) {
                user = Newtonsoft.Json.JsonConvert.DeserializeObject<t_sys_userObj> (cUserInfo);
            }

            Utils.DataContextHelper db = Utils.DBHelper.CreateContext ();
            List<t_assets_organizational_process_assetsObj> list = db.t_assets_organizational_process_assets.Where (rec => rec.id == id).ToList ();
            if (list != null && list.Count == 1) {
                list[0].is_delete = 1;
                list[0].last_modify_id = user.id.ToString ();
                list[0].last_modify_name = user.name;
                list[0].last_modify_time = DateTime.Now;
                db.t_assets_organizational_process_assets.Update (list[0]);
                if (db.SaveChanges () > 0) {
                    ro.code = (int) EResponseState.TRUMGU_IMS_SUCCESS;
                    ro.msg = EResponseState.TRUMGU_IMS_SUCCESS.ToString ();
                } else {
                    ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_SAVE;
                    ro.msg = EResponseState.TRUMGU_IMS_ERROR_SAVE.ToString ();
                }
            } else {
                ro.code = (int) EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND;
                ro.msg = EResponseState.TRUMGU_IMS_ERROR_NOT_FOUND.ToString ();
            }
            db.Dispose ();
            return Json (ro);
        }
    }
}