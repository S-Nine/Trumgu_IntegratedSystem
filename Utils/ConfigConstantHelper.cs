using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;
using Trumgu_IntegratedManageSystem.Models.xfund;

namespace Trumgu_IntegratedManageSystem.Utils
{
    public static class ConfigConstantHelper
    {
        /// <summary>
        /// 综合管理库
        /// </summary>

        public static string trumgu_ims_db_connstr { get; set; }
        /// <summary>
        /// BI库
        /// </summary>
        public static string trumgu_bi_db_connstr { get; set; }

        /// <summary>
        /// fund库
        /// </summary>
        public static string fund_connstr { get; set; }

        /// <summary>
        /// 技术支持
        /// </summary>
        public static string ProgramName { get; set; }

        /// <summary>
        /// 技术支持
        /// </summary>
        public static string TechnicalSupport { get; set; }

        /// <summary>
        /// XFund系统上传文件的本目录
        /// </summary>
        public static string XFundUploadFileRootPath { get; set; }

        /// <summary>
        /// 私募banner目录
        /// </summary>
        public static string PFUploadBannerRootPath { get; set; }

        /// <summary>
        /// 私募网址
        /// </summary>
        public static string PFWebUrl { get; set; }
    }
}