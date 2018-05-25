using System;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{

    public class t_xfund_user_login_infoObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _user_name = null;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string user_name { get { return _user_name; } set { _user_name = value; } }

        private string _person_liable = null;
        /// <summary>
        /// 负责人
        /// </summary>
        public string person_liable { get { return _person_liable; } set { _person_liable = value; } }

        private DateTime? _login_time = null;
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? login_time { get { return _login_time; } set { _login_time = value; } }

        private string _login_name = null;
        /// <summary>
        /// 账号
        /// </summary>
        public string login_name { get { return _login_name; } set { _login_name = value; } }

        private string _token = null;
        /// <summary>
        /// 账号
        /// </summary>
        public string token { get { return _token; } set { _token = value; } }

        private int? _call_num = null;
        /// <summary>
        /// 调用服务次数
        /// </summary>
        public int? call_num { get { return _call_num; } set { _call_num = value; } }

        private DateTime? _create_date = null;
        /// <summary>
        /// 数据生成时间
        /// </summary>
        public DateTime? create_date { get { return _create_date; } set { _create_date = value; } }
    }

    public class t_xfund_user_login_infoExObj : t_xfund_user_login_infoObj
    {
        public t_xfund_user_login_infoExObj()
        { }

        /// <summary>
        /// 总登录次数
        /// </summary>
        public int? sum_login_num { get; set; }

        /// <summary>
        /// 最早登录日期
        /// </summary>
        public DateTime? login_time_min { get; set; }

        /// <summary>
        /// 最晚登录日期
        /// </summary>
        public DateTime? login_time_max { get; set; }

        /// <summary>
        /// 年月
        /// </summary>   
        public string year_month { get; set; }
    }

    public class t_xfund_user_login_infoSelObj
    {
        /// <summary>
        /// 用户名模糊查询
        /// </summary>
        public string user_name_like { get; set; }

        /// <summary>
        /// 登录账号模糊查询
        /// </summary>
        public string login_name_like { get; set; }

        /// <summary>
        /// 最早登录日期
        /// </summary>
        public DateTime? login_time_min { get; set; }

        /// <summary>
        /// 最晚登录日期
        /// </summary>
        public DateTime? login_time_max { get; set; }

        public int? page { get; set; }
        public int? rows { get; set; }
    }

    public class OperationStatisticsObj
    {
         public int? id {get;set;}
        /// <summary>
        /// 功能Code
        /// </summary>
        public string operation_code { get; set; }

        /// <summary>
        /// 功能描述
        /// </summary>
        public string operation_desc { get; set; }

        /// <summary>
        /// 总使用次数
        /// </summary>
        public int? sum_login_num { get; set; }
    }
}