using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Dtos
{

    /// <summary>
    /// 用于显示推荐的变频器表格列字段
    /// </summary>
    public class RecommendedInverterDto
    {
        public string? Variant { get; set; }
        public string? Power { get; set; }
        public string? RatedCurrent { get; set; }
        public string? MaxOverloadCurrent { get; set; }
    }
}
