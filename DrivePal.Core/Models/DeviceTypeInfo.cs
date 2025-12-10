using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Models
{
    /// <summary>
    /// 产品类型/系列表
    /// </summary>
    [SugarTable("DeviceTypes")]
    public class DeviceTypeInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }


        /// <summary>
        /// 产品系列 (e.g., "HF680N", "HF500")
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]

        public string ProductSeries { get; set; }

        /// <summary>
        /// 类型代码 (e.g., "01M", "LC03M")
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]
        public string TypeCode { get; set; }

        /// <summary>
        /// 易读的类型名称 (e.g., "基本整流模块")
        /// </summary>
        [SugarColumn(Length = 100)]
        public string TypeName { get; set; }
    }
}
