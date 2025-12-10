using DrivePal.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure
{

	public class SqlSugarDbContext
	{
		public SqlSugarScope Db { get; }

		public SqlSugarDbContext(string connectionString)
		{

			Db = new SqlSugarScope(new ConnectionConfig()
			{
				ConnectionString = connectionString,
				DbType = DbType.Sqlite, // 修改为SQLite
				IsAutoCloseConnection = true,
				InitKeyType = InitKeyType.Attribute // 从特性读取主键和自增列
			}); 
		}




		/// <summary>
		/// 自动建表（Code First）
		/// </summary>
		public void InitTables()
		{
			Db.CodeFirst.InitTables(
				typeof(DeviceInfo),
				typeof(DeviceParameterInfo)
			);
		}
	}
}
