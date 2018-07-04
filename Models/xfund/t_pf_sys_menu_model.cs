using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_pf_sys_menu")]
    public class xfund_t_pf_sys_menuObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _menu = null;
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string menu { get { return _menu; } set { _menu = value; } }

        private int? _menu_level = null;
        /// <summary>
        /// 菜单级别
        /// </summary>
        public int? menu_level { get { return _menu_level; } set { _menu_level = value; } }

        private string _classes = null;
        /// <summary>
        /// 菜单层级
        /// </summary>
        public string classes { get { return _classes; } set { _classes = value; } }

        private string _path = null;
        /// <summary>
        /// 菜单路径
        /// </summary>
        public string path { get { return _path; } set { _path = value; } }

        private int? _seq = null;
        /// <summary>
        /// 排序
        /// </summary>
        public int? seq { get { return _seq; } set { _seq = value; } }

        private int? _status = null;
        /// <summary>
        /// 菜单状态
        /// </summary>
        public int? status { get { return _status; } set { _status = value; } }

        private string _pathweb = null;
        public string pathweb { get { return _pathweb; } set { _pathweb = value; } }

        private string _code = null;
        public string code { get { return _code; } set { _code = value; } }
    }

    public class xfund_t_pf_sys_menuExObj : xfund_t_pf_sys_menuObj
    {
        public int? parent_id { get; set; }
    }
}