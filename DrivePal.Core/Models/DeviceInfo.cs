using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Models
{

	/// <summary>
	/// 变频器主表
	/// </summary>
	[SugarTable("Devices")]
	public class DeviceInfo
	{
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		[SugarColumn(ColumnDescription = "品牌", Length = 50)]
		public string Brand { get; set; }


		/// <summary>
		/// 主型号, 例如HF630N
		/// </summary>
		[SugarColumn(ColumnDescription = "型号", Length = 100)]
		public string Model { get; set; }


        /// <summary>
        /// 子型号, 例如HF653N-137-4
        /// </summary>
        [SugarColumn(ColumnDescription ="子型号")]
		public string Variant { get; set; }


        [SugarColumn(ColumnDescription = "类型")]
        public string DeviceType { get; set; }


		/// <summary>
		/// 尺寸
		/// </summary>
		public string Dimension { get; set; }


		/// <summary>
		/// 重量
		/// </summary>
		public double Weight { get; set; }


		/// <summary>
		/// 功率
		/// </summary>
		public string Power { get; set; }

		/// <summary>
		/// 设备用途(变频器, 逆变, 整流...)
		/// </summary>
		public string Usage { get; set; }

		/// <summary>
		/// 电源
		/// </summary>
		public string PowerSupply { get; set; }


		/// <summary>
		/// 电压等级
		/// </summary>
		public string VoltageLevel { get; set; }


        [Navigate(NavigateType.OneToMany, nameof(DeviceParameterInfo.DeviceId))]
        public List<DeviceParameterInfo> Parameters { get; set; }
    }

}
