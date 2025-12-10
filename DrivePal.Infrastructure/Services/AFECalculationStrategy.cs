using DrivePal.Core.POCO;
using DrivePal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure.Services
{
	public class AFECalculationStrategy : IMechanismCalculationStrategy
	{
		AFEMechanism mechanism;

		#region IMechanismCalculationStrategy 接口实现

		public void Calculate(IMechanism entity)
		{
			mechanism = entity as AFEMechanism ?? throw new ArgumentNullException(nameof(entity));

			// 根据选中的机构自动计算最大负载负荷
			mechanism.MaximumSimultaneousLoad = CalculateTotalSelectedLoad(
				mechanism.IncludeHoisting ? mechanism.HoistingPower : null,
				mechanism.IncludeTrolleyGantry ? mechanism.TrolleyGantryPower : null,
				mechanism.IncludeLuffing ? mechanism.LuffingPower : null,
				mechanism.IncludeRotating ? mechanism.RotatingPower : null
			);

			var afeMaxLoad = CalculateAFEMaxLoad(mechanism.MaximumSimultaneousLoad, mechanism.MotorEfficiency, mechanism.InverterEfficiency);
			var afeRequiredInputCurrent = CalculateRequiredInputCurrent(afeMaxLoad, mechanism.AFEInputVoltage, mechanism.AFEEfficiency, mechanism.OverloadMultiple);

            mechanism.AFEMaximumLoad = afeMaxLoad;
			mechanism.AFEInputCurrent = afeRequiredInputCurrent;
        }

		#endregion


		#region 内部实现

		private decimal CalculateAFEMaxLoad(decimal maximumSimultaneousLoad, decimal motorEfficiency, decimal inverterEfficiency)
		{
            // 公式 : Ptotal= ∑［P / ( ηmot × ηinvv）］

            // 同时工作的最大负载负荷[kW] / 电机效率 / 逆变器效率
            return maximumSimultaneousLoad / motorEfficiency / inverterEfficiency;
        }

		private decimal CalculateRequiredInputCurrent(decimal afeMaxLoad, decimal afeInputVoltage, decimal afeEfficiency, decimal overloadMultiple)
		{
            // IAFE= Ptotal/( √3 ·UIN · ηRec ·k)


			// 将380V转换为KV
            decimal inputVoltageInKV = afeInputVoltage / 1000m;

			//  AFE 的额定电流需大于等于此值  
			//var result = afeMaxLoad / 1.732m / inputVoltageInKV / afeEfficiency / overloadMultiple;
			// 不再用过载倍数
			var result = afeMaxLoad / 1.732m / inputVoltageInKV / afeEfficiency;


            return result;
        }

		/// <summary>
		/// 计算选中的机构总负载
		/// </summary>
		private decimal CalculateTotalSelectedLoad(decimal? hoistingPower, decimal? trolleyGantryPower, decimal? luffingPower, decimal? rotatingPower)
		{
			return (hoistingPower ?? 0) + (trolleyGantryPower ?? 0) + (luffingPower ?? 0) + (rotatingPower ?? 0);
		}
        #endregion
    }
}
