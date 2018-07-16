using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("vote")]
    public class voteObj
    {
        public int id { get; set; }
        public string vote_title { get; set; }
        public string vote_content { get; set; }
        public DateTime vote_createdate { get; set; }
        public DateTime vote_startdate { get; set; }
        public DateTime vote_enddate { get; set; }
        public int vote_checktype { get; set; }
        public int vote_isclosed { get; set; }
        public int? vote_createuser { get; set; }

    }

    public class voteSelObj
    {
        public string title_like { get; set; }
    }

    [Table("vote_option")]
    public class voteOptionObj
    {
        public int id { get; set; }
        public string option_title { get; set; }
        public string option_header { get; set; }
        public int vote_id { get; set; }

    }

    public class voteOptionSelObj
    {
        public int vote_id { get; set; }
    }

    [Table("vote_answer")]
    public class voteAnswerObj
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int vote_id { get; set; }
        public int option_id { get; set; }
        public DateTime answer_createdate { get; set; }

    }

    public class voteAnswerSelObj
    {
        public int user_id { get; set; }
        public int vote_id { get; set; }
    }

    [Table("vote_comment")]
    public class voteLeaveObj
    {
        public int id { get; set; }
        public int vote_id { get; set; }
        public string comment_content { get; set; }
        public DateTime comment_date { get; set; }
        public int user_id { get; set; }
    }

    public class voteLeaveSelObj
    {
        public int id { get; set; }
        public int vote_id { get; set; }
        public string key { get; set; }
    }


}
