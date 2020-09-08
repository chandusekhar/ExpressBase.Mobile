﻿using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Services;
using ExpressBase.Mobile.ViewModels.BaseModels;
using ExpressBase.Mobile.Views.Dynamic;
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

        private List<MobilePagesWraper> _objectList;

        public List<MobilePagesWraper> ObjectList
        {
            get => _objectList;
            set
            {
                _objectList = value;
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

        public Command MenuItemTappedCommand => new Command<object>(async (o) => await ItemTapedEvent(o));

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
                SolutionLogo = await menuServices.GetLogo(App.Settings.Sid);
                await HelperFunctions.CreateDirectory("FILES");

                EbLog.Error($"Current Application :'{PageTitle}' with page count of {this.ObjectList.Count}.");
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
                    EbLog.Error($"Current Application :'{PageTitle}' refreshed with page count of {this.ObjectList.Count}.");
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

        private async Task ItemTapedEvent(object obj)
        {
            if (isTapped) return;

            MobilePagesWraper item = (obj as EbMenuItem).PageWraper;

            try
            {
                EbMobilePage page = EbPageFinder.GetPage(item.RefId);

                if (page == null) return;

                isTapped = true;

                ContentPage renderer = this.GetPageByContainer(page);
                if (renderer != null)
                    await (Application.Current.MainPage as MasterDetailPage).Detail.Navigation.PushAsync(renderer);

                isTapped = false;
            }
            catch (Exception ex)
            {
                isTapped = false;
                EbLog.Error("Failed to open page ::" + ex.Message);
            }
        }

        private ContentPage GetPageByContainer(EbMobilePage page)
        {
            ContentPage renderer = null;
            try
            {
                switch (page.Container)
                {
                    case EbMobileForm f:
                        renderer = new FormRender(page);
                        break;
                    case EbMobileVisualization v:
                        renderer = new ListRender(page);
                        break;
                    case EbMobileDashBoard d:
                        renderer = new DashBoardRender(page);
                        break;
                    case EbMobilePdf p:
                        renderer = new PdfRender(page);
                        break;
                    default:
                        EbLog.Error("inavlid container type");
                        break;
                }
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
            return renderer;
        }

        public async Task LocationSwitched()
        {
            ObjectList = await menuServices.GetDataAsync();
            await menuServices.DeployFormTables(ObjectList);
        }

        public bool IsEmpty()
        {
            return !ObjectList.Any();
        }
    }
}
