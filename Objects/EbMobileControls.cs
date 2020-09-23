﻿using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Structures;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ExpressBase.Mobile
{
    public class EbMobileControl : EbMobilePageBase
    {
        public virtual string Label { set; get; }

        public virtual EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public virtual bool Hidden { set; get; }

        public virtual bool Unique { get; set; }

        public virtual bool ReadOnly { get; set; }

        public virtual bool DoNotPersist { get; set; }

        public virtual bool Required { get; set; }

        public string SQLiteType
        {
            get
            {
                if (this.EbDbType == EbDbTypes.String)
                    return "TEXT";
                else if (this.EbDbType == EbDbTypes.Int16 || this.EbDbType == EbDbTypes.Int32)
                    return "INT";
                else if (this.EbDbType == EbDbTypes.Decimal || this.EbDbType == EbDbTypes.Double)
                    return "REAL";
                else if (this.EbDbType == EbDbTypes.Date || this.EbDbType == EbDbTypes.DateTime)
                    return "DATETIME";
                else if (this.EbDbType == EbDbTypes.Boolean)
                    return "INT";
                else
                    return "TEXT";
            }
        }

        //mobile prop
        public virtual object SQLiteToActual(object value) { return value; }

        //mobile prop
        public View XControl { set; get; }

        protected Color XBackground => this.ReadOnly ? Color.FromHex("eeeeee") : Color.Transparent;

        public virtual void InitXControl() { }

        public virtual void InitXControl(FormMode mode, NetworkMode network)
        {
            this.FormRenderMode = mode;
            this.NetworkType = network;
        }

        public virtual StackLayout XView
        {
            get
            {
                var formatted = new FormattedString { Spans = { new Span { Text = this.Label } } };

                if (this.Required)
                    formatted.Spans.Add(new Span { Text = " *", FontSize = 16, TextColor = Color.Red });

                return new StackLayout
                {
                    Padding = new Thickness(15, 10, 15, 10),
                    IsVisible = !(this.Hidden),
                    Children =
                    {
                        new Label { FormattedText =  formatted },
                        XControl
                    }
                };
            }
        }

        public FormMode FormRenderMode { set; get; }

        public NetworkMode NetworkType { set; get; }

        public virtual object GetValue() { return null; }

        public virtual void SetValue(object value) { }

        public virtual void SetAsReadOnly(bool Enable)
        {
            if (Enable == true)
                this.XControl.IsEnabled = false;
            else
                this.XControl.IsEnabled = true;
        }

        public virtual void Reset() { }

        public virtual bool Validate()
        {
            if (this.Required && GetValue() == null)
                return false;

            return true;
        }

        public virtual MobileTableColumn GetMobileTableColumn()
        {
            object value = this.GetValue();
            if (value == null)
                return null;

            return new MobileTableColumn
            {
                Name = this.Name,
                Type = this.EbDbType,
                Value = value
            };
        }
    }

    public class EbMobileTableLayout : EbMobileControl, ILayoutControl
    {
        public int RowCount { set; get; }

        public int ColumCount { set; get; }

        public List<EbMobileTableCell> CellCollection { set; get; }

        public override bool Hidden { set; get; }

        public override bool Unique { get; set; }

        public EbMobileTableLayout()
        {
            this.CellCollection = new List<EbMobileTableCell>();
        }
    }

    public class EbMobileTableCell : EbMobilePageBase
    {
        public int RowIndex { set; get; }

        public int ColIndex { set; get; }

        public int Width { set; get; }

        public List<EbMobileControl> ControlCollection { set; get; }

        public EbMobileTableCell()
        {
            this.ControlCollection = new List<EbMobileControl>();
        }

        public bool IsEmpty()
        {
            return this.ControlCollection.Count <= 0;
        }
    }
}
