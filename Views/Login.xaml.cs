﻿using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExpressBase.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();

            SolutionName.Text = Settings.SolutionId;
            BindingContext = new LoginViewModel();
        }

        private void Email_Completed(object sender, EventArgs e)
        {
            PassWord.Focus();
        }
    }
}