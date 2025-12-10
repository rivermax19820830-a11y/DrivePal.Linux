using DrivePal.Core.Dtos;
using DrivePal.Core.Enums;
using DrivePal.Core.Models;
using DrivePal.Core.POCO;
using DrivePal.Core.Services;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure.Services
{
    public class DeviceService : IDeviceService
    {
        #region 变量

        private readonly SqlSugarScope db;

        #endregion

        #region 构造

        public DeviceService(SqlSugarDbContext dbContext)
        {
            db = dbContext.Db;
        }

        #endregion

        public async Task<List<DeviceParameterInfo>> GetDeviceParametersAsync(int deviceId)
        {
            return await db.Queryable<DeviceParameterInfo>()
                .Where(p => p.DeviceId == deviceId)
                .ToListAsync();
        }

        #region 通过净功率计算变频器容量

        /// <summary>
        /// 根据电流得到合适的变频器型号
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
		public async Task<List<RecommendedInverterDto>>? GetSuitableInvertersByCurrentAsync(NetPowerFilterCriteria criteria)
        {
            List<RecommendedInverterDto> recommendedHoistingInverters = new List<RecommendedInverterDto>();
            List<DeviceInfo> resultDevices = new List<DeviceInfo>();

            /*** 目前只做400V的变频器选型 ***/
            switch (criteria.Mechanism)
            {
                case Mechanisms.HoistingMechanism:
                case Mechanisms.TrolleyGantryMechanism:
                case Mechanisms.LuffingMechanism:
                case Mechanisms.RotatingMechanism:
                    {
                        if (criteria.DriveMode != DriveMode.MultipleInvertersMultipleMotors)
                        {
                            #region 在HF650N变频器中选型
                            {
                                // 单传动在650N中选

                                resultDevices = await db.Queryable<DeviceInfo>()
                                    .LeftJoin<DeviceParameterInfo>((d, p) => d.Id == p.DeviceId)
                                    .Where((d, p) =>
                                        d.Model == "HF650N" &&
                                        p.ParameterName == "输出电流" &&
                                        SqlFunc.ToDecimal(p.ParameterValue) >= criteria.RequiredCurrent)
                                    .OrderBy((d, p) => SqlFunc.ToDecimal(p.ParameterValue), OrderByType.Asc)
                                    .Select((d, p) => d)
                                    .ToListAsync();
                            }
                            #endregion
                        }
                        else
                        {
                            #region 在HF680N变频器中选型

                            if (criteria.Mechanism == Mechanisms.HoistingMechanism)
                            {
                                resultDevices = await 查询起升机构多传动用变频器(criteria);
                            }
                            else if (criteria.Mechanism == Mechanisms.TrolleyGantryMechanism)
                            {
                                resultDevices = await 查询起升机构多传动用变频器(criteria);
                            }
                            else if (criteria.Mechanism == Mechanisms.LuffingMechanism)
                            {
                                resultDevices = await 查询起升机构多传动用变频器(criteria);
                            }
                            else if (criteria.Mechanism == Mechanisms.RotatingMechanism)
                            {
                                resultDevices = await 查询起升机构多传动用变频器(criteria);
                            }

                            #endregion
                        }
                    }
                    break;
                case Mechanisms.AFEMechanisms:
                    {
                        resultDevices = await 查询AFE(criteria);

                        //// 2. 第二步：从结果中提取所有设备ID
                        //var deviceIds = resultDevices.Select(d => d.Id).ToList();
                        //recommendedHoistingInverters = resultDevices.Select(device => new RecommendedInverterDto
                        //{
                        //    Variant = device.Variant,
                        //    Power = device.Power,
                        //    MotorRatedCurrent = device.Parameters?.FirstOrDefault(p => p.ParameterName == "轻过载工况 - 直流输出电流")?.ParameterValue,
                        //    MaxOverloadCurrent = device.Parameters?.FirstOrDefault(p => p.ParameterName == "重过载工况 - 输出电流")?.ParameterValue
                        //}).ToList();
                    }
                    break;
                default:
                    break;
            }
            // TODO: 处理参数不足的情况 




            // 【核心改动 1】
            // 在所有查询结束后，调用辅助方法确保 Parameters 被加载
            // 这个方法是智能的，如果查询AFE时已经用Includes加载了，它就不会重复查询
            await EnsureParametersAreLoadedAsync(resultDevices);

            // 【核心改动 2】
            // 统一组装返回结果，消除重复代码
            if (resultDevices == null || !resultDevices.Any())
            {
                return new List<RecommendedInverterDto>();
            }

            var recommendedInverters = resultDevices.Select(device =>
            {
                // 根据驱动模式确定要查找的参数名称
                string ratedCurrentParamName;
                string maxOverloadCurrentParamName;

                if (criteria.Mechanism == Mechanisms.AFEMechanisms)
                {
                    // AFE使用的是整流
                    ratedCurrentParamName = "交流额定电流";
                    maxOverloadCurrentParamName = "";
                }
                else if (criteria.DriveMode == DriveMode.MultipleInvertersMultipleMotors)
                {
                    // 多传使用的是逆变(风冷)
                    ratedCurrentParamName = "轻过载工况 - 直流输出电流";
                    maxOverloadCurrentParamName = "重过载工况 - 直流输出电流";
                }
                else // 单传动
                {
                    ratedCurrentParamName = "输出电流";
                    maxOverloadCurrentParamName = ""; // 单传动没有最大过载电流字段
                }

                return new RecommendedInverterDto
                {
                    Variant = device.Variant,
                    Power = device.Power,
                    RatedCurrent = device.Parameters?.FirstOrDefault(p => p.ParameterName == ratedCurrentParamName)?.ParameterValue,
                    MaxOverloadCurrent = string.IsNullOrEmpty(maxOverloadCurrentParamName)
                        ? ""
                        : device.Parameters?.FirstOrDefault(p => p.ParameterName == maxOverloadCurrentParamName)?.ParameterValue
                };
            }).ToList();


            // AFE同时显示两个型号的逻辑
            if (criteria.Mechanism == Mechanisms.AFEMechanisms)
            {
                var firstItem = recommendedInverters.First();
                var secondItem = recommendedInverters.FirstOrDefault(p => p.Power == firstItem.Power && p.Variant != firstItem.Variant);
                if (secondItem != null)
                {
                    return new List<RecommendedInverterDto> { firstItem, secondItem };
                }
                else
                {
                    return new List<RecommendedInverterDto> { firstItem };
                }

            }
            else
            {
                // 其他机构类型仍然只返回第一个
                return recommendedInverters.Any() ? new List<RecommendedInverterDto> { recommendedInverters.First() } : recommendedInverters;
            }
        }



        private async Task EnsureParametersAreLoadedAsync(List<DeviceInfo> devices)
        {
            if (devices == null || !devices.Any())
            {
                return;
            }

            // 找出那些 Parameters 属性尚未被加载的设备 (值为 null)
            var devicesWithoutParams = devices.Where(d => d.Parameters == null).ToList();

            if (!devicesWithoutParams.Any())
            {
                // 如果所有设备都已加载参数（例如通过 .Includes()），则无需任何操作
                return;
            }

            // 提取需要加载参数的设备ID
            var deviceIdsToLoad = devicesWithoutParams.Select(d => d.Id).ToList();

            // 一次性查询出所有相关的参数
            var allParameters = await db.Queryable<DeviceParameterInfo>()
                .Where(p => deviceIdsToLoad.Contains(p.DeviceId))
                .ToListAsync();

            // 为了方便快速查找，将参数列表按 DeviceId 分组
            var parametersLookup = allParameters.ToLookup(p => p.DeviceId);

            // 将查询到的参数填充回原来的设备对象中
            foreach (var device in devicesWithoutParams)
            {
                device.Parameters = parametersLookup.Contains(device.Id) ? parametersLookup[device.Id].ToList() : new List<DeviceParameterInfo>();
            }
        }

        private async Task<List<DeviceInfo>> 查询起升机构多传动用变频器(NetPowerFilterCriteria criteria)
        {
            var resultDevices = await db.Queryable<DeviceInfo>()
                // 1. 筛选主表 DeviceInfo 的固定条件
                .Where(device =>
                    device.Model == "HF680N" &&
                    SqlFunc.ToInt32(device.VoltageLevel) == (int)criteria.Voltage &&
                    device.Usage == "逆变(风冷)"
                )
                // 2. 使用 SqlFunc.Subquery<T>().Any() 检查第一个参数条件
                .Where(device =>
                    SqlFunc.Subqueryable<DeviceParameterInfo>()
                        .Where(p => p.DeviceId == device.Id &&
                                      p.ParameterName == "轻过载工况 - 直流输出电流" &&
                                      SqlFunc.ToDecimal(p.ParameterValue) >= criteria.RequiredCurrent)
                        .Any()
                )
                // 3. 使用 SqlFunc.Subquery<T>().Any() 检查第二个参数条件
                .Where(device =>
                    SqlFunc.Subqueryable<DeviceParameterInfo>()
                        .Where(p => p.DeviceId == device.Id &&
                                      p.ParameterName == "轻过载工况 - 适用电机功率" &&
                                      p.ParameterValue == device.Power)
                        .Any()
                )
                // 4. 最后排序并查询结果
                .OrderBy(device => SqlFunc.ToDouble(device.Power), OrderByType.Asc)
                .Select(device => device)
                .ToListAsync();

            return resultDevices;
        }


        private async Task<List<DeviceInfo>> 查询AFE(NetPowerFilterCriteria criteria)
        {
            var resultDevices = await db.Queryable<DeviceInfo>()
                .Includes(d => d.Parameters)
                .Where(d => d.Model == "HF680N"
                && SqlFunc.ToInt32(d.VoltageLevel) == (int)criteria.Voltage
                && d.Usage == "有源整流(风冷)"
                //&& d.DeviceType.Contains("成柜")
                )
                .Where(device =>
                    SqlFunc.Subqueryable<DeviceParameterInfo>()
                        .Where(p => p.DeviceId == device.Id &&
                                      p.ParameterName == "交流额定电流" &&
                                      SqlFunc.ToDecimal(p.ParameterValue) >= criteria.RequiredCurrent)
                        .Any()
                )
                .OrderBy(d => SqlFunc.ToDouble(d.Power), OrderByType.Asc)
                .ToListAsync();

            return resultDevices;
        }

        /// <summary>
        /// 获取设备型号的前缀，用于分组
        /// 对于HF680N02M和HF680N02C，将分别获取完整的型号作为前缀
        /// 这样可以确保不同后缀类型的设备被分别处理
        /// </summary>
        /// <param name="variant">设备型号</param>
        /// <returns>用于分组的标识</returns>
        private string GetModelPrefix(string variant)
        {
            // 对于AFE设备，直接返回完整型号
            // 这样可以确保不同后缀的设备（如HF680N02M和HF680N02C）被单独处理
            return variant;
        }

        #endregion

        #region 通用变频器选型

        /// <summary>
        /// 根据型号和冷却方式返回对应的电流参数名称
        /// </summary>
        private string GetRatedCurrentParamName(string model, string usage)
        {
            bool isWaterCooled = usage.Contains("水冷");

            return model switch
            {
                "HF630N" => "轻过载 - 输出电流",

                "HF650N" => "输出电流",

                "HF500" => "轻过载工况 - 输出电流",

                "HF680N" when isWaterCooled => "输出电流", // 多传 + 水冷

                "HF680N" => "轻过载工况 - 直流输出电流", // 多传 + 风冷

                _ => "输出电流"
            };
        }


        /// <summary>
        /// 通用变频器筛选
        /// </summary>
        public async Task<List<RecommendedInverterDto>> QueryGeneralPurposeDevices(MotorFilterCriteria criteria)
        {
            // ----------- 冷却方式筛选 ----------
            string coolingWord = criteria.CoolingMethod == HeatDissipationType.WaterCooled ? "水冷" : "风冷";

            // ----------- SQL 查询（初筛） ----------
            var query = db.Queryable<DeviceInfo>()
                .LeftJoin<DeviceParameterInfo>((d, p) => d.Id == p.DeviceId)
                .Where((d, p) =>
                    SqlFunc.ToInt32(d.VoltageLevel) == (int)criteria.Voltage &&
                    (
                        // ------------------ 单传 ------------------
                        criteria.ProductType == DriveTypeSelection.SingleDrive &&
                        (
                            // 单传固定是风冷 → HF630N / HF650N
                            (criteria.CoolingMethod == HeatDissipationType.AirCooled &&
                             (d.Model == "HF630N" || d.Model == "HF650N"))
                        )
                        ||

                        // ------------------ 多传 ------------------
                        criteria.ProductType == DriveTypeSelection.MultiDriveSystem &&
                        (
                            // 多传 + 风冷 → HF680N + Usage包含逆变
                            (criteria.CoolingMethod == HeatDissipationType.AirCooled &&
                             d.Model == "HF680N" &&
                             d.Usage.Contains("逆变"))

                            ||

                            // 多传 + 水冷 → HF680N
                            (criteria.CoolingMethod == HeatDissipationType.WaterCooled &&
                             d.Model == "HF680N" &&
                             d.Usage.Contains("水冷"))
                        )
                        ||

                        // ------------------ 水冷单传（特殊型号） ------------------
                        (criteria.ProductType == DriveTypeSelection.SingleDrive &&
                         criteria.CoolingMethod == HeatDissipationType.WaterCooled &&
                         d.Model == "HF500" &&
                         d.Usage.Contains("水冷"))
                    )
                    && // 功率校核 (新增：变频器功率 >= 电机功率)
                    SqlFunc.ToDecimal(d.Power) >= Convert.ToDecimal(criteria.MotorPower) &&
                    SqlFunc.ToDecimal(p.ParameterValue) >= criteria.RequiredCurrent
                )
                .OrderBy((d, p) => SqlFunc.ToDecimal(p.ParameterValue), OrderByType.Asc)
                .Select((d, p) => d)
                .Distinct();

            var devices = await query.ToListAsync();

            // ----------- 加载参数（用于二次过滤） ----------
            await EnsureParametersAreLoadedAsync(devices);

            // ----------- 分组（按型号分组） ----------
            var grouped = devices.GroupBy(d => d.Model).ToList();

            var result = new List<RecommendedInverterDto>();

            // ----------- 放档逻辑（GearFactor） ----------
            foreach (var group in grouped)
            {
                var list = group
                    .OrderBy(d =>
                    {
                        string paramName = GetRatedCurrentParamName(d.Model, d.Usage);
                        return Convert.ToDecimal(d.Parameters?.FirstOrDefault(p => p.ParameterName == paramName)?.ParameterValue ?? "999999");
                    })
                    .ToList();

                int index = criteria.GearFactor >= list.Count ? list.Count - 1 : criteria.GearFactor;
                if (index < 0)
                    index = 0;

                var selected = list[index];

                // 再获取当前设备使用的电流参数名称
                string ratedName = GetRatedCurrentParamName(selected.Model, selected.Usage);

                result.Add(new RecommendedInverterDto
                {
                    Variant = selected.Variant,
                    Power = selected.Power,
                    RatedCurrent = selected.Parameters?.FirstOrDefault(p => p.ParameterName == ratedName)?.ParameterValue,
                    MaxOverloadCurrent = selected.Parameters?.FirstOrDefault(p =>
                        p.ParameterName.Contains("过载") || p.ParameterName == "最大电流")?.ParameterValue
                });
            }

            return result;
        }


        #endregion

    }
}