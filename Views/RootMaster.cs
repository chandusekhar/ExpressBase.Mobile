﻿using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.ViewModels;
using ExpressBase.Mobile.Views.Shared;
using System;
using Xamarin.Forms;

namespace ExpressBase.Mobile.Views
{
    class RootMaster : MasterDetailPage
    {
        public RootMaster(Type pageType)
        {
            try
            {
                Master = new SideBar(); 
                Detail = new NavigationPage
                {
                    BarBackgroundColor = Color.FromHex("0046bb"),
                    BarTextColor = Color.White
                };

                Detail.Navigation.PushAsync((Page)Activator.CreateInstance(pageType));
            }
            catch(Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
    }
}
