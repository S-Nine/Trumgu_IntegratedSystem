using System;
using System.Collections.Generic;

namespace Trumgu_IntegratedManageSystem.Models.sys {

    public class t_assets_cipher_thin_serurityObj {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private int? _order_id = null;
        /// <summary>
        /// 逻辑外键
        /// </summary>
        public int? order_id { get { return _order_id; } set { _order_id = value; } }

        private string _question = null;
        /// <summary>
        /// 问题
        /// </summary>
        public string question { get { return _question; } set { _question = value; } }

        private string _answer = null;
        /// <summary>
        /// 答案
        /// </summary>
        public string answer { get { return _answer; } set { _answer = value; } }

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