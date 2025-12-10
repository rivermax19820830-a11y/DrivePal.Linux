using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Dtos
{
    public class SelectionProcessDto
    {
        public string StepTitle { get; set; } // 步骤标题，例如：“1. 负载电流修正”
        public string Description { get; set; } // 详细描述，例如：“电机额定电流 200A 乘以 1.5 倍过载系数。”
        public decimal InputValue { get; set; } // 输入值
        public decimal OutputValue { get; set; } // 输出值/中间结果
    }
}
