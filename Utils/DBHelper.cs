using System;
using Microsoft.EntityFrameworkCore;

namespace Trumgu_IntegratedManageSystem.Utils
{
    public class DBHelper
    {
        public static DataContextHelper CreateContext(string mySqlConnectionString = null)
        {
            if (string.IsNullOrWhiteSpace(mySqlConnectionString))
            {
                mySqlConnectionString = ConfigConstantHelper.trumgu_ims_db_connstr;
            }
            var optionBuilder = new DbContextOptionsBuilder<DataContextHelper>();
            optionBuilder.UseMySQL(mySqlConnectionString);
            var context = new DataContextHelper(optionBuilder.Options);
            return context;
        }
    }
}