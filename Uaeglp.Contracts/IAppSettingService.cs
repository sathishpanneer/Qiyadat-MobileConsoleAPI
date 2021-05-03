using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts
{
    public interface IAppSettingService
    {
        Task<IAppSettingResponse> GetAppSettingAsync();
        Task<IAppSettingResponse> InsertAppSettingAsync(ApplicationSettingViewModel model);
        Task<IAppSettingResponse> UpdateAppSettingAsync(ApplicationSettingViewModel appSetting);
        Task<IAppSettingResponse> DeleteAppSettingAsync(string key);
    }
}
