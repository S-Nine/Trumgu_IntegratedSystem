using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.sys {

    [Table ("t_sys_dictionaries",Schema="trumgu_ims_db")]
    public class t_sys_dictionariesObj {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _code = null;
        /// <summary>
        /// 字典编码：唯一标识
        /// </summary>
        public string code { get { return _code; } set { _code = value; } }

        private string _name = null;
        /// <summary>
        /// 字典名称
        /// </summary>
        public string name { get { return _name; } set { _name = value; } }

        private string _value = null;
        /// <summary>
        /// 字典值
        /// </summary>
        public string value { get { return _value; } set { _value = value; } }

        private int? _parent_id = null;
        /// <summary>
        /// 父级主键
        /// </summary>
        public int? parent_id { get { return _parent_id; } set { _parent_id = value; } }

        private string _remarks = null;
        /// <summary>
        /// 字典备注
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

    public class t_sys_dictionariesSelObj {
        public int? page { get; set; }
        public int? rows { get; set; }

        public int? parent_id { get; set; }

        public string code { get; set; }

        public string name_like { get; set; }
    }
}