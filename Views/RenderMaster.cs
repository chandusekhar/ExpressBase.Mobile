﻿using ExpressBase.Mobile.Common.Structures;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Views.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ExpressBase.Mobile.Views
{
    class RenderMaster : MasterDetailPage
    {
        SideBar Sidebar;

        public RenderMaster(EbObjectToMobResponse wraper)
        {
            Sidebar = new SideBar();
            Master = Sidebar;

            Sidebar.listView.ItemSelected += OnItemSelected;

            EbObjectWrapper obj_wrapr = wraper.ObjectWraper;

            if (obj_wrapr.EbObjectType == (int)EbObjectTypes.WebForm)
            {
                Detail = new NavigationPage(new FormRender(obj_wrapr));
            }
            else if(obj_wrapr.EbObjectType == (int)EbObjectTypes.Report)
            {
                Detail = new NavigationPage(new ReportRender());
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            Application.Current.MainPage = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
        }
    }
}
