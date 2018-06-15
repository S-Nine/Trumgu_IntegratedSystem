using System;
using System.Collections.Generic;

namespace Trumgu_IntegratedManageSystem.Models.sys
{

    public class t_assets_cipher_thinObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private int? _user_id = null;
        /// <summary>
        /// 用户主键
        /// </summary>
        public int? user_id { get { return _user_id; } set { _user_id = value; } }

        private string _title = null;
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get { return _title; } set { _title = value; } }

        private string _account_number = null;
        /// <summary>
        /// 账号
        /// </summary>
        public string account_number { get { return _account_number; } set { _account_number = value; } }

        private string _account_pwd = null;
        /// <summary>
        /// 密码
        /// </summary>
        public string account_pwd { get { return _account_pwd; } set { _account_pwd = value; } }

        private string _account_email = null;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string account_email { get { return _account_email; } set { _account_email = value; } }

        private string _account_tel = null;
        /// <summary>
        /// 电话
        /// </summary>
        public string account_tel { get { return _account_tel; } set { _account_tel = value; } }

        private string _account_url = null;
        /// <summary>
        /// 网址
        /// </summary>
        public string account_url { get { return _account_url; } set { _account_url = value; } }

        private DateTime? _account_register_date = null;
        /// <summary>
        /// 注册日期
        /// </summary>
        public DateTime? account_register_date { get { return _account_register_date; } set { _account_register_date = value; } }

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

    public class t_assets_cipher_thinSelObj
    {
        /// <summary>
        /// 标题模糊查询
        /// </summary>
        public string title_like { get; set; }

        public int? page { get; set; }
        public int? rows { get; set; }
    }

    public class t_assets_cipher_thinExObj : t_assets_cipher_thinObj
    {
        public List<t_assets_cipher_thin_serurityObj> qa { get; set; }
    }
}