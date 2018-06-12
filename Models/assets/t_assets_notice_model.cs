using System;
using System.Collections.Generic;

namespace Trumgu_IntegratedManageSystem.Models.sys {

    public class t_assets_noticeObj {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _title = null;
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get { return _title; } set { _title = value; } }

        private int? _is_settop = null;
        /// <summary>
        /// 是否置顶
        /// </summary>
        public int? is_settop { get { return _is_settop; } set { _is_settop = value; } }

        private string _remarks = null;
        /// <summary>
        /// 备注
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

    public class t_assets_noticeSelObj {
        /// <summary>
        /// 标题模糊查询
        /// </summary>
        public string title_like { get; set; }
        
        public int? page { get; set; }
        public int? rows { get; set; }
    }

    

    public class t_assets_noticeExObj : t_assets_noticeObj
    {
        public List<FileInfoObj> files { get; set; }
    }

    public class FileInfoObj
    {
        public string fileName { get; set; }
        public long? fileSize { get; set; }
        public string fileUrl { get; set; }
    }
}