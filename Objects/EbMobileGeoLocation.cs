﻿using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Structures;
using System;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ExpressBase.Mobile
{
    public class EbMobileGeoLocation : EbMobileControl
    {
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public bool HideSearchBox { set; get; }

        private Location cordinates;

        private WebView webView;

        private ActivityIndicator loader;

        private string PlaceHolder => @"<html><head><style>body{height:100%;width:100%;display:flex;justify-content:center;
                                    align-items:center;flex-direction:column}</style></head><body style='background:#eee'> 
                                    <container style='line-height:1.5'><p style='text-align: center;'>
                                    @content@ </p> </container></body></html>";

        public override void InitXControl(FormMode Mode, NetworkMode Network)
        {
            base.InitXControl(Mode, Network);

            try
            {
                this.BuildXControl();

                if (this.NetworkType == NetworkMode.Online)
                {
                    if (Mode != FormMode.EDIT)
                        this.SetCordinates();
                }
                else if (this.NetworkType == NetworkMode.Mixed)
                {
                    if (Mode != FormMode.EDIT)
                    {
                        if (Utils.HasInternet)
                            this.SetCordinates();
                        else
                        {
                            loader.IsVisible = false;
                            webView.HeightRequest = 100;
                            webView.Source = new HtmlWebViewSource
                            {
                                Html = PlaceHolder.Replace("@content@", "You are not connected to internet")
                            };
                            webView.IsVisible = true;
                        }
                    }
                }
                else
                {
                    loader.IsVisible = false;
                    webView.HeightRequest = 100;
                    webView.Source = new HtmlWebViewSource
                    {
                        Html = PlaceHolder.Replace("@content@", "Offline")
                    };
                    webView.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
        }

        private void BuildXControl()
        {
            try
            {
                loader = new ActivityIndicator
                {
                    Style = (Style)HelperFunctions.GetResourceValue("GeoLocLoader")
                };

                Frame _frame = new Frame()
                {
                    Style = (Style)HelperFunctions.GetResourceValue("GeoLocFrame")
                };

                StackLayout Layout = new StackLayout
                {
                    Margin = new Thickness(-10, 0),
                    BackgroundColor = Color.Transparent,
                    Children = { loader }
                };
                webView = new WebView { HeightRequest = 300, IsVisible = false };
                webView.Navigated += WebView_Navigated;
                Layout.Children.Add(webView);
                _frame.Content = Layout;

                this.XControl = Layout;
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            loader.IsVisible = false;
            webView.IsVisible = true;
        }

        private async void SetCordinates()
        {
            try
            {
                cordinates = await GeoLocation.Instance.GetCurrentGeoLocation();
                if (cordinates != null)
                {
                    this.SetWebViewUrl(cordinates.Latitude, cordinates.Longitude);
                }
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
        }

        private void SetWebViewUrl(double lat, double lon)
        {
            try
            {
                string url = $"{App.Settings.RootUrl}/api/map?bToken={App.Settings.BToken}&rToken={App.Settings.RToken}&type=GOOGLEMAP&latitude={lat}&longitude={lon}";
                this.webView.Source = new UrlWebViewSource { Url = url };
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
        }

        public override object GetValue()
        {
            try
            {
                if (webView.Source is UrlWebViewSource)
                {
                    Uri uri = new Uri((webView.Source as UrlWebViewSource).Url);
                    var query = HttpUtility.ParseQueryString(uri.Query);

                    double lat = Convert.ToDouble(query.Get("latitude"));
                    double lon = Convert.ToDouble(query.Get("longitude"));

                    if (cordinates == null)
                        cordinates = new Location { Latitude = lat, Longitude = lon };

                    return $"{lat},{lon}";
                }
            }
            catch (Exception ex)
            {
                EbLog.Error("EbMobileGeoLocation.GetValue Failed :: " + ex.Message);
            }
            return null;
        }

        public override void SetValue(object value)
        {
            try
            {
                if (value != null)
                {
                    string[] cordinates = (value as string).Split(',');
                    if (cordinates.Length >= 2)
                    {
                        double lat = Convert.ToDouble(cordinates[0]);
                        double lng = Convert.ToDouble(cordinates[1]);

                        this.SetWebViewUrl(lat, lng);
                    }
                }
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
                EbLog.Error(ex.StackTrace);
            }
        }

        public override bool Validate()
        {
            string value = this.GetValue() as string;

            if (this.Required && string.IsNullOrEmpty(value))
                return false;

            return true;
        }
    }
}
