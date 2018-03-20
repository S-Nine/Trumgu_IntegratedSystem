using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Trumgu_IntegratedManageSystem.Models.sys;

namespace Trumgu_IntegratedManageSystem.Utils
{
    public class DataContextHelper : DbContext
    {
        public DataContextHelper(DbContextOptions<DataContextHelper> options) : base(options)
        {
        }

        public DbSet<t_sys_menuObj> t_sys_menu{set;get;}


        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySQL(@"server=119.28.69.87;port=3306;database=trumgu_ims_db;uid=root;pwd=Cy616620664.;charset='utf8';SslMode=None;");
    }
}