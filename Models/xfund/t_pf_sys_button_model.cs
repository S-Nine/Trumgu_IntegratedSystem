using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_pf_sys_button")]
    public class xfund_t_pf_sys_buttonObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _btn_js_id = null;
        /// <summary>
        /// JS按钮ID
        /// </summary>
        public string btn_js_id { get { return _btn_js_id; } set { _btn_js_id = value; } }

        private string _btn_name = null;
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string btn_name { get { return _btn_name; } set { _btn_name = value; } }

        private int? _menu_id = null;
        /// <summary>
        /// 所属菜单ID
        /// </summary>
        public int? menu_id { get { return _menu_id; } set { _menu_id = value; } }

        private int? _sort = null;
        /// <summary>
        /// 按钮排序
        /// </summary>
        public int? sort { get { return _sort; } set { _sort = value; } }
    }
}