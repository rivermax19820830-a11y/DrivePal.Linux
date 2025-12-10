using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Core.Services
{
    public interface IWpfDataService
	{
        string GetWpfVariableName();
        event Action OnVariableNameChanged; // 用于通知 Blazor 页面数据更新
    }
}
