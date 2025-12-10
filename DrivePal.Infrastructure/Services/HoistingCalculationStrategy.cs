using DrivePal.Core.POCO;
using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure.Services
{
	public class HoistingCalculationStrategy : IMechanismCalculationStrategy
    {
        HoistingMechanism hoistingMechanism;


        #region IMechanismCalculationStrategy接口实现

        public void Calculate(IMechanism mechanism)
        {
            hoistingMechanism = mechanism as HoistingMechanism ?? throw new ArgumentNullException(nameof(mechanism), "mechanism 不能转换为 HoistingMechanism");

            // 计算净功率
            decimal totalNetPower = CalculateTotalNetPower(hoistingMechanism.LoadWeight_t!.Value, hoistingMechanism.SpreaderWeight_t!.Value, hoistingMechanism.LiftingSpeed_m_per_min!.Value, hoistingMechanism.TransmissionEfficiency!.Value);
            decimal singleNetPower = CalculateSingleNetPower(hoistingMechanism.IsMultidrive, hoistingMechanism.MotorCount, totalNetPower);

            // 计算负载电流
            var motorParameters = hoistingMechanism.Motor;
            decimal scaledPower = CalculateScaledPower(motorParameters.OperatingFrequency, motorParameters.RatedFrequency, singleNetPower);
            decimal loadCurrent = CalculateLoadCurrent(motorParameters.RatedVoltage, motorParameters.PowerFactor, scaledPower);

            // 计算所需变频器额定电流  
            decimal condition1 = CalculateInverterRatedCurrent(loadCurrent);
            decimal condition2 = CalculateInverterOverloadCurrent(loadCurrent, hoistingMechanism.OverloadMultiple);
            decimal requiredCurrent = Math.Max(condition1, condition2);

            hoistingMechanism.TotalNetPower = totalNetPower;
            hoistingMechanism.SingleNetPower = singleNetPower;
            hoistingMechanism.ScaledPower = scaledPower;
            hoistingMechanism.LoadCurrent = loadCurrent;
            hoistingMechanism.RatedCurrentCondition1 = condition1;
            hoistingMechanism.RatedCurrentCondition2 = condition2;
            hoistingMechanism.RequiredCurrent = requiredCurrent;
        }

        #endregion

        #region 内部实现

        private const decimal Gravity = 9.8m;
        private const decimal Sqrt3 = 1.732m;
        private const decimal IndustryCoefficient = 6.120m; // 起重机行业标准系数

        public decimal CalculateTotalNetPower(decimal loadWeight, decimal spreaderWeight , decimal liftingSpeed, decimal transmissionEfficiency)
        {
            decimal totalNetPower = 0;
            // 国际标准算法
            //return (hoisting.LoadWeight_t + hoisting.SpreaderWeight_t) * Gravity * hoisting.LiftingSpeed_m_per_min / (60 * hoisting.TransmissionEfficiency);


            // 工程实践算法: Psteady=G × V / 6.120 * η ;
            totalNetPower = (loadWeight + spreaderWeight) * liftingSpeed / (IndustryCoefficient * transmissionEfficiency);

            //if (mechanism is HoistingMechanism hoisting)
            //{
            //}
            //else if (mechanism is TrolleyGantryMechanism traveling)
            //{
            //    totalNetPower = traveling.FrictionCoefficient * (traveling.LoadWeight_t + traveling.SpreaderWeight_t) * Gravity * traveling.TravelingSpeed_m_per_min / (60 * traveling.TransmissionEfficiency);
            //}
            //else if (mechanism is LuffingMechanism luffing)
            //{
            //    totalNetPower = luffing.NetPower_kW;
            //}
            //else
            //{
            //    throw new ArgumentException("不支持的机构类型.");
            //}


            return totalNetPower;
        }



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
        /// <param name="mechanism">机构对象</param>
        /// <param name="singleNetPower">单台净功率</param>
        /// <returns></returns>
        public decimal CalculateScaledPower(decimal operatingFrequency, decimal RatedFrequency, decimal singleNetPower)
        {
            // 公式:  P_scaled = P_net * (f_rated / f_operating)

            // 按照Excel文档的例子, 当额定速度对应电机频率 < 50Hz时进行折算，否则直接使用净功率
            if (operatingFrequency < 50)
            {
                // 缩放净功率(或者叫折算净功率) = 净功率 * 电机额定频率 / 额定速度对应电机频率
                return singleNetPower * (RatedFrequency / operatingFrequency);
            }
            else
            {
                return singleNetPower;
            }

        }


        /// <summary>
        /// 计算负载电流
        /// </summary>
        /// <param name="mechanism"></param>
        /// <param name="scaledPower"></param>
        /// <returns></returns>
        public decimal CalculateLoadCurrent(decimal ratedVoltage, decimal powerFactor, decimal scaledPower)
        {
            // 公式: I_motor [A] = (P_scaled * 1000) / (√3 * U_motor * cosφ)

            /*
             * - Isteady : 额定负载电流（单位：A）
             * - Psteady : 起升机构额定稳态净功率（单位：KW）
             * - Un : 电机额定电压（单位：KV）
             * - COSΦ : 电机功率因数
             */

            return (scaledPower * 1000) / (Sqrt3 * ratedVoltage * powerFactor);
        }


        /// <summary>
        /// 计算变频器额定电流条件1
        /// </summary>
        /// <param name="mechanism"></param>
        /// <param name="loadCurrent"></param>
        /// <returns></returns>
        public decimal CalculateInverterRatedCurrent(decimal loadCurrent)
        {
            // 等同于电机的"负载电流"
            return loadCurrent;
        }

        /// <summary>
        /// 计算变频器额定电流条件2: 1.5*Ie >= 高过载电流倍数 * 负载电流
        /// </summary>
        /// <param name="mechanism"></param>
        /// <param name="loadCurrent"></param>
        /// <param name="overloadMultiple"></param>
        /// <returns></returns>
        public decimal CalculateInverterOverloadCurrent(decimal loadCurrent, decimal overloadMultiple)
        {
            /* 基于Excel文档要求：
             * 变频器额定电流Ie需满足：
             * 1. Ie ≥ 负载电流
             * 2. 1.5×Ie ≥ 高过载电流倍数×负载电流
             * 
             * 从第二个条件可推导出：Ie ≥ (高过载电流倍数×负载电流)/1.5 
             *
             * 当参数高过载电流倍数overloadFactor为1.8-2之间时
             *  - 当使用1.8作为系数: 1.8/1.5 = 1.2
             *  - 当使用2作为系数: 2/1.5 ≈ 1.333
             * 所以等价于 Ie ≥ (1.2~1.3) × 负载电流, 与Word文档的描述相符
             */
            return (overloadMultiple * loadCurrent) / 1.5m;
        }



        #endregion
    }

}
