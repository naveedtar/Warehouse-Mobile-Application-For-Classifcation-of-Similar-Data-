using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WarehouseHandheld.Extensions;
using WarehouseHandheld.Helpers;
using WarehouseHandheld.Models.Orders;
using WarehouseHandheld.Modules;

namespace WarehouseHandheld.Util
{
    public static class Util
    {
        public static async Task<bool> IsConnected()
        {
            var status = await App.WarehouseService.ServerPing.CheckPortConnection(ModulesConfig.SerialNo);
            switch (status)
            {
                case Services.ServerPing.ServerStatusEnum.OK:
                    return true;
                case Services.ServerPing.ServerStatusEnum.Unauthorized:
                    "Check device permissions in system".ToToast();
                    return false;
                case Services.ServerPing.ServerStatusEnum.TimeOut:
                    "Check internet connectivity.".ToToast();
                    return false;
            }
            return true;
        }

        public static async Task ShowErrorPopupWithBeep(string txt)
        {
            AudioHelper.PlayBeep();
            await App.Current.MainPage.DisplayAlert("Warning", txt, "Ok");
        }

        public static async Task<bool> ShowErrorPopupPromptWithBeep(string txt, string yesBtnTxt = "Ok", string noBtnTxt = "")
        {
            AudioHelper.PlayBeep();
            bool result = await App.Current.MainPage.DisplayAlert("Warning", txt, yesBtnTxt, noBtnTxt);
            return result;
        }
    }

}
