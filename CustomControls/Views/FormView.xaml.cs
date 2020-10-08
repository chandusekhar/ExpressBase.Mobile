﻿using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Helpers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExpressBase.Mobile.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormView : ContentView
    {
        public static readonly BindableProperty ControlsProperty = BindableProperty.Create("Controls", typeof(List<EbMobileControl>), typeof(ContentView));

        public static readonly BindableProperty NetWorkTypeProperty = BindableProperty.Create("NetWorkType", typeof(NetworkMode), typeof(ContentView));

        public static readonly BindableProperty FormModeProperty = BindableProperty.Create("FormMode", typeof(FormMode), typeof(ContentView));

        public List<EbMobileControl> Controls
        {
            get { return (List<EbMobileControl>)GetValue(ControlsProperty); }
            set { SetValue(ControlsProperty, value); }
        }

        public NetworkMode NetWorkType
        {
            get { return (NetworkMode)GetValue(NetWorkTypeProperty); }
            set { SetValue(NetWorkTypeProperty, value); }
        }

        public FormMode FormMode
        {
            get { return (FormMode)GetValue(FormModeProperty); }
            set { SetValue(FormModeProperty, value); }
        }

        public FormView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            this.Render();
        }

        private void Render()
        {
            try
            {
                FormViewContainer.Children.Clear();

                foreach (EbMobileControl ctrl in Controls)
                {
                    if (ctrl is EbMobileTableLayout)
                    {
                        foreach (EbMobileTableCell Tc in (ctrl as EbMobileTableLayout).CellCollection)
                        {
                            foreach (EbMobileControl tbctrl in Tc.ControlCollection)
                            {
                                tbctrl.InitXControl(this.FormMode, this.NetWorkType);
                                FormViewContainer.Children.Add(ctrl.XView);
                            }
                        }
                    }
                    else
                    {
                        ctrl.InitXControl(this.FormMode, this.NetWorkType);
                        FormViewContainer.Children.Add(ctrl.XView);
                    }
                }
            }
            catch(Exception ex)
            {
                EbLog.Error(ex.Message);
            }
        }
    }
}