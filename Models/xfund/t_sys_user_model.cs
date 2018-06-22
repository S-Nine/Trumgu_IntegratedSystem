using System;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{

    public class xfund_t_sys_userObj
    {
        private int? _id = null;
        public int? id { get { return _id; } set { _id = value; } }

        private string _name = null;
        /// <summary>
        /// 昵称
        /// </summary>
        public string name { get { return _name; } set { _name = value; } }

        private string _userid = null;
        /// <summary>
        /// 账号
        /// </summary>
        public string userid { get { return _userid; } set { _userid = value; } }

        private string _password = null;
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get { return _password; } set { _password = value; } }

        private int? _status = null;
        /// <summary>
        /// 状态
        /// </summary>
        public int? status { get { return _status; } set { _status = value; } }

        private DateTime? _lastlogin = null;
        /// <summary>
        /// 最后一次登录日期
        /// </summary>
        public DateTime? lastlogin { get { return _lastlogin; } set { _lastlogin = value; } }

        private string _loginip = null;
        /// <summary>
        /// 最后一次登录的IP
        /// </summary>
        public string loginip { get { return _loginip; } set { _loginip = value; } }

        private int? _loginsum = null;
        /// <summary>
        /// 总登录次数
        /// </summary>
        public int? loginsum { get { return _loginsum; } set { _loginsum = value; } }

        private int? _islogin = null;
        /// <summary>
        /// 是否可以登录
        /// </summary>
        public int? islogin { get { return _islogin; } set { _islogin = value; } }

        private string _color = null;
        /// <summary>
        /// 颜色（废弃）
        /// </summary>
        public string color { get { return _color; } set { _color = value; } }

        private string _macaddr = null;
        /// <summary>
        /// MAC地址
        /// </summary>
        public string macaddr { get { return _macaddr; } set { _macaddr = value; } }

        private DateTime? _expiretime = null;
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime? expiretime { get { return _expiretime; } set { _expiretime = value; } }

        private string _person_liable = null;
        /// <summary>
        /// 负责人
        /// </summary>
        public string person_liable { get { return _person_liable; } set { _person_liable = value; } }

        private string _telephone = null;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string telephone { get { return _telephone; } set { _telephone = value; } }

        private string _company_name = null;
        /// <summary>
        /// 公司名称
        /// </summary>
        public string company_name { get { return _company_name; } set { _company_name = value; } }

        private string _hpcompany_id = null;
        /// <summary>
        /// 公司ID（机构版不保存）
        /// </summary>
        public string hpcompany_id { get { return _hpcompany_id; } set { _hpcompany_id = value; } }

        private int? _parents_id = null;
        /// <summary>
        /// 父账号ID
        /// </summary>
        public int? parents_id { get { return _parents_id; } set { _parents_id = value; } }

        private string _customertype_name = null;
        /// <summary>
        /// 客户类型
        /// </summary>
        public string customertype_name { get { return _customertype_name; } set { _customertype_name = value; } }

        private DateTime? _create_time = null;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? create_time { get { return _create_time; } set { _create_time = value; } }

        private string _create_user_name = null;
        /// <summary>
        /// 创建人昵称
        /// </summary>
        public string create_user_name { get { return _create_user_name; } set { _create_user_name = value; } }

        private int? _create_user_id = null;
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int? create_user_id { get { return _create_user_id; } set { _create_user_id = value; } }

        private int? _is_person_liable = null;
        /// <summary>
        /// 是否为负责人
        /// </summary>
        public int? is_person_liable { get { return _is_person_liable; } set { _is_person_liable = value; } }

        private int? _person_liable_id = null;
        /// <summary>
        /// 负责人ID
        /// </summary>
        public int? person_liable_id { get { return _person_liable_id; } set { _person_liable_id = value; } }

        private int? _is_pay = null;
        /// <summary>
        /// 是否为付费用户
        /// </summary>
        public int? is_pay { get { return _is_pay; } set { _is_pay = value; } }

        private string _mailbox = null;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string mailbox { get { return _mailbox; } set { _mailbox = value; } }

        private string _department = null;
        /// <summary>
        /// 部门
        /// </summary>
        public string department { get { return _department; } set { _department = value; } }

        private int? _iscompany_show = null;
        /// <summary>
        /// 消息是否显示公司名称（0：是，1否）
        /// </summary>
        public int? iscompany_show { get { return _iscompany_show; } set { _iscompany_show = value; } }
    }

    public class xfund_t_sys_userExObj : xfund_t_sys_userObj
    {
        public xfund_t_sys_userExObj()
        { }

        /// <summary>
        /// 历史登录次数
        /// </summary>
        public int? sum_history_login_num { get; set; }

        /// <summary>
        /// 最近一个月登录次数（30天）
        /// </summary>
        public int? sum_year_login_num { get; set; }

        /// <summary>
        /// 上周登录次数
        /// </summary>
        public int? sum_week_login_num { get; set; }
    }

    public class xfund_t_sys_userSelObj
    {
        public int? page { get; set; }
        public int? rows { get; set; }

        public string name_like { get; set; }

        public string company_name_like { get; set; }

        public string person_liable_like { get; set; }

        public string userid_like { get; set; }
    }
}