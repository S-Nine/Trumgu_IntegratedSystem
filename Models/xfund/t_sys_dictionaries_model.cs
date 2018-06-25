using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund {
    [Table ("t_sys_dictionaries")]
    public class xfund_t_sys_dictionariesObj {
        private int? _id = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? id { get { return _id; } set { _id = value; } }

        private string _code = null;
        /// <summary>
        /// Code
        /// </summary>
        public string code { get { return _code; } set { _code = value; } }

        private string _name = null;
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get { return _name; } set { _name = value; } }

        private string _value = null;
        /// <summary>
        /// 值
        /// </summary>
        public string value { get { return _value; } set { _value = value; } }

        private string _parentCode = null;
        /// <summary>
        /// 附近ID
        /// </summary>
        public string parentCode { get { return _parentCode; } set { _parentCode = _value; } }

        private int? _sort = null;
        /// <summary>
        /// 拍寻
        /// </summary>
        public int? sort { get { return _sort; } set { _sort = value; } }
    }
}