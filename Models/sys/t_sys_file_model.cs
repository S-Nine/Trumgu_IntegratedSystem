using System;
using System.Collections.Generic;

namespace Trumgu_IntegratedManageSystem.Models.sys
{

    public class t_sys_fileObj
    {
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

        private DateTime? _upload_time = null;
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime? upload_time { get { return _upload_time; } set { _upload_time = value; } }

        private string _belong_modular = null;
        /// <summary>
        /// 所属模块
        /// </summary>
        public string belong_modular { get { return _belong_modular; } set { _belong_modular = value; } }

        private string _belong_modular_id = null;
        /// <summary>
        /// 所属数据
        /// </summary>
        public string belong_modular_id { get { return _belong_modular_id; } set { _belong_modular_id = value; } }
    }
}