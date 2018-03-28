using System;

namespace Trumgu_IntegratedManageSystem.Models.sys
{

    public class t_sys_relation_role_menu_buttonObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private int? _role_id = null;
        /// <summary>
        /// 角色主键
        /// </summary>
        public int? role_id { get { return _role_id; } set { _role_id = value; } }

        private int? _menu_id = null;
        /// <summary>
        /// 菜单主键
        /// </summary>
        public int? menu_id { get { return _menu_id; } set { _menu_id = value; } }

        private int? _btn_id = null;
        /// <summary>
        /// 菜单按钮主键
        /// </summary>
        public int? btn_id { get { return _btn_id; } set { _btn_id = value; } }

        private int? _is_delete = null;
        /// <summary>
        /// 是否删除：0->未删除；1->已删除
        /// </summary>
        public int? is_delete { get { return _is_delete; } set { _is_delete = value; } }

        private string _last_modify_id = null;
        /// <summary>
        /// 最后修改人主键
        /// </summary>
        public string last_modify_id { get { return _last_modify_id; } set { _last_modify_id = value; } }

        private string _last_modify_name = null;
        /// <summary>
        /// 最后修改人名称
        /// </summary>
        public string last_modify_name { get { return _last_modify_name; } set { _last_modify_name = value; } }

        private DateTime? _last_modify_time = null;
        /// <summary>
        /// 最后修改日期
        /// </summary>
        public DateTime? last_modify_time { get { return _last_modify_time; } set { _last_modify_time = value; } }

        private DateTime? _create_time = null;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? create_time { get { return _create_time; } set { _create_time = value; } }
    }
}