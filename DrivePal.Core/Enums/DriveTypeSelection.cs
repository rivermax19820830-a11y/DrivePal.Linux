using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Enums
{


    public enum HeatDissipationType
    {
        /// <summary>
        /// 风冷
        /// </summary>
        AirCooled,
        /// <summary>
        /// 水冷
        /// </summary>
        WaterCooled
    }

    /// <summary>
    /// 用于传动方式选择的枚举
    /// </summary>
    public enum DriveTypeSelection 
    {
        /// <summary>
        /// 独立变频器驱动模式.
        /// 指的是独立的、集成的变频器单元.
        /// </summary>
        SingleDrive,
        /// <summary>
        /// 多传动系统.
        /// 这是一种模块化、共直流母线（DC Bus）的架构.
        /// </summary>
        MultiDriveSystem
    }

    /* 多传动系统（如 HF500A/B, HF680N02/03）
     *      * 是指一个系统包含一个公共的直流母线，由一个集中的整流回馈单元 (AFE) 或基本整流单元供电，
     *      * 然后通过多个独立的逆变器单元（每个单元驱动一个或一组电机）连接到这个公共直流母线上。
     *      * 在这种架构下，每台电机通常由一个独立的逆变模块驱动，
     *      * 但这些模块共享电网输入能量。
     */

    /// <summary>
    /// 驱动模式 (暂时无用)
    /// </summary>
	public enum DriveMode
	{
        /// <summary>
        /// 单传动
        /// </summary>
        OneInverterOneMotor,
        /// <summary>
        /// 多电机单传动 (需要考虑环流因素)
        /// </summary>
		OneInverterMultipleMotors,
        /// <summary>
        /// 多传动
        /// </summary>
		MultipleInvertersMultipleMotors
	}
}
