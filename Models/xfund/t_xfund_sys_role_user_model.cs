using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_sys_role_user", Schema = "fund")]
    public class xfund_t_sys_role_userObj
    {
        public int? id { get; set; }
        public int? roleid { get; set; }
        public int? userid { get; set; }
    }
}
