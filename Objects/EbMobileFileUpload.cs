﻿using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Views.Dynamic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace ExpressBase.Mobile
{
    public class EbMobileFileUpload : EbMobileControl, INonPersistControl
    {
        public bool EnableCameraSelect { set; get; }

        public bool EnableFileSelect { set; get; }

        public virtual bool MultiSelect { set; get; }

        public virtual bool EnableEdit { set; get; }

        protected FileUploader XamControl;

        private List<FileMetaInfo> uploadedFileRef;

        public override void InitXControl(FormMode Mode, NetworkMode Network)
        {
            base.InitXControl(Mode, Network);

            XamControl = new FileUploader();
            XamControl.Initialize(this);
            XamControl.BindFullScreenCallback(ShowFullScreen);
            XamControl.BindDeleteCallback(DeleteFile);
            this.XControl = XamControl;
        }

        public void ShowFullScreen(Image image)
        {
            Page navigator = (Application.Current.MainPage as MasterDetailPage).Detail;
            Page current = (navigator as NavigationPage).CurrentPage;
            if (current is FormRender)
            {
                (current as FormRender).ShowFullScreenImage(image);
            }
        }

        public void DeleteFile()
        {

        }

        public override MobileTableColumn GetMobileTableColumn()
        {
            return null;
        }

        public void PushFilesToDir(string TableName, int RowId)
        {
            INativeHelper helper = DependencyService.Get<INativeHelper>();

            List<FileWrapper> files = XamControl.GetFiles(this.Name);

            foreach (FileWrapper wrapr in files)
            {
                wrapr.Name = $"{TableName}-{RowId}-{this.Name}-{Guid.NewGuid().ToString("n").Substring(0, 10)}.jpg";
                File.WriteAllBytes(helper.NativeRoot + $"/{App.Settings.AppDirectory}/{ App.Settings.Sid.ToUpper()}/FILES/{wrapr.Name}", wrapr.Bytea);
            }
        }

        public override object GetValue()
        {
            List<FileWrapper> files = XamControl.GetFiles(this.Name);

            if (uploadedFileRef != null && files.Any())
            {
                foreach (var meta in uploadedFileRef)
                {
                    files.Add(new FileWrapper
                    {
                        FileRefId = meta.FileRefId,
                        FileName = meta.FileName,
                        IsUploaded = true,
                        ControlName = Name,
                    });
                }
            }
            return files;
        }

        public override void SetValue(object value)
        {
            if (value != null)
            {
                uploadedFileRef = (value as FUPSetValueMeta).Files;

                XamControl.SetValue(this.NetworkType, value as FUPSetValueMeta, this.Name);
            }
        }

        public override bool Validate()
        {
            List<FileWrapper> files = this.GetValue() as List<FileWrapper>;

            if (this.Required && !files.Any())
                return false;

            return true;
        }
    }
}
