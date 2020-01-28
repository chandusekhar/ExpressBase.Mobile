﻿using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Data;
using ExpressBase.Mobile.Extensions;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Services;
using ExpressBase.Mobile.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ExpressBase.Mobile.ViewModels
{
    public class ObjectsRenderViewModel : BaseViewModel
    {
        const int RefreshDuration = 2;

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return this._isRefreshing; }
            set
            {
                this._isRefreshing = value;
                this.NotifyPropertyChanged();
            }
        }

        private string _loaderMessage;

        public string LoaderMessage
        {
            get { return this._loaderMessage; }
            set
            {
                this._loaderMessage = value;
                this.NotifyPropertyChanged();
            }
        }

        public View View { set; get; }

        public List<MobilePagesWraper> ObjectList { private set; get; }

        public Command SyncButtonCommand => new Command(OnSyncClick);

        public Command ObjectSelectCommand => new Command(OnObjectClick);

        public Command RefreshCommand => new Command(async () => await OnRefreshPulled());

        public ObjectsRenderViewModel()
        {
            LoaderMessage = "Opening page...";
            PageTitle = Settings.AppName;

            SetUpData();
        }

        private void SetUpData()
        {
            string _objlist = Store.GetValue(AppConst.OBJ_COLLECTION);

            if (_objlist == null)
            {
                bool _pull = this.PulledTableExist() ? false : true;

                MobilePageCollection Coll = Api.GetEbObjects(Settings.AppId, Settings.LocationId, _pull);
                this.ObjectList = Coll.Pages;

                Store.SetValue(AppConst.OBJ_COLLECTION, JsonConvert.SerializeObject(Coll.Pages));

                if (Coll.TableNames != null)
                {
                    Store.SetValue(string.Format(AppConst.APP_PULL_TABLE, Settings.AppId), string.Join(",", Coll.TableNames.ToArray()));

                    if (_pull == true && Coll.TableNames.Count > 0)
                        this.LoadDTAsync(Coll.Data);
                }
            }
            else
            {
                List<MobilePagesWraper> _list = JsonConvert.DeserializeObject<List<MobilePagesWraper>>(_objlist);
                this.ObjectList = _list;
            }
        }

        private bool PulledTableExist()
        {
            string sv = Store.GetValue(string.Format(AppConst.APP_PULL_TABLE, Settings.AppId));

            string[] Tables = (sv == null) ? new string[0] : sv.Split(',');

            if (Tables.Length <= 0)
                return false;

            foreach (string s in Tables)
            {
                var status = App.DataDB.DoScalar(string.Format(StaticQueries.TABLE_EXIST, s));
                if (Convert.ToInt32(status) <= 0)
                    return false;
            }
            return true;
        }

        private async void LoadDTAsync(EbDataSet DS)
        {
            if (DS.Tables.Count > 0)
                await CommonServices.Instance.LoadLocalData(DS);
        }

        private void OnSyncClick(object sender)
        {
            IToast toast = DependencyService.Get<IToast>();

            if (Settings.HasInternet)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Device.BeginInvokeOnMainThread(() => { IsBusy = true; LoaderMessage = "Pushing from local data..."; });

                        bool status = SyncServices.Instance.Sync();

                        if (status)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                IsBusy = false;
                                toast.Show("Push Completed.");
                            });
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                IsBusy = false;
                                toast.Show(SyncServices.Instance.Message);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            }
            else
                toast.Show("You are not connected to internet !");
        }

        private void OnObjectClick(object obj)
        {
            MobilePagesWraper item = (obj as MobilePagesWraper);
            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() => { IsBusy = true; LoaderMessage = "Loading page..."; });
                try
                {
                    EbMobilePage page = HelperFunctions.GetPage(item.RefId);
                    if (page == null)
                        (Application.Current.MainPage as MasterDetailPage).Detail.DisplayAlert("Alert!", "This page is no longer available.", "Ok");

                    if (page.Container is EbMobileForm)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            IsBusy = false;
                            (Application.Current.MainPage as MasterDetailPage).Detail.Navigation.PushAsync(new FormRender(page));
                        });
                    }
                    else if (page.Container is EbMobileVisualization)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            IsBusy = false;
                            (Application.Current.MainPage as MasterDetailPage).Detail.Navigation.PushAsync(new ListViewRender(page));
                        });
                    }
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() => IsBusy = false);
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }

        private async Task OnRefreshPulled()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DependencyService.Get<IToast>().Show("Not connected to internet!");
                return;
            }

            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));

            Store.Remove(AppConst.OBJ_COLLECTION);
            SetUpData();
            this.IsRefreshing = false;
            DependencyService.Get<IToast>().Show("Pulled successfully");
        }
    }
}
