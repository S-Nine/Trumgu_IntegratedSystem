using System;
using Microsoft.EntityFrameworkCore;

namespace Trumgu_IntegratedManageSystem.Utils
{
    public class DBHelper
    {
        private const string DefaultMySqlConnectionString = @"server=119.28.69.87;port=3306;database=trumgu_ims_db;uid=root;pwd=Cy616620664.;charset='utf8';SslMode=None;";

        public static DataContextHelper CreateContext(string mySqlConnectionString = null)
        {
            if(string.IsNullOrWhiteSpace(mySqlConnectionString))
            {
                mySqlConnectionString=DefaultMySqlConnectionString;                
            }
            var optionBuilder = new DbContextOptionsBuilder<DataContextHelper>();     
            optionBuilder.UseMySQL(mySqlConnectionString); 
            var context = new DataContextHelper(optionBuilder.Options);
            return context; 
        }
    }
}