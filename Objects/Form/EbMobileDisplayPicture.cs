﻿using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Structures;
using ExpressBase.Mobile.Views.Base;
using Xamarin.Forms;

namespace ExpressBase.Mobile
{
    public class EbMobileDisplayPicture : EbMobileControl, IFileUploadControl
    {
        public bool EnableCameraSelect { set; get; }

        public bool EnableFileSelect { set; get; }

        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        private FileUploader XamControl;

        public override void InitXControl(FormMode Mode, NetworkMode Network)
        {
            base.InitXControl(Mode, Network);

            XamControl = new FileUploader(this);
            XamControl.BindFullScreenCallback(ShowFullScreen);
            this.XControl = XamControl;
        }

        public void ShowFullScreen(Image image)
        {
            if (App.Navigation.GetCurrentPage() is IFormRenderer rendrer)
                rendrer.ShowFullScreenImage(image.Source);
        }

        public override object GetValue()
        {
            return XamControl.GetFiles(this.Name);
        }

        public override MobileTableColumn GetMobileTableColumn()
        {
            return new MobileTableColumn
            {
                Name = this.Name,
                Type = this.EbDbType
            };
        }

        public override void SetValue(object value)
        {
            if (value != null)
            {
                XamControl.SetValue(this.NetworkType, value as FUPSetValueMeta, this.Name);
            }
        }

        public override bool Validate()
        {
            return base.Validate();
        }
    }
}