using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_pf_sys_role")]
    public class xfund_t_pf_sys_roleObj
    {
        private int? _id = null;
        public int? id { get { return _id; } set { _id = value; } }

        private string _role = null;
        public string role { get { return _role; } set { _role = value; } }

        private string _rolecode = null;
        public string rolecode { get { return _rolecode; } set { _rolecode = value; } }

        private string _status = null;
        public string status { get { return _status; } set { _status = value; } }

        private int? _data_authority = null;
        public int? data_authority { get { return _data_authority; } set { _data_authority = value; } }
    }

    public class xfund_t_pf_sys_roleSelObj
    {
        public string name_like{get;set;}        
    }
}