using DrivePal.Core.Dtos;
using DrivePal.Core.Models;
using DrivePal.Core.POCO;
using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure.Services
{

    /// <summary>
    /// 默认计算策略
    /// </summary>
    public class GeneralCalculationStrategy
    {
        private const double Gravity = 9.8;
        private const double Sqrt3 = 1.732;
        private const double IndustryCoefficient = 6.120; // 起重机行业标准系数

        private readonly IDeviceService deviceService;

        public GeneralCalculationStrategy(IDeviceService deviceService)
        {
            this.deviceService = deviceService;
        }

        public double CalculateTotalNetPower(IMechanism mechanism)
        {
            double totalNetPower = 0;

            if (mechanism is HoistingMechanism hoisting)
            {
                // 国际标准算法
                //return (hoisting.LoadWeight_t + hoisting.SpreaderWeight_t) * Gravity * hoisting.LiftingSpeed_m_per_min / (60 * hoisting.TransmissionEfficiency);


                // 工程实践算法: Psteady=G × V / 6.120 * η ;
                totalNetPower = (hoisting.LoadWeight_t + hoisting.SpreaderWeight_t) * hoisting.LiftingSpeed_m_per_min / (IndustryCoefficient * hoisting.TransmissionEfficiency);
            }
            else if (mechanism is TrolleyGantryMechanism traveling)
            {
                totalNetPower = traveling.FrictionCoefficient * (traveling.LoadWeight_t + traveling.SpreaderWeight_t) * Gravity * traveling.TravelingSpeed_m_per_min / (60 * traveling.TransmissionEfficiency);
            }
            else if (mechanism is LuffingMechanism luffing)
            {
                totalNetPower = luffing.NetPower_kW;
            }
            else
            {
                throw new ArgumentException("不支持的机构类型.");
            }


            return totalNetPower;
        }

        public double CalculateSingleNetPower(IMechanism mechanism, double totalNetPower)
        {
            double singleNetPower = totalNetPower; 
            if (mechanism.IsMultidrive)
            {
                singleNetPower = totalNetPower / mechanism.MotorCount;
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
        public double CalculateScaledPower(IMechanism mechanism, double singleNetPower)
        {
            // 公式:  P_scaled = P_net * (f_rated / f_operating)

            // 按照Excel文档的例子, 当额定速度对应电机频率 < 50Hz时进行折算，否则直接使用净功率
            if (mechanism.Motor.OperatingFrequency < 50)
            {
                // 缩放净功率(或者叫折算净功率) = 净功率 * 电机额定频率 / 额定速度对应电机频率
                return singleNetPower * (mechanism.Motor.RatedFrequency / mechanism.Motor.OperatingFrequency);
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
        public double CalculateLoadCurrent(IMechanism mechanism, double scaledPower)
        {
            // 公式: I_motor [A] = (P_scaled * 1000) / (√3 * U_motor * cosφ)
            
            /*
             * - Isteady : 额定负载电流（单位：A）
             * - Psteady : 起升机构额定稳态净功率（单位：KW）
             * - Un : 电机额定电压（单位：KV）
             * - COSΦ : 电机功率因数
             */ 

            return (scaledPower * 1000) / (Sqrt3 * mechanism.Motor.RatedVoltage * mechanism.Motor.PowerFactor);
        }


        /// <summary>
        /// 计算变频器额定电流条件1
        /// </summary>
        /// <param name="mechanism"></param>
        /// <param name="loadCurrent"></param>
        /// <returns></returns>
        public double CalculateInverterRatedCurrent(IMechanism mechanism, double loadCurrent)
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
        public double CalculateInverterOverloadCurrent(IMechanism mechanism, double loadCurrent, double overloadMultiple)
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
            return (overloadMultiple * loadCurrent) / 1.5;
        }

        /// <summary>
        /// 选择适合的变频器
        /// </summary>
        /// <param name="mechanism">机构对象</param>
        /// <param name="deviceService">设备服务</param>
        /// <returns>适合的变频器列表</returns>
        public Task<List<RecommendedInverterDto>> SelectSuitableInvertersAsync(IMechanism mechanism, DeviceSearchCriteria criteria)
        {
            // 计算净功率
            double totalNetPower = CalculateTotalNetPower(mechanism);
            double singleNetPower = CalculateSingleNetPower(mechanism, totalNetPower);

            // 计算负载电流
            double scaledPower = CalculateScaledPower(mechanism, singleNetPower);
            double loadCurrent = CalculateLoadCurrent(mechanism, scaledPower);

            // 计算所需变频器额定电流  
            double condition1 = CalculateInverterRatedCurrent(mechanism, loadCurrent);
            double condition2 = CalculateInverterOverloadCurrent(mechanism, loadCurrent, mechanism.OverloadMultiple);
            double requiredInverterCurrent = Math.Max(condition1, condition2);

            // 从数据库查找匹配的变频器型号  
            return deviceService.GetSuitableInvertersAsync(criteria);
        }
         
	}
}
