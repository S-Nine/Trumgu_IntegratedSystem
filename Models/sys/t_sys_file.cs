using System;

namespace Trumgu_IntegratedManageSystem.Models.sys {

    public class t_sys_fileObj {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _file_name = null;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string file_name { get { return _file_name; } set { _file_name = value; } }

        private string _file_type = null;
        /// <summary>
        /// 文件类型
        /// </summary>
        public string file_type { get { return _file_type; } set { _file_type = value; } }

        private string _file_path = null;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string file_path { get { return _file_path; } set { _file_path = value; } }

        private long? _file_size = null;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long? file_size { get { return _file_size; } set { _file_size = value; } }

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