
using Common;
using Domain.Model.Sys;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;

namespace EFDatabase
{
    public class CMSDbContext
    {
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public CMSDbContext()
        {
         
               Db = new SqlSugarClient(new ConnectionConfig {
                ConnectionString = ConfigExtensions.Configuration["DBConnection:SqllistConnectionString"],
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute

               });

            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                string s = sql;
                Console.WriteLine(s);
                //Console.WriteLine(sql + "\r\n" +
                //    Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                //Console.WriteLine();
            };
           
        }

        public void Init()
        {
            Db.CodeFirst.BackupTable().InitTables<SysAdmin>();
            Db.CodeFirst.BackupTable().InitTables<SysCode>();
            Db.CodeFirst.BackupTable().InitTables<SysPermissions>();
        }

        public SimpleClient<SysAdmin> SysAdminDb => new SimpleClient<SysAdmin>(Db);
        public SimpleClient<SysCode> SysCodeDb => new SimpleClient<SysCode>(Db);

        public SimpleClient<SysPermissions> SysPermissionsDb => new SimpleClient<SysPermissions>(Db);
    }
}
