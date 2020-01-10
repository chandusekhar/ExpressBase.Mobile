﻿using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Services;
using ExpressBase.Mobile.Views;
using Xamarin.Forms;
using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Data;
using ExpressBase.Mobile.Helpers;

namespace ExpressBase.Mobile
{
    public partial class App : Application
    {
        public static string DbPath { set; get; }

        public static IDataBase DataDB { get; set; }

        public App(string dbPath)
        {
            InitializeComponent();

            DbPath = dbPath;

            if (DataDB == null)
            {
                DataDB = DependencyService.Get<IDataBase>();
            }

            this.InitNavigation();
        }

        public App()
        {
            InitializeComponent();
            DataDB = DependencyService.Get<IDataBase>();
            this.InitNavigation();
        }

        void InitNavigation()
        {
            MainPage = new NavigationPage
            {
                BarBackgroundColor = Color.FromHex("315eff"),
                BarTextColor = Color.White
            };

            string sid = Store.GetValue(AppConst.SID);
            if (sid == null)
            {
                MainPage.Navigation.PushAsync(new SolutionSelect());
            }
            else
            {
                string rtoken = Store.GetValue(AppConst.RTOKEN);
                if (rtoken != null)
                {
                    if (Auth.IsTokenExpired(rtoken))
                    {
                        string username = Store.GetValue(AppConst.USERNAME);
                        string password = Store.GetValue(AppConst.PASSWORD);
                        ApiAuthResponse authresponse = Auth.TryAuthenticate(username, password);
                        if (authresponse.IsValid)
                        {
                            MainPage = new RootMaster(typeof(ObjectsRenderer));
                        }
                        else
                        {
                            MainPage.Navigation.PushAsync(new Login());
                        }
                    }
                    else
                    {
                        string apid = Store.GetValue(AppConst.APPID);

                        if (apid == null)
                            MainPage.Navigation.PushAsync(new AppSelect());
                        else
                            MainPage = new RootMaster(typeof(ObjectsRenderer));
                    }
                }
                else
                    MainPage.Navigation.PushAsync(new Login());
            }
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
