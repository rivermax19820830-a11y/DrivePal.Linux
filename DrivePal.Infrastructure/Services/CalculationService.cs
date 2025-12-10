using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure.Services
{
	public class CalculationService
	{
		/// <summary>
		/// 计算净功率
		/// </summary>
		/// <param name="G">最大起重量</param>
		/// <param name="V">额定起升速度</param>
		/// <param name="eta">传动效率</param>
		/// <returns>返回Psteady 起升机构额定稳态净功率, 单位[KW]</returns>
		public double CalcNetPower(double G, double V, double eta)
		{
			return (G * V) / (6.120 * eta); // kW
		}

		/// <summary>
		/// 计算负载电流
		/// </summary>
		/// <param name="Psteady">起升机构额定稳态净功率，单位[KW]</param>
		/// <param name="UnKV">电机额定电压，单位[KV]</param>
		/// <param name="cosPhi">电机功率因数，一般在 0.75--0.86 之间，具体值要查电机 样本</param>
		/// <returns>返回Isteady额定负载电流, 单位[A]</returns>
		public double CalcLoadCurrent(double Psteady, double UnKV, double cosPhi)
		{
			return Psteady / (Math.Sqrt(3) * UnKV * 1000 * cosPhi);
		}


		/// <summary>
		/// 计算所需电流
		/// </summary>
		/// <param name="Isteady">额定负载电流</param>
		/// <param name="k">高过载电流的倍数，持续 60s 过载倍数（港迪 HF500、HF650N，西门子 S120，ABBACS800 都为 1.5 ；ABB ACS880 不同 功率过载倍数根据变频器厂家提供的）</param>
		/// <returns>返回Iinv 变频器的输出额定电流（起重设备一般取持续 60s 过载时额定电流）</returns>
		public double CalcRequiredInverterCurrent(double Isteady, double k)
		{
			double cond1 = (1.8 * Isteady) / k;
			double cond2 = 1.2 * Isteady;
			return Math.Max(cond1, cond2);
		}
	}
}
