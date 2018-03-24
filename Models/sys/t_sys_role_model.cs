using System;

namespace Trumgu_IntegratedManageSystem.Models.sys
{
    public class t_sys_roleObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }


        private string _name = null;
        /// <summary>
        /// 角色名称
        /// </summary>
        public string name { get { return _name; } set { _name = value; } }

        private int? _data_permiss = null;
        /// <summary>
        /// 角色权限：0->全部数据；100->本部门及下属部门数据；200->自建数据
        /// </summary>
        public int? data_permiss { get { return _data_permiss; } set { _data_permiss = value; } }

        private int? _state = null;
        /// <summary>
        /// 角色状态：0->禁用；1->启用
        /// </summary>
        public int? state { get { return _state; } set { _state = value; } }

        private string _remarks = null;
        /// <summary>
        /// 角色备注
        /// </summary>
        public string remarks { get { return _remarks; } set { _remarks = value; } }

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