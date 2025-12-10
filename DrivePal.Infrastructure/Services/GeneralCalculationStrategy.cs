using DrivePal.Core.Services;
using DrivePal.Core.POCO;
using System;
using DrivePal.Core.Dtos;

namespace DrivePal.Infrastructure.Services
{
    /// <summary>
    /// 通用选型计算策略
    /// </summary>
    public class GeneralCalculationStrategy : IMechanismCalculationStrategy
    {
        private const decimal Sqrt3 = 1.732M;

        GeneralVFDMechanism mechanism;

        public void Calculate(IMechanism entity)
        {
            mechanism = entity as GeneralVFDMechanism ?? throw new ArgumentNullException(nameof(entity));

            /**
             * 第一步：计算—确定最小电流容量 (Irequired)
             *      - 电机额定电流 (MotorRatedCurrent):       变频器的额定电流必须大于电机的额定电流 。
             *      - 海拔高度(Altitude):                  海拔 >1000m 时，需要考虑降容因素。
             *      - 产品类型 (ProductType):                如果选择“多传”，电机总电流需乘以 1.1 的环流系数 。 
             *      - 电机功率(MotorPower):                   
             *      
             * 第二步：筛选—组合所有约束进行查询
             *      - Irequired + 数据库 => 确定具体型号      (在DataService中实现)
             */

            if (mechanism is GeneralVFDMechanism general)
            {
                // 确保基准电流存在
                if (general.MotorRatedCurrent == null)
                {
                    // 如果电机额定电流为空，则无法计算，可以根据功率和电压估算，但这里先抛出异常要求输入
                    throw new InvalidOperationException("电机额定电流 (MotorRatedCurrent) 必须提供用于计算基准电流。"); 
                }


                // 1. 基准电流：电机额定电流  
                var baseCurrent = general.MotorRatedCurrent.Value; 
                var requiredInverterRatedCurrent = baseCurrent;



                // 输出:
                general.CalculationHistory.Clear();
                general.CalculationHistory.Add(new SelectionProcessDto
                {
                    StepTitle = "基准电流",
                    Description = "电机额定电流",
                    OutputValue = requiredInverterRatedCurrent
                });


                // 2. 考虑多传动环流因素 (已在页面上判断电机数)
                var multiDriveFactor = 1.0M;
                if (general.DriveMode == Core.Enums.DriveMode.OneInverterMultipleMotors)
                {
                    // "对于一个变频器驱动多台电动机，要考虑环流因素的影响，需要将各台电机的电流之和乘以 1.1 的系数作为电机总电流" 
                    multiDriveFactor = 1.1M;


                    // 输出:
                    general.CalculationHistory.Add(new SelectionProcessDto
                    {
                        StepTitle = "一个变频器驱动多台电动机",
                        Description = $"应用系数: 乘以 1.1",
                        OutputValue = baseCurrent * multiDriveFactor
                    });
                    requiredInverterRatedCurrent = baseCurrent * multiDriveFactor;
                }


                // 3. 考虑海拔降容因素 (文档 四. 注意事项)
                // 假设通用降容规则（常见）：海拔超过 1000m 后，每增加 100m，容量降低 1%。
                // 我们需要计算一个放大系数，来弥补变频器在高原环境下的容量损失。
                decimal altitudeDeratingFactor = 1.0M;
                if (general.Altitude > 1000)
                {
                    // 超过 1000m 的部分
                    decimal excessAltitude_m = general.Altitude - 1000;
                    // 降容率：每 100m 降容 1% (0.01)
                    decimal deratingRate = excessAltitude_m / 100M * 0.01M;

                    // 保护：最大降容50%
                    if (deratingRate >= 0.5M)
                    {
                        deratingRate = 0.5M;
                    }

                    // 放大系数 = 1 / (1 - 降容率)
                    altitudeDeratingFactor = 1.0M / (1.0M - deratingRate);


                    // 输出: 
                    general.CalculationHistory.Add(new SelectionProcessDto
                    {
                        StepTitle = "海拔超过 1000m",
                        Description = $"应用系数: 乘以 {altitudeDeratingFactor.ToString("F1")}",
                        OutputValue = requiredInverterRatedCurrent * altitudeDeratingFactor
                    });
                    requiredInverterRatedCurrent = requiredInverterRatedCurrent * altitudeDeratingFactor;
                } 



                // 2. 过载要求 (I_overload_factor) 
                var requiredOverloadFactor = general.OverloadFactor ?? 1.0M;
                // 输出:
                general.CalculationHistory.Add(new SelectionProcessDto
                {
                    StepTitle = "过载系数",
                    Description = $"应用系数: 乘以 {requiredOverloadFactor.ToString("F1")}",
                    OutputValue = requiredInverterRatedCurrent * requiredOverloadFactor
                });
                requiredInverterRatedCurrent = requiredInverterRatedCurrent * requiredOverloadFactor;


                // 5. 最终计算所需的最小变频器额定电流
                // RequiredInverterRatedCurrent = I_motor_rated * I_multi_drive * I_altitude_derating * I_overload_factor  
                requiredInverterRatedCurrent = (baseCurrent  * multiDriveFactor * altitudeDeratingFactor * requiredOverloadFactor);


                // 输出最终结果
                general.CalculationHistory.Add(new SelectionProcessDto
                {
                    StepTitle = "计算结果",
                    Description = "最小变频器额定电流",
                    OutputValue = requiredInverterRatedCurrent
                });

                // 保存计算变频器所需电流
                general.RequiredInverterCurrent_A = requiredInverterRatedCurrent; 
            }

        }
         

    }
}