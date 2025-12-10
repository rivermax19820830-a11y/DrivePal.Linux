using DrivePal.Core.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using DrivePal.Core.Models;
using DrivePal.Core.POCO;
using DrivePal.Core.Dtos;

namespace DrivePal.Core.Services
{
    public interface IMechanism
    {
        /// <summary>
        /// 执行与该机构相关的计算
        /// </summary>
        /// <param name="strategy">用于计算的策略</param>
        void PerformCalculation(IMechanismCalculationStrategy strategy);

        /// <summary>
        /// 根据当前机构的参数组装成"设备筛选条件对象"
        /// </summary>
        /// <returns></returns>
        FilterCriteria ToDeviceSearchCriteria();

    }
}
