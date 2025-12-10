using DrivePal.Core.Dtos;
using DrivePal.Core.Enums;
using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.POCO
{

    /// <summary>
    /// 通用机构模型，适用于非特定行业的通用负载。 
    /// </summary>
    public class GeneralVFDMechanism : IMechanism
    {
        #region 输入参数

        public MotorParameters Motor { get; set; } = new MotorParameters();

        public int MotorCount { get; set; }

        /// <summary>
        /// 电机功率 
        /// </summary>
        public decimal? MotorPower { get; set; } = 1.0M;

        /// <summary>
        /// 电机额定电流
        /// </summary>
        public decimal? MotorRatedCurrent { get; set; }

        /// <summary>
        /// 过载系数
        /// </summary>
        public decimal? OverloadFactor { get; set; }

        /// <summary>
        /// 电压等级
        /// </summary>
        public VoltageLevel? VoltageLevel { get; set; }

        /// <summary>
        /// 海拔高度，单位：米 (m)。用于变频器降容计算。
        /// </summary>
        public int Altitude { get; set; } = 1000;

        /// <summary>
        /// 散热方式（风冷、水冷等）
        /// </summary>
        public HeatDissipationType? HeatDissipation { get; set; }

        /// <summary>
        /// 产品类型（单传、多传等）
        /// </summary>
        public DriveTypeSelection? ProductType { get; set; }


        /// <summary>
        /// 驱动方式 (例如: 一拖多, 多拖多)
        /// </summary>
        public DriveMode? DriveMode { get; set; }


        /// <summary>
        /// 变频器放档系数(默认为0, 平档)
        /// </summary>
        public decimal GearFactor { get; set; } = 0;

        /// <summary>
        /// 负载的类型描述（例如：风机、水泵、通用输送带等）。
        /// </summary>
        public string LoadDescription { get; set; }

        #endregion

        #region 输出参数
         
        /// <summary>
        /// 变频器最小额定电流
        /// </summary>
        public decimal? RequiredInverterCurrent_A { get; set; }

        public List<SelectionProcessDto> CalculationHistory { get; set; } = new List<SelectionProcessDto>();

        #endregion

        public void PerformCalculation(IMechanismCalculationStrategy strategy)
        {
            strategy.Calculate(this); 
        }

        public FilterCriteria ToDeviceSearchCriteria()
        {
            if (RequiredInverterCurrent_A == null)
            {
                throw new InvalidOperationException("必须先执行计算以确定所需的变频器电流。");
            }

            // 使用已计算的最小电流作为主要筛选条件，并结合其他筛选参数
            return new MotorFilterCriteria
            {

                // 从计算结果属性中获取所需电流
                RequiredCurrent = this.RequiredInverterCurrent_A.Value,
                MotorPower = this.MotorPower?.ToString() ?? string.Empty,
                MotorRatedCurrent = this.MotorRatedCurrent, // 变频器额定电流应大于此值
                GearFactor = Convert.ToInt32(this.GearFactor),

                // 变频器筛选参数
                Voltage = (VoltageLevel)(this.VoltageLevel ?? 0),
                CoolingMethod = (HeatDissipationType)(this.HeatDissipation ?? 0),
                ProductType = this.ProductType,
                DriveMode = this.DriveMode,
                // 其他可能需要的参数
                //RequiredPower_kW = this.MotorPower, 

            };
        }
    }
}
