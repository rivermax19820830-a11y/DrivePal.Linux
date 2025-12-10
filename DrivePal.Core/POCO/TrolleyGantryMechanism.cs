using DrivePal.Core.Dtos;
using DrivePal.Core.Enums;
using DrivePal.Core.Models;
using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DrivePal.Core.POCO
{
    /// <summary>
    /// 大小车机构
    /// </summary>
    public class TrolleyGantryMechanism : IMechanism
    {
        #region 属性

        /// <summary>
        /// 负载重量 [t]
        /// </summary>
        public decimal LoadWeight_t { get; set; }
        /// <summary>
        ///吊具重量 [t]
        /// </summary>
        public decimal SpreaderWeight_t { get; set; } 
        /// <summary>
        /// 额定运行速度 [m/min]
        /// </summary>
        public decimal TravelingSpeed_m_per_min { get; set; } 
        /// <summary>
        /// 摩擦系数
        /// </summary>
        public decimal FrictionCoefficient { get; set; }
        /// <summary>
        /// 传动效率
        /// </summary>
        public decimal TransmissionEfficiency { get; set; } 

        /// <summary>
        /// 变频器数量
        /// </summary>
        public int InverterCount { get; set; }


        // 继承自IMechanism接口的属性和方法
        public MotorParameters Motor { get; set; }


        /// <summary>
        /// 单传动还是多传动
        /// </summary>
        public bool IsMultidrive { get; set; }

        /// <summary>
        /// 高过载电流倍数
        /// </summary>
        public decimal  OverloadMultiple
        {
            get;
            set;
        } 
        public IndustryTypes IndustryType { get; set; }





        // =======================================================
        // 2. 计算结果 (Calculated Results) - [JsonIgnore] 避免序列化
        // 这些属性在计算后被填充，可以直接用于UI展示
        // =======================================================
        [JsonIgnore]
        public decimal TotalNetPower { get; set; }
        [JsonIgnore]
        public decimal SingleNetPower { get; set; }
        [JsonIgnore]
        public decimal ScaledPower { get; set; }
        [JsonIgnore]
        public decimal LoadCurrent { get; set; }

        [JsonIgnore]
        public decimal RatedCurrentCondition1 { get; set; }

        [JsonIgnore]
        public decimal RatedCurrentCondition2 { get; set; }

        [JsonIgnore]
        public decimal? RequiredCurrent { get; set; } // 最终选型用电流




        #endregion

        #region 构造函数

        public TrolleyGantryMechanism()
        {
            
        }

        #endregion

        #region IMechanism 接口成员

        public void PerformCalculation(IMechanismCalculationStrategy strategy)
        {
            strategy.Calculate(this);
        }

		public FilterCriteria ToDeviceSearchCriteria()
        {
            // 确保计算已经执行
            if (RequiredCurrent == null || RequiredCurrent.Value == 0)
            {
                throw new InvalidOperationException("必须先执行PerformCalculation方法才能生成查询条件。");
            }

            var criteria = new NetPowerFilterCriteria();
            if (!IsMultidrive && InverterCount > 1)
            {
                criteria.DriveMode = DriveMode.OneInverterMultipleMotors;
            }
            else if (!IsMultidrive && InverterCount == 1)
            {
                criteria.DriveMode = DriveMode.OneInverterOneMotor;
            }
            else
            {
                criteria.DriveMode = DriveMode.MultipleInvertersMultipleMotors;
            }
            criteria.RequiredCurrent = RequiredCurrent.Value;
            criteria.Industry = this.IndustryType;
            criteria.Mechanism = Mechanisms.TrolleyGantryMechanism;
            criteria.Voltage = VoltageLevel.V380;
            
            
            
            criteria.Model = "HF650N";

            // 添加动态条件
            //criteria.DynamicParameters.Add("输出电流", (">=", this.RequiredCurrent));
            // criteria.DynamicParameters.Add("防护等级", ("=", "IP20"));

            return criteria;
        }


        #endregion

    }

}
