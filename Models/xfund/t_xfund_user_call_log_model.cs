using System;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{

    public class t_xfund_user_call_logObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _menu_code = null;
        /// <summary>
        /// 菜单Code
        /// </summary>
        public string menu_code { get { return _menu_code; } set { _menu_code = value; } }

        private string _menu_name = null;
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string menu_name { get { return _menu_name; } set { _menu_name = value; } }

        private string _user_name = null;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string user_name { get { return _user_name; } set { _user_name = value; } }

        private string _login_name = null;
        /// <summary>
        /// 登录名称
        /// </summary>
        public string login_name { get { return _login_name; } set { _login_name = value; } }

        private string _token = null;
        /// <summary>
        /// 登录Token
        /// </summary>
        public string token { get { return _token; } set { _token = value; } }

        private DateTime? _call_time = null;
        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime? call_time { get { return _call_time; } set { _call_time = value; } }

        private string _page_title = null;
        /// <summary>
        /// 功能页面
        /// </summary>
        public string page_title { get { return _page_title; } set { _page_title = value; } }

        private string _parameters = null;
        /// <summary>
        /// 参数
        /// </summary>
        public string parameters { get { return _parameters; } set { _parameters = value; } }

        private DateTime? _create_date = null;
        /// <summary>
        /// 数据生成日期
        /// </summary>
        public DateTime? create_date { get { return _create_date; } set { _create_date = value; } }

        private string _operation_code = null;
        /// <summary>
        /// 功能模块
        /// </summary>
        public string operation_code { get { return _operation_code; } set { _operation_code = value; } }
    }

    public class t_xfund_user_call_logExObj : t_xfund_user_call_logObj
    {
        public t_xfund_user_call_logExObj()
        { }

        /// <summary>
        /// 总登录次数
        /// </summary>
        public int? sum_login_num { get; set; }

        /// <summary>
        /// 年月
        /// </summary>   
        public string year_month { get; set; }

        /// <summary>
        /// 功能描述
        /// </summary>  
        public string operation_desc { get; set; }
    }
}