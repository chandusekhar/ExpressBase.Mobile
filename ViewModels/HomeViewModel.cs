﻿using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Services;
using ExpressBase.Mobile.ViewModels.BaseModels;
using ExpressBase.Mobile.Views.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ExpressBase.Mobile.ViewModels
{
    public class HomeViewModel : StaticBaseViewModel
    {
        private readonly IMenuServices menuServices;

        private List<MobilePagesWraper> objectList;

        public List<MobilePagesWraper> ObjectList
        {
            get => objectList;
            set
            {
                objectList = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource solutionlogo;

        public ImageSource SolutionLogo
        {
            get => solutionlogo;
            set
            {
                solutionlogo = value;
                NotifyPropertyChanged();
            }
        }

        public bool RefreshOnAppearing => App.Settings.CurrentApplication.HasMenuApi();

        public Command SyncButtonCommand => new Command(async () => await SyncButtonEvent());

        public Command MenuItemTappedCommand => new Command<MobilePagesWraper>(async (o) => await ItemTapedEvent(o));

        private bool isTapped;

        public HomeViewModel() : base(App.Settings.CurrentApplication?.AppName)
        {
            menuServices = new MenuServices();
        }

        public override async Task InitializeAsync()
        {
            try
            {
                this.ObjectList = await menuServices.GetDataAsync();
                await menuServices.DeployFormTables(ObjectList);

                SolutionLogo = CommonServices.GetLogo(App.Settings.Sid);
                await HelperFunctions.CreateDirectory("FILES");

                EbLog.Info($"Current Application :'{PageTitle}' with page count of {this.ObjectList.Count}.");
            }
            catch (Exception ex)
            {
                EbLog.Error("Home page initialization data request failed ::" + ex.Message);
            }
        }

        public override async Task UpdateAsync()
        {
            try
            {
                if (!Utils.HasInternet)
                {
                    Utils.Alert_NoInternet();
                }
                else
                {
                    this.ObjectList = await menuServices.UpdateDataAsync();

                    await menuServices.DeployFormTables(ObjectList);
                    EbLog.Info($"Current Application :'{PageTitle}' refreshed with page count of {this.ObjectList.Count}.");
                }
            }
            catch (Exception ex)
            {
                EbLog.Error("Home page update data request failed ::" + ex.Message);
            }
        }

        private async Task SyncButtonEvent()
        {
            try
            {
                if (NavigationService.IsTokenExpired(App.Settings.RToken))
                {
                    await NavigationService.LoginAction();
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        Device.BeginInvokeOnMainThread(() => { IsBusy = true; });

                        SyncResponse response = await menuServices.Sync();

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            IsBusy = false;
                            DependencyService.Get<IToast>().Show(response.Message);
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                EbLog.Error("Failed to sync::" + ex.Message);
            }
        }

        private async Task ItemTapedEvent(MobilePagesWraper item)
        {
            if (isTapped) return;
            try
            {
                EbMobilePage page = EbPageFinder.GetPage(item.RefId);

                if (page == null) return;

                isTapped = true;

                bool render = true;
                string message = string.Empty;

                if (page.Container is EbMobileForm form)
                {
                    Device.BeginInvokeOnMainThread(() => IsBusy = true);
                    render = await EbPageFinder.ValidateFormRendering(form);
                    message = form.MessageOnFailed;
                    Device.BeginInvokeOnMainThread(() => IsBusy = false);
                }

                if (render)
                {
                    ContentPage renderer = EbPageFinder.GetPageByContainer(page);
                    await App.RootMaster.Detail.Navigation.PushAsync(renderer);
                }
                else
                    await App.RootMaster.Detail.Navigation.PushAsync(new Redirect(message));

                isTapped = false;
            }
            catch (Exception ex)
            {
                isTapped = false;
                Device.BeginInvokeOnMainThread(() => IsBusy = false);
                EbLog.Error("Failed to open page ::" + ex.Message);
            }
        }

        public async Task LocationSwitched()
        {
            ObjectList = await menuServices.GetDataAsync();
            await menuServices.DeployFormTables(ObjectList);
        }

        public bool IsObjectsEmpty()
        {
            return !ObjectList.Any();
        }
    }
}
