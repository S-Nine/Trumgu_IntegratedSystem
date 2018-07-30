using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Relational;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_sys_role", Schema = "fund")]
    public class xfund_t_sys_roleObj
    {
        public int? id { get; set; }
        public string role { get; set; }
        public string rolecode { get; set; }
        public string status { get; set; }
        public int? data_authority { get; set; }
    }
}
