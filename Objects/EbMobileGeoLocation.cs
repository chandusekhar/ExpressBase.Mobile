﻿using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Structures;
using System;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ExpressBase.Mobile
{
    public class EbMobileGeoLocation : EbMobileControl
    {
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public bool HideSearchBox { set; get; }

        private Location Cordinates { set; get; }

        public string Place { set; get; }

        private WebView WebView { set; get; }

        private string PlaceHolder => @"<html><head><style>body{height:100%;width:100%;display:flex;justify-content:center;
                                    align-items:center;flex-direction:column}</style></head><body style='background:#eee'> 
                                    <container style='line-height:1.5'><p style='text-align: center;'>
                                    You are not connected to internet</p> </container></body></html>";

        public override void InitXControl(FormMode Mode)
        {
            this.BuildXControl();

            if (Settings.HasInternet)
            {
                if (Mode != FormMode.EDIT)
                    this.SetCordinates();
            }
            else
            {
                WebView.HeightRequest = 100;
                WebView.Source = new HtmlWebViewSource { Html = PlaceHolder };
            } 
        }

        private void BuildXControl()
        {
            Frame _frame = new Frame()
            {
                HasShadow = false,
                Padding = new Thickness(0, 0, 0, 10),
                CornerRadius = 10.0f,
            };

            WebView = new WebView { HeightRequest = 300 };
            _frame.Content = WebView;

            this.XControl = _frame;
        }

        private async void SetCordinates()
        {
            if (Cordinates == null)
            {
                Cordinates = await GeoLocation.Instance.GetCurrentGeoLocation();

                if (Cordinates != null)
                {
                    Placemark plc = await GeoLocation.Instance.GetAddressByCordinates(Cordinates.Latitude, Cordinates.Longitude);
                    if (plc != null)
                    {
                        Place = $"{plc.FeatureName},{plc.Locality},{plc.Locality}";
                        SetWebViewUrl(Cordinates.Latitude, Cordinates.Longitude, this.Place);
                    }
                }
            }
            else
                SetWebViewUrl(Cordinates.Latitude, Cordinates.Longitude, this.Place);
        }

        private void SetWebViewUrl(double lat, double lon, string place)
        {
            string url = $"{Settings.RootUrl}/api/map?bToken={AppConst.BTOKEN}&rToken={AppConst.RTOKEN}&type=GOOGLEMAP&latitude={lat}&longitude={lon}&place={place}";
            this.WebView.Source = new UrlWebViewSource { Url = url };
            WebView.Reload();
        }

        public override object GetValue()
        {
            try
            {
                Uri uri = new Uri((WebView.Source as UrlWebViewSource).Url);
                var query = HttpUtility.ParseQueryString(uri.Query);

                double lat = Convert.ToDouble(query.Get("latitude"));
                double lon = Convert.ToDouble(query.Get("longitude"));

                if (Cordinates == null)
                    Cordinates = new Location { Latitude = lat, Longitude = lon };

                return $"{lat},{lon}";
            }
            catch (Exception ex)
            {
                Log.Write("EbMobileGeoLocation.GetValue" + ex.Message);
            }
            return null;
        }

        public override bool SetValue(object value)
        {
            if (value == null)
                return false;

            string[] cordinates = (value as string).Split(',');
            if (cordinates.Length >= 2)
            {
                double lat = Convert.ToDouble(cordinates[0]);
                double lng = Convert.ToDouble(cordinates[1]);

                Task.Run(async () =>
                {
                    Placemark plc = await GeoLocation.Instance.GetAddressByCordinates(lat, lng);
                    this.Place = $"{plc.FeatureName},{plc.Locality},{plc.Locality}";

                    SetWebViewUrl(lat, lng, this.Place);
                });
            }
            return true;
        }
    }
}
