using DrivePal.Core.POCO;
using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure.Services
{
	public class LufferCalculationStrategy : IMechanismCalculationStrategy
	{
        LuffingMechanism mechanism;


        public void Calculate(IMechanism entity)
		{

            mechanism = entity as LuffingMechanism ?? throw new ArgumentNullException(nameof(entity), "entity 不能转换为 LuffingMechanism");

            // 计算净功率
            decimal singleNetPower = CalculateSingleNetPower(mechanism.IsMultidrive, mechanism.MotorCount, mechanism.TotalNetPower);

            // 计算折算净功率
            var motorParameters = mechanism.Motor;
            decimal scaledPower = CalculateScaledPower(motorParameters.OperatingFrequency, motorParameters.RatedFrequency, singleNetPower);

            // 计算负载电流
            decimal loadCurrent = CalculateLoadCurrent(motorParameters.RatedVoltage, motorParameters.PowerFactor, scaledPower);

            // 计算所需变频器额定电流  
            decimal condition1 = CalculateInverterRatedCurrent(loadCurrent);
            decimal condition2 = CalculateInverterOverloadCurrent(loadCurrent, mechanism.OverloadMultiple);
            decimal requiredCurrent = Math.Max(condition1, condition2);

            mechanism.TotalNetPower = mechanism.TotalNetPower;
            mechanism.SingleNetPower = singleNetPower;
            mechanism.ScaledPower = scaledPower;
            mechanism.LoadCurrent = loadCurrent;
            mechanism.RatedCurrentCondition1 = condition1;
            mechanism.RatedCurrentCondition2 = condition2;
            mechanism.RequiredCurrent = requiredCurrent;
        }





        #region 内部实现

        private const decimal Sqrt3 = 1.732m;
        private const decimal IndustryCoefficient = 6.120m; // 起重机行业标准系数
         


        public decimal CalculateSingleNetPower(bool isMultidrive, int motorCount, decimal totalNetPower)
        {
            decimal singleNetPower = totalNetPower;
            if (isMultidrive)
            {
                singleNetPower = totalNetPower / motorCount;
            }

            return singleNetPower;
        }



        /// <summary>
        /// 计算缩放功率
        /// </summary> 
        /// <remarks>在变频调速系统中，当电机以低于额定频率运行时，其输出功率通常会降低；当以高于额定频率运行时，输出功率可能会增加（但受限于电机的物理特性）。这个缩放公式正是用来计算不同频率下实际需要的功率值。例如，如果电机的额定频率是 50Hz，但实际运行在 25Hz，那么根据这个公式，缩放功率将是净功率的 2 倍，这意味着变频器需要能够提供比净功率更大的容量来满足低频运行时的需求。</remarks>
        /// <param name="operatingFrequency">额定速度V对应电机频率[Hz]</param>
        /// <param name="RatedFrequency">电机额定频率[Hz]</param>
        /// <param name="singleNetPower">单台净功率</param>
        /// <returns></returns>
        public decimal CalculateScaledPower(decimal operatingFrequency, decimal RatedFrequency, decimal singleNetPower)
        { 
            if (operatingFrequency < 50)
            {
                // 折算净功率 = 单台净功率 * 电机额定频率 / 额定速度对应电机频率
                return singleNetPower * RatedFrequency / operatingFrequency;
            }
            else
            {
                return singleNetPower;
            }

        }


        /// <summary>
        /// 计算负载电流
        /// </summary>
        /// <param name="ratedVoltage">额定电压</param>
        /// <param name="powerFactor">电机功率因数</param>
        /// <param name="scaledPower"></param>
        /// <returns></returns>
        public decimal CalculateLoadCurrent(decimal ratedVoltage, decimal powerFactor, decimal scaledPower)
        {
            return scaledPower / Sqrt3 / ratedVoltage / powerFactor * 1000m;
        }


        /// <summary>
        /// 计算变频器额定电流条件1
        /// </summary>
        /// <param name="mechanism"></param>
        /// <param name="loadCurrent"></param>
        /// <returns></returns>
        public decimal CalculateInverterRatedCurrent(decimal loadCurrent)
        {
            return 1m * loadCurrent;
        }

        /// <summary>
        /// 计算变频器额定电流条件2: 1.5*Ie >= 高过载电流倍数 * 负载电流
        /// </summary>
        /// <param name="loadCurrent"></param>
        /// <param name="overloadMultiple">1.8</param>
        /// <returns></returns>
        public decimal CalculateInverterOverloadCurrent(decimal loadCurrent, decimal overloadMultiple)
        {
            return overloadMultiple * loadCurrent;
        }



        #endregion
    }
}
