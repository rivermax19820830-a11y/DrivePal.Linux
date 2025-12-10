using DrivePal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.POCO
{
    /// <summary>
    /// 净功率选择方法
    /// 表示承载最终搜索条件的载体
    /// </summary>
    public class NetPowerFilterCriteria: FilterCriteria
    {
		/// <summary>
		/// 所需电流
		/// </summary>
		public decimal RequiredCurrent { get; set; }

		/// <summary>
		/// 行业 (例如: 起重, 港机, 矿山等)
		/// </summary>
		public IndustryTypes Industry { get; set; }

		/// <summary>
		/// 机构 (例如: 起升, 变幅, 回转等)
		/// </summary>
		public Mechanisms Mechanism { get; set; }

        /// <summary>
        /// 传动类型
        /// </summary>
        public DriveTypeSelection? ProductType { get; set; }


        /// <summary>
        /// 驱动方式 (例如: 一拖多, 多拖多)
        /// </summary>
        public DriveMode? DriveMode { get; set; } // DriveMode 可以是一个枚举

		
		public VoltageLevel Voltage { get; set; }

        public HeatDissipationType? CoolingMethod { get; set; }

        public string? Model { get; set; }

        // 对应 DeviceParameterInfo 表中的动态参数
        // Key: 参数名, 例如 "输出电流"
        // Value: 一个表示条件的元组 (操作符, 值), 例如 (">=", 100)
        public Dictionary<string, (string Operator, object Value)> DynamicParameters { get; set; } = new();
    }

}
