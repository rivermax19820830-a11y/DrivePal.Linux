using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Models
{
    [SugarTable("DeviceParameters")]
    public class DeviceParameterInfo
	{

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

         
        /// <summary>
        /// 外键
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }


        [Navigate(NavigateType.ManyToOne, nameof(DeviceId))]
        public DeviceInfo Device { get; set; }
    }
}
