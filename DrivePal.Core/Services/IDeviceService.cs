using DrivePal.Core.Dtos;
using DrivePal.Core.Enums;
using DrivePal.Core.Models;
using DrivePal.Core.POCO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrivePal.Core.Services
{
    public interface IDeviceService
    {
        /// <summary>
        /// 根据电流需求查询合适的变频器
        /// </summary>
        /// <param name="requiredCurrent">所需电流</param>
        /// <param name="isMultiDevice">单传还是多传</param>
        /// <returns>匹配的变频器列表</returns>
        Task<List<RecommendedInverterDto>> GetSuitableInvertersByCurrentAsync(NetPowerFilterCriteria criteria);

        /// <summary>
        /// 获取设备的参数信息
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备参数列表</returns>
        Task<List<DeviceParameterInfo>> GetDeviceParametersAsync(int deviceId);



        Task<List<RecommendedInverterDto>> QueryGeneralPurposeDevices(MotorFilterCriteria criteria);
    }
}