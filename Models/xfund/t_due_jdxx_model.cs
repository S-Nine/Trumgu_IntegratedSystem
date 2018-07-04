using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_due_jdxx")]
    public class xfund_t_due_jdxxObj
    {
        private int? _id = null;
        /// <summary>
        /// ID
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _gsmc = null;
        /// <summary>
        /// 公司名称
        /// </summary>
        public string gsmc { get { return _gsmc; } set { _gsmc = value; } }

        private string _gsjc = null;
        /// <summary>
        /// 公司简称
        /// </summary>
        public string gsjc { get { return _gsjc; } set { _gsjc = value; } }

        private string _gsbh = null;
        public string gsbh { get { return _gsbh; } set { _gsbh = value; } }

        private int? _tzlx = null;
        public int? tzlx { get { return _tzlx; } set { _tzlx = value; } }

        private string _zczb = null;
        /// <summary>
        /// 注册资本
        /// </summary>
        public string zczb { get { return _zczb; } set { _zczb = value; } }

        private string _clrq = null;
        /// <summary>
        /// 成立日期
        /// </summary>
        public string clrq { get { return _clrq; } set { _clrq = value; } }

        private string _djrq = null;
        /// <summary>
        /// 登记日期
        /// </summary>
        public string djrq { get { return _djrq; } set { _djrq = value; } }

        private string _hybj = null;
        /// <summary>
        /// 是否会员
        /// </summary>
        public string hybj { get { return _hybj; } set { _hybj = value; } }

        private double? _glgm = null;
        /// <summary>
        /// 管理规模
        /// </summary>
        public double? glgm { get { return _glgm; } set { _glgm = value; } }

        private int? _cpsl = null;
        /// <summary>
        /// 产品数量
        /// </summary>
        public int? cpsl { get { return _cpsl; } set { _cpsl = value; } }

        private string _bgcs = null;
        /// <summary>
        /// 办公城市
        /// </summary>
        public string bgcs { get { return _bgcs; } set { _bgcs = value; } }

        private string _zcl = null;
        /// <summary>
        /// 子策略
        /// </summary>
        public string zcl { get { return _zcl; } set { _zcl = value; } }

        private string _tzcl = null;
        /// <summary>
        /// 投资策略
        /// </summary>
        public string tzcl { get { return _tzcl; } set { _tzcl = value; } }

        private string _dbcp = null;
        /// <summary>
        /// 代表产品
        /// </summary>
        public string dbcp { get { return _dbcp; } set { _dbcp = value; } }

        private string _dlpf = null;
        /// <summary>
        /// 定量评分
        /// </summary>
        public string dlpf { get { return _dlpf; } set { _dlpf = value; } }

        private string _jdwj = null;
        /// <summary>
        /// 尽调问卷
        /// </summary>
        public string jdwj { get { return _jdwj; } set { _jdwj = value; } }

        private string _txrq = null;
        /// <summary>
        /// 填写日期
        /// </summary>
        public string txrq { get { return _txrq; } set { _txrq = value; } }

        private string _jdjl = null;
        /// <summary>
        /// 尽调记录
        /// </summary>
        public string jdjl { get { return _jdjl; } set { _jdjl = value; } }

        private int? _jdcs = null;
        /// <summary>
        /// 尽调次数
        /// </summary>
        public int? jdcs { get { return _jdcs; } set { _jdcs = value; } }

        private string _zjjd = null;
        /// <summary>
        /// 最新尽调
        /// </summary>
        public string zjjd { get { return _zjjd; } set { _zjjd = value; } }

        private int? _pjbg = null;
        /// <summary>
        /// 评价报告
        /// </summary>
        public int? pjbg { get { return _pjbg; } set { _pjbg = value; } }

        private string _pjrq = null;
        /// <summary>
        /// 评价日期
        /// </summary>
        public string pjrq { get { return _pjrq; } set { _pjrq = value; } }

        private string _dxpj = null;
        /// <summary>
        /// 定性评价
        /// </summary>
        public string dxpj { get { return _dxpj; } set { _dxpj = value; } }

        private int? _jdzt = null;
        /// <summary>
        /// 尽调状态
        /// </summary>
        public int? jdzt { get { return _jdzt; } set { _jdzt = value; } }

        private string _gsjs = null;
        /// <summary>
        /// 公司介绍
        /// </summary>
        public string gsjs { get { return _gsjs; } set { _gsjs = value; } }
    }
}