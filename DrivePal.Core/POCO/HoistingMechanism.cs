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
    /// 起升机构
    /// </summary>
    public class HoistingMechanism : IMechanism
    {
        #region 字段


        #endregion

        #region 属性

        // =======================================================
        // 输入参数 - 来自UI页面
        // =======================================================

        /// <summary>
        /// 负载重量 [t]
        /// </summary>
        public decimal? LoadWeight_t { get; set; }

        /// <summary>
        /// 吊具重量 [t]
        /// </summary>
        public decimal? SpreaderWeight_t { get; set; }
        /// <summary>
        /// 额定上升速度 [m/min]
        /// </summary>
        public decimal? LiftingSpeed_m_per_min { get; set; }
        /// <summary>
        /// 传动效率
        /// </summary>
        public decimal? TransmissionEfficiency { get; set; }


        /// <summary>
        /// 电机参数
        /// </summary>
        public MotorParameters Motor { get; set; } = new();

        /// <summary>
        /// 高过载电流倍数
        /// </summary>
        public decimal OverloadMultiple
        {
            get;
            set;
        }
        /// <summary>
        /// 电机数量
        /// </summary>
        public int MotorCount { get; set; }

        /// <summary>
        /// 单传动还是多传动
        /// </summary>
        public bool IsMultidrive { get; set; }

        /// <summary>
        /// 行业 (为后面不同行业作准备)
        /// </summary>
        public IndustryTypes IndustryType { get; set; } = IndustryTypes.CraneIndustry;

        // =======================================================
        // 计算结果
        // =======================================================
        [JsonIgnore]
        public decimal TotalNetPower { get;  set; }
        [JsonIgnore]
        public decimal SingleNetPower { get;  set; }
        [JsonIgnore]
        public decimal ScaledPower { get;  set; }
        [JsonIgnore]
        public decimal LoadCurrent { get;  set; }

        [JsonIgnore]
        public decimal RatedCurrentCondition1 { get; set; }

        [JsonIgnore]
        public decimal RatedCurrentCondition2 { get; set; }

        [JsonIgnore]
        public decimal? RequiredCurrent { get;  set; } // 最终选型用电流
        

        #endregion


        #region 构造

        public HoistingMechanism()
        { 
        }

        #endregion

        #region 公开的方法
         

        /// <summary>
        /// IMechanism接口中的方法，用于触发计算。
        /// 它接收一个策略，并用这个策略来填充自己的结果属性。
        /// </summary>
        public void PerformCalculation(IMechanismCalculationStrategy strategy)
        {
            strategy.Calculate(this);
        }


        /// <summary>
        /// 将自身状态转换为设备查询条件。
        /// “拼装”的逻辑被完美地封装在了最了解情况的对象内部。
        /// </summary>
        public FilterCriteria ToDeviceSearchCriteria()
        {
            // 确保计算已经执行
            if (RequiredCurrent == null || RequiredCurrent.Value == 0)
            {
                throw new InvalidOperationException("必须先执行PerformCalculation方法才能生成查询条件。");
            }

            var criteria = new NetPowerFilterCriteria();
            if (!IsMultidrive && MotorCount > 1)
            {
                criteria.DriveMode = DriveMode.OneInverterMultipleMotors;
            }
            else if (!IsMultidrive && MotorCount == 1)
            {
                criteria.DriveMode = DriveMode.OneInverterOneMotor;
            }
            else
            {
                criteria.DriveMode = DriveMode.MultipleInvertersMultipleMotors;
            }
            criteria.RequiredCurrent = RequiredCurrent.Value;
            criteria.Industry = this.IndustryType;
            criteria.Mechanism = Mechanisms.HoistingMechanism;
            criteria.Voltage = VoltageLevel.V380;
            criteria.Model = "HF650N";



            // 添加动态条件
            //criteria.DynamicParameters.Add("输出电流", (">=", this.RequiredCurrent));
            // criteria.DynamicParameters.Add("防护等级", ("=", "IP20"));

            return criteria;
        }



        //public Task<List<RecommendedInverterDto>> SelectSuitableInvertersAsync()
        //{
        //    NetPowerFilterCriteria criteria = new NetPowerFilterCriteria()
        //    {
        //        Industry = 
        //    };

        //    return CalculationStrategy.SelectSuitableInvertersAsync(this, criteria);
        //}

        #endregion



    }

}
