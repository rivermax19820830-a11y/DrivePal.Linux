using DrivePal.Core.Dtos;
using DrivePal.Core.Enums;
using DrivePal.Core.Models;
using DrivePal.Core.Services;
using System.Text.Json.Serialization;

namespace DrivePal.Core.POCO
{
    /// <summary>
    /// 旋转机构
    /// </summary>
    public class RotatingMechanism : IMechanism
    {
        #region 输入参数

        /// <summary>
        /// 总净功率[kW]
        /// </summary>
        public decimal NetPower_kW { get; set; }

        /// <summary>
        /// 电机数量
        /// </summary>
        public int MotorCount { get; set; }

        /// <summary>
        /// 电机参数
        /// </summary>
        public MotorParameters Motor { get; set; }

        /// <summary>
        /// 高过载电流倍数
        /// </summary>
        public decimal OverloadMultiple { get; set; }

        /// <summary>
        /// 单传动还是多传动
        /// </summary>
        public bool IsMultidrive { get; set; }

        /// <summary>
        /// 行业类型
        /// </summary>
        public IndustryTypes IndustryType { get; set; }

        #endregion

        #region 计算结果

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
        public decimal? RequiredCurrent { get; set; }

        #endregion

        #region IMechanism 接口实现

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
            criteria.Mechanism = Mechanisms.RotatingMechanism;
            criteria.Voltage = VoltageLevel.V380;
            criteria.Model = "HF650N";

            return criteria;
        }

        #endregion
    }
}
