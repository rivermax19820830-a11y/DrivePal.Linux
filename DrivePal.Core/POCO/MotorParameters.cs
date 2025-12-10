using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.POCO
{

    /// <summary>
    /// 封装电机的基本电气参数
    /// <remarks>用于接收UI参数</remarks>
    /// </summary>
    public class MotorParameters
    {

        /// <summary>
        /// 额定电压 [V]
        /// </summary>
        public decimal RatedVoltage { get; set; }

        /// <summary>
        /// 电机功率因数 (cosφ)
        /// </summary>
        public decimal PowerFactor { get; set; }
        /// <summary>
        /// 电机额定频率 [Hz]
        /// </summary>
        public decimal RatedFrequency { get; set; }  
        /// <summary>
        /// 电机效率 (η_motor)
        /// </summary>
        public decimal Efficiency { get; set; } 


        /// <summary>
        /// 额定速度对应的电机频率 [Hz]
        /// </summary>
        public decimal OperatingFrequency { get; set; }



    }
}
