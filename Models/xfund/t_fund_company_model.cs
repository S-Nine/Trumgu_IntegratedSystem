using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_fund_company")]
    public class xfund_t_fund_companyObj
    {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _regis_code = null;
        /// <summary>
        /// 登记编号
        /// </summary>
        public string regis_code { get { return _regis_code; } set { _regis_code = value; } }

        private string _cn_name = null;
        /// <summary>
        /// 公司中文名称
        /// </summary>
        public string cn_name { get { return _cn_name; } set { _cn_name = value; } }

        private string _en_name = null;
        /// <summary>
        /// 英文名称
        /// </summary>
        public string en_name { get { return _en_name; } set { _en_name = value; } }

        private string _org_code = null;
        /// <summary>
        /// 组织机构代码
        /// </summary>
        public string org_code { get { return _org_code; } set { _org_code = value; } }

        private string _regis_date = null;
        /// <summary>
        /// 登记时间
        /// </summary>
        public string regis_date { get { return _regis_date; } set { _regis_date = value; } }

        private string _setup_date = null;
        /// <summary>
        /// 成立时间
        /// </summary>
        public string setup_date { get { return _setup_date; } set { _setup_date = value; } }

        private string _regis_address = null;
        /// <summary>
        /// 注册地址
        /// </summary>
        public string regis_address { get { return _regis_address; } set { _regis_address = value; } }

        private string _address = null;
        /// <summary>
        /// 办公地址
        /// </summary>
        public string address { get { return _address; } set { _address = value; } }

        private string _regis_capital = null;
        /// <summary>
        /// 注册资本
        /// </summary>
        public string regis_capital { get { return _regis_capital; } set { _regis_capital = value; } }

        private string _paidin_capital = null;
        /// <summary>
        /// 实缴资本
        /// </summary>
        public string paidin_capital { get { return _paidin_capital; } set { _paidin_capital = value; } }

        private string _corp_property = null;
        /// <summary>
        /// 企业性质
        /// </summary>
        public string corp_property { get { return _corp_property; } set { _corp_property = value; } }

        private string _percent_of_paidin = null;
        /// <summary>
        /// 实缴比例
        /// </summary>
        public string percent_of_paidin { get { return _percent_of_paidin; } set { _percent_of_paidin = value; } }

        private string _type_of_funds = null;
        /// <summary>
        /// 基金类别
        /// </summary>
        public string type_of_funds { get { return _type_of_funds; } set { _type_of_funds = value; } }

        private string _other_business = null;
        /// <summary>
        /// 其他业务
        /// </summary>
        public string other_business { get { return _other_business; } set { _other_business = value; } }

        private int? _staff_count = null;
        /// <summary>
        /// 员工人数
        /// </summary>
        public int? staff_count { get { return _staff_count; } set { _staff_count = value; } }

        private string _website = null;
        /// <summary>
        /// 机构网址
        /// </summary>
        public string website { get { return _website; } set { _website = value; } }

        private string _if_member = null;
        /// <summary>
        /// 是否会员
        /// </summary>
        public string if_member { get { return _if_member; } set { _if_member = value; } }

        private string _type_of_member = null;
        /// <summary>
        /// 会员类型
        /// </summary>
        public string type_of_member { get { return _type_of_member; } set { _type_of_member = value; } }

        private string _date_of_join = null;
        /// <summary>
        /// 入会时间
        /// </summary>
        public string date_of_join { get { return _date_of_join; } set { _date_of_join = value; } }

        private string _legal_opinion = null;
        /// <summary>
        /// 法律意见书
        /// </summary>
        public string legal_opinion { get { return _legal_opinion; } set { _legal_opinion = value; } }

        private string _legal_repre = null;
        /// <summary>
        /// 法定代表人
        /// </summary>
        public string legal_repre { get { return _legal_repre; } set { _legal_repre = value; } }

        private string _if_qualifi = null;
        /// <summary>
        /// 是否有从业资格
        /// </summary>
        public string if_qualifi { get { return _if_qualifi; } set { _if_qualifi = value; } }

        private string _qualifi_way = null;
        /// <summary>
        /// 是否有从业资格
        /// </summary>
        public string qualifi_way { get { return _qualifi_way; } set { _qualifi_way = value; } }

        private string _credit_info = null;
        /// <summary>
        /// 机构诚信信息
        /// </summary>
        public string credit_info { get { return _credit_info; } set { _credit_info = value; } }

        private long? _company_id = null;
        public long? company_id { get { return _company_id; } set { _company_id = value; } }

        private int? _fund_count = null;
        public int? fund_count { get { return _fund_count; } set { _fund_count = value; } }

        private decimal? _company_scale = null;
        public decimal? company_scale { get { return _company_scale; } set { _company_scale = value; } }

        private string _funds_before_implement = null;
        public string funds_before_implement { get { return _funds_before_implement; } set { _funds_before_implement = value; } }

        private string _funds_after_implement = null;
        public string funds_after_implement { get { return _funds_after_implement; } set { _funds_after_implement = value; } }

        private string _insert_date = null;
        public string insert_date { get { return _insert_date; } set { _insert_date = value; } }

        private int? _if_outage = null;
        public int? if_outage { get { return _if_outage; } set { _if_outage = value; } }

        private int? _if_gongmu = null;
        public int? if_gongmu { get { return _if_gongmu; } set { _if_gongmu = value; } }

        private DateTime? _date_end = null;
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? date_end { get { return _date_end; } set { _date_end = value; } }

        private int? _if_new = null;
        /// <summary>
        /// 是否为本次新增数据
        /// </summary>
        public int? if_new { get { return _if_new; } set { _if_new = value; } }

        private string _province_id = null;
        public string province_id { get { return _province_id; } set { _province_id = value; } }

        private int? _city_id = null;
        public int? city_id { get { return _city_id; } set { _city_id = value; } }

        private string _district_id = null;
        public string district_id { get { return _district_id; } set { _district_id = value; } }

        private string _street_id = null;
        public string street_id { get { return _street_id; } set { _street_id = value; } }

        private string _hpcompany_id = null;
        public string hpcompany_id { get { return _hpcompany_id; } set { _hpcompany_id = value; } }

        private string _tyc_id = null;
        public string tyc_id { get { return _tyc_id; } set { _tyc_id = value; } }

        private decimal? _baidu_lat = null;
        /// <summary>
        /// 百度纬度
        /// </summary>
        public decimal? baidu_lat { get { return _baidu_lat; } set { _baidu_lat = value; } }

        private decimal? _baidu_lng = null;
        /// <summary>
        /// 百度经度
        /// </summary>
        public decimal? baidu_lng { get { return _baidu_lng; } set { _baidu_lng = value; } }

        private int? _temp_mark = null;
        /// <summary>
        /// 是否缺失天眼查对应数据
        /// </summary>
        public int? temp_mark { get { return _temp_mark; } set { _temp_mark = value; } }

        private int? _edit_mark = null;
        /// <summary>
        /// 0：未被认领；1：已被经纪人认领；2：已被私募公司管理人认领
        /// </summary>
        public int? edit_mark { get { return _edit_mark; } set { _edit_mark = value; } }

        private string _intro = null;
        public string intro { get { return _intro; } set { _intro = value; } }

        private string _core_person = null;
        public string core_person { get { return _core_person; } set { _core_person = value; } }

        private string _invest_idea = null;
        public string invest_idea { get { return _invest_idea; } set { _invest_idea = value; } }

        private int? _bind_mark = null;
        /// <summary>
        /// 公司是否与关联公司账号绑定：0：未绑定    1：绑定
        /// </summary>
        public int? bind_mark { get { return _bind_mark; } set { _bind_mark = value; } }

        private int? _jfz_id = null;
        public int? jfz_id { get { return _jfz_id; } set { _jfz_id = value; } }

        private int? _isdelete = null;
        public int? isdelete { get { return _isdelete; } set { _isdelete = value; } }
    }

    public class xfund_t_fund_companyExObj : xfund_t_fund_companyObj
    {
        public string jdjl { get; set; }

        public string gsjs { get; set; }

        public int? jdcs { get; set; }

        public string company_intro { get; set; }
    }

    public class xfund_t_fund_companySelObj
    {
        public string cn_name_like { get; set; }
        public int? page { get; set; }
        public int? rows { get; set; }

        /// <summary>
        /// 是否上传了公司介绍
        /// </summary>
        public bool? isIntroduction { get; set; }

        /// <summary>
        /// 是否上传了尽调记录
        /// </summary>
        public bool? isInvestigation { get; set; }
    }

    [Table("t_company_info")]
    public class t_company_infoObj
    {
        public int id { get; set; }
        public string company_name { get; set; }
        public string company_shortname { get; set; }
        public string company_intro { get; set; }
        public string invest_idea { get; set; }
        public string core_person { get; set; }
        public string regis_code { get; set; }
        public int? jy_id { get; set; }
        public DateTime? insert_date { get; set; }
        public int? edit_mark { get; set; }
    }




}