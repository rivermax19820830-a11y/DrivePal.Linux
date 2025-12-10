using DrivePal.Core.Enums;
using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.POCO
{
	public class AFEMechanism : IMechanism
    {
        #region 属性

        /// <summary>
        /// 同时工作的最大负载负荷[kW]
        /// </summary>
        public decimal MaximumSimultaneousLoad { get; set; }
        
        /// <summary>
        /// 起升机构功率[kW]
        /// </summary>
        public decimal? HoistingPower { get; set; }
        
        /// <summary>
        /// 是否包含起升机构
        /// </summary>
        public bool IncludeHoisting { get; set; }
        
        /// <summary>
        /// 大小车机构功率[kW]
        /// </summary>
        public decimal? TrolleyGantryPower { get; set; }
        
        /// <summary>
        /// 是否包含大小车机构
        /// </summary>
        public bool IncludeTrolleyGantry { get; set; }
        
        /// <summary>
        /// 变幅机构功率[kW]
        /// </summary>
        public decimal? LuffingPower { get; set; }
        
        /// <summary>
        /// 是否包含变幅机构
        /// </summary>
        public bool IncludeLuffing { get; set; }
        
        /// <summary>
        /// 旋转机构功率[kW]
        /// </summary>
        public decimal? RotatingPower { get; set; }
        
        /// <summary>
        /// 是否包含旋转机构
        /// </summary>
        public bool IncludeRotating { get; set; }
        
        /// <summary>
        /// 电机效率
        /// </summary>
        public decimal MotorEfficiency { get; set; }

        /// <summary>
        ///  逆变器效率 
        /// </summary>
        public decimal InverterEfficiency { get; set; }


        /// <summary>
        /// AFE 输入电压[V] 
        /// </summary>
        public decimal AFEInputVoltage { get; set; }

        /// <summary>
        /// AFE 的效率
        /// </summary>
        public decimal AFEEfficiency { get; set; }

        /// <summary>
        /// 过载倍数
        /// </summary>
        public decimal OverloadMultiple { get; set; }


        public IndustryTypes IndustryType { get; set; }

        /// <summary>
        /// AFE最大负荷[KW]
        /// </summary>
        public decimal AFEMaximumLoad { get; set; }


        /// <summary>
        /// AFE 输入电流
        /// </summary>
        public decimal AFEInputCurrent { get; set; }



        #endregion

        #region IMechanism 接口实现

        public void PerformCalculation(IMechanismCalculationStrategy strategy)
		{
            strategy.Calculate(this);

        }

		public FilterCriteria ToDeviceSearchCriteria()
		{ 
            if (AFEInputCurrent == 0)
            {
                throw new InvalidOperationException("必须先执行PerformCalculation方法才能生成查询条件。");
            }

            var criteria = new NetPowerFilterCriteria(); 

            criteria.RequiredCurrent = AFEInputCurrent;
            criteria.Industry = this.IndustryType;
            criteria.Mechanism = Mechanisms.AFEMechanisms;
            criteria.Voltage = VoltageLevel.V380;
            criteria.Model = "HF680N"; 
            return criteria;
        } 

        #endregion

    }
}
