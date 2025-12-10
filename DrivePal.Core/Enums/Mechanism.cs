using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Enums
{
	public enum Mechanisms
	{
		Unknown,
		/// <summary>
		/// 起升
		/// </summary>
		HoistingMechanism,
		/// <summary>
		/// 变幅
		/// </summary>
		LuffingMechanism,
		/// <summary>
		/// 大小车
		/// </summary>
		TrolleyGantryMechanism, 
		/// <summary>
		/// 旋转
		/// </summary>
		RotatingMechanism,
		/// <summary>
		/// AFE
		/// </summary>
		AFEMechanisms,
	}
}
