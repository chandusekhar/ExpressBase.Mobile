﻿using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExpressBase.Mobile.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SideBar : ContentPage
    {
        public SideBar()
        {
            InitializeComponent();

            var _user = Utils.UserObject;
            UserName.Text = _user.FullName;
            Email.Text = _user.Email;
            this.SetDp();
        }

        private void SetDp()
        {
            INativeHelper helper = DependencyService.Get<INativeHelper>();
            string sid = Utils.SolutionId;
            try
            {
                var bytes = helper.GetPhoto($"ExpressBase/{sid}/user.png");
                if (bytes != null)
                    UserDp.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch (Exception ex)
            {
                Log.Write("SideBar.SetDp---" + ex.Message);
            }
        }

        private void About_Tapped(object sender, EventArgs e)
        {
            App.RootMaster.IsPresented = false;
            App.RootMaster.Detail.Navigation.PushAsync(new About());
        }

        private void ChangeSolution_Tapped(object sender, EventArgs e)
        {
            try
            {
                Application.Current.MainPage = new NavigationPage(new MySolutions())
                {
                    BarBackgroundColor = Color.FromHex("0046bb"),
                    BarTextColor = Color.White
                };
                App.RootMaster.IsPresented = false;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        private async void ChangeApplication_Tapped(object sender, EventArgs e)
        {
            try
            {
                App.RootMaster.IsPresented = false;
                await App.RootMaster.Detail.Navigation.PushAsync(new MyApplications(true));
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        private async void ChangeLocation_Tapped(object sender, EventArgs e)
        {
            try
            {
                App.RootMaster.IsPresented = false;
                await App.RootMaster.Detail.Navigation.PushAsync(new MyLocations());
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        private void Logout_Tapped(object sender, EventArgs e)
        {
            try
            {
                App.RootMaster.IsPresented = false;
                Page navigator = (Application.Current.MainPage as MasterDetailPage).Detail;
                Home current = (navigator as NavigationPage).CurrentPage as Home;
                current.ConfirmLogout();
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        private void Setup_Tapped(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }
    }
}