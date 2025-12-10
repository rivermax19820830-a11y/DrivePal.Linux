using DrivePal.Core.POCO;
using DrivePal.Core.Services;
using System;

namespace DrivePal.Infrastructure.Services
{
    public class RotatingCalculationStrategy : IMechanismCalculationStrategy
    {
        RotatingMechanism mechanism;

        public void Calculate(IMechanism entity)
        {
            mechanism = entity as RotatingMechanism ?? throw new ArgumentNullException(nameof(entity), "entity 不能转换为 RotatingMechanism");

            // 计算净功率
            decimal totalNetPower = mechanism.NetPower_kW;
            decimal singleNetPower = CalculateSingleNetPower(mechanism.IsMultidrive, mechanism.MotorCount, totalNetPower);

            // 计算折算净功率
            var motorParameters = mechanism.Motor;
            decimal scaledPower = CalculateScaledPower(motorParameters.OperatingFrequency, motorParameters.RatedFrequency, singleNetPower);

            // 计算负载电流
            decimal loadCurrent = CalculateLoadCurrent(motorParameters.RatedVoltage, motorParameters.PowerFactor, scaledPower);

            // 计算所需变频器额定电流  
            decimal condition1 = CalculateInverterRatedCurrent(loadCurrent);
            decimal condition2 = CalculateInverterOverloadCurrent(loadCurrent, mechanism.OverloadMultiple);
            decimal requiredCurrent = Math.Max(condition1, condition2);

            mechanism.TotalNetPower = totalNetPower;
            mechanism.SingleNetPower = singleNetPower;
            mechanism.ScaledPower = scaledPower;
            mechanism.LoadCurrent = loadCurrent;
            mechanism.RatedCurrentCondition1 = condition1;
            mechanism.RatedCurrentCondition2 = condition2;
            mechanism.RequiredCurrent = requiredCurrent;
        }

        // 计算单台电机净功率
        private decimal CalculateSingleNetPower(bool isMultidrive, int motorCount, decimal totalNetPower)
        {
            if (isMultidrive || motorCount <= 0)
                return totalNetPower / motorCount;
            return totalNetPower;
        }

        // 计算折算净功率
        private decimal CalculateScaledPower(decimal operatingFrequency, decimal ratedFrequency, decimal netPower)
        {
            if (ratedFrequency <= 0)
                return netPower;
            return netPower * (ratedFrequency / operatingFrequency);
        }

        // 计算负载电流
        private decimal CalculateLoadCurrent(decimal ratedVoltage, decimal powerFactor, decimal scaledPower)
        {
            if (ratedVoltage <= 0 || powerFactor <= 0)
                return 0;
            // 三相电机电流计算公式: I = P * 1000 / (√3 * U * cosφ)
            return (scaledPower * 1000) / (decimal)(Math.Sqrt(3) * (double)ratedVoltage * (double)powerFactor);
        }

        // 计算变频器额定电流要求1
        private decimal CalculateInverterRatedCurrent(decimal loadCurrent)
        {
            return loadCurrent;
        }

        // 计算变频器额定电流要求2
        private decimal CalculateInverterOverloadCurrent(decimal loadCurrent, decimal overloadMultiple)
        {
            return loadCurrent * overloadMultiple;
        }
    }
}
