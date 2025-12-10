using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Models
{


    // 设备规格信息表
    // (存储的是 DeviceType 的通用技术规格)
    [SugarTable("DeviceSpecifications")]
    public class DeviceSpecificationInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)]
        // 关联到 DeviceTypes.Id
        public int DeviceTypeId { get; set; }

        // --- 产品特征 ---
        [SugarColumn(Length = 50)]
        public string DriveType { get; set; } // 传动类型: 单传动/多传动

        [SugarColumn(Length = 50)]
        public string UnitRole { get; set; } // 单元功能角色: 逆变器/整流单元/直流斩波

        [SugarColumn(Length = 50)]
        public string StructureType { get; set; } // 结构类型: 模块型/成柜型/套件型

        [SugarColumn(Length = 50)]
        public string CoolingMethod { get; set; } // 冷却方式: 风冷/水冷

        // --- 核心电气规格 ---
        [SugarColumn(Length = 10)]
        public string InputVoltageLevel { get; set; } // 电压等级代码 (e.g., '4', '6')

        [SugarColumn(Length = 50)]
        public string RatedInputVoltage_V { get; set; } // 额定输入电压范围 (e.g., '380V~480V')

        [SugarColumn(Length = 50)]
        public string DCLinkVoltage_V { get; set; } // 直流母线电压范围 (e.g., '540V~700V')

        public double MinRatedPower_kW { get; set; } // 额定功率范围最小值 (用于筛选)

        public double MaxRatedPower_kW { get; set; } // 额定功率范围最大值 (用于筛选)

        public double MaxOutputCurrent_LT_A { get; set; } // 轻过载最大输出电流 (A)

        public double MaxOutputCurrent_HT_A { get; set; } // 重过载最大输出电流 (A)

        [SugarColumn(Length = 255)]
        public string OverloadRating { get; set; } // 详细过载能力描述

        // --- 附加功能和环境要求 ---
        [SugarColumn(ColumnDataType = "TEXT")]
        public string ControlModes { get; set; } // 支持的控制模式 (e.g., 'VC, SVC, V/F')

        public double MaxOutputFrequency_Hz { get; set; } // 最大输出频率 (Hz)

        [SugarColumn(Length = 10)]
        public string IPRating { get; set; } // 防护等级 (e.g., 'IP20', 'IP65')

        public bool HasBuiltinDCR_Default { get; set; } // 是否默认标配内置直流电抗器

        public bool HasBuiltinBrake_Default { get; set; } // 是否默认标配内置制动单元
    }
}
