using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure
{

    /// <summary>
    /// 变频器选型器，封装AFE的计算逻辑
    /// </summary>
    public static class InverterSelector
    {
        private const double Sqrt3 = 1.732;

        public class AFEUnit
        {
            public double InputPower { get; set; }
            public double InputCurrent { get; set; }
        }

        //public static AFEUnit SelectAFEUnit(
        //    IEnumerable<IMechanism> simultaneousMechanisms,
        //    double inverterEfficiency,
        //    double afeEfficiency,
        //    double afeInputVoltage,
        //    double afeOverloadMultiple)
        //{
        //    double totalNetPower = simultaneousMechanisms.Sum(m => m.CalculateNetPower());

        //    double totalAfeInputPower = totalNetPower / (inverterEfficiency * simultaneousMechanisms.First().Motor.Efficiency);

        //    double totalAfeInputCurrent = (totalAfeInputPower * 1000) / (Sqrt3 * afeInputVoltage * afeEfficiency * afeOverloadMultiple);

        //    return new AFEUnit
        //    {
        //        InputPower = totalAfeInputPower,
        //        InputCurrent = totalAfeInputCurrent
        //    };
        //}
    }
}
