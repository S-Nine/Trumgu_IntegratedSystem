using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trumgu_IntegratedManageSystem.Models.xfund
{
    [Table("t_pf_sys_button_right")]
    public class xfund_t_pf_sys_button_rightObj
    {
        private int? _ID = null;
        /// <summary>
        /// 主键
        /// </summary>
        public int? ID { get { return _ID; } set { _ID = value; } }

        private int? _role_id = null;
        public int? role_id { get { return _role_id; } set { _role_id = value; } }

        private int? _menu_id = null;
        public int? menu_id { get { return _menu_id; } set { _menu_id = value; } }

        private string _button_id = null;
        public string button_id { get { return _button_id; } set { _button_id = value; } }

        private int? _btn_id = null;
        public int? btn_id { get { return _btn_id; } set { _btn_id = value; } }
    }
}