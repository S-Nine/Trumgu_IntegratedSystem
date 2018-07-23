using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_pf_banner")]
    public class t_pf_bannerObj
    {
        public int id { get; set; }
        public string banner_title { get; set; }
        public string banner_url { get; set; }
        public int banner_sort { get; set; }
        public int banner_target { get; set; }
        public string banner_link { get; set; }
        public DateTime create_time { get; set; }
        public int? create_userid { get; set; }
        public DateTime modify_time { get; set; }
        public int? modify_userid { get; set; }
        public int is_enable { get; set; }

    }

    public class t_pf_bannerEXObj : t_pf_bannerObj
    {
        public string web_banner_url { get; set; }
    }
}
