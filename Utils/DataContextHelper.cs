using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trumgu_IntegratedManageSystem.Models;
using Trumgu_IntegratedManageSystem.Models.sys;
using Trumgu_IntegratedManageSystem.Models.xfund;

namespace Trumgu_IntegratedManageSystem.Utils
{
    public class DataContextHelper : DbContext
    {
        public DataContextHelper(DbContextOptions<DataContextHelper> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseMySQL(@"server=127.0.0.1;port=3306;database=trumgu_ims_db;uid=root;pwd=Cy616620664.;charset='utf8';SslMode=None;");
        }

        /// <summary>
        /// 系统菜单表
        /// </summary>
        public DbSet<t_sys_menuObj> t_sys_menu { get; set; }

        /// <summary>
        /// 系统用户表
        /// </summary>
        public DbSet<t_sys_userObj> t_sys_user { get; set; }

        /// <summary>
        /// 系统组织机构表
        /// </summary>
        public DbSet<t_sys_delpartmentObj> t_sys_delpartment { get; set; }

        /// <summary>
        /// 系统组织机构与系统用户关系表
        /// </summary>
        public DbSet<t_sys_relation_delpartment_userObj> t_sys_relation_delpartment_user { get; set; }

        /// <summary>
        /// 系统角色与系统用户关系表
        /// </summary>
        public DbSet<t_sys_relation_role_userObj> t_sys_relation_role_user { get; set; }

        /// <summary>
        /// 系统角色表
        /// </summary>
        public DbSet<t_sys_roleObj> t_sys_role { get; set; }

        /// <summary>
        /// 系统角色&菜单&按钮关系表
        /// </summary>
        public DbSet<t_sys_relation_role_menu_buttonObj> t_sys_relation_role_menu_button { get; set; }

        /// <summary>
        /// 系统按钮表
        /// </summary>
        public DbSet<t_sys_buttonObj> t_sys_button { get; set; }

        /// <summary>
        /// 菜单树
        /// </summary>
        public DbSet<MenuTreeDataObj> MenuTreeData { get; set; }

        /// <summary>
        /// 用户部门树
        /// </summary>
        public DbSet<UserDelpartmentTreeDataObj> UserDelpartmentTreeData { get; set; }

        /// <summary>
        /// 用户角色树
        /// </summary>
        public DbSet<UserRoleTreeDataObj> UserRoleTreeData { get; set; }

        /// <summary>
        /// 公告文档
        /// </summary>
        public DbSet<t_assets_noticeObj> t_assets_notice { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public DbSet<t_sys_fileObj> t_sys_file { get; set; }

        /// <summary>
        /// XFund用户登录详情表
        /// </summary>
        public DbSet<t_xfund_user_login_infoObj> t_xfund_user_login_info { get; set; }

        /// <summary>
        /// XFund用户菜单使用详情表
        /// </summary>
        public DbSet<t_xfund_user_call_logObj> t_xfund_user_call_log { get; set; }

        public DbSet<OperationStatisticsObj> OperationStatistics { get; set; }
    }
}