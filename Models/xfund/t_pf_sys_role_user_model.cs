using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_pf_sys_role_user")]
    public class xfund_t_pf_sys_role_userObj
    {
        private int? _id = null;
        public int? id { get { return _id; } set { _id = value; } }

        private int? _roleid = null;
        public int? roleid { get { return _roleid; } set { _roleid = value; } }

        private int? _userid = null;
        public int? userid { get { return _userid; } set { _userid = value; } }
    }
}