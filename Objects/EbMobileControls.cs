﻿using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Structures;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace ExpressBase.Mobile
{
    public abstract class EbMobileControl : EbMobilePageBase
    {
        public static readonly Color DefaultBorder = Color.FromHex("cccccc");

        public static readonly Color ValidationError = Color.Red;

        public static readonly Color ReadOnlyBackground = Color.FromHex("eeeeee");


        public virtual string Label { set; get; }

        public virtual EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public virtual bool Hidden { set; get; }

        public virtual EbScript HiddenExpr { get; set; }

        public virtual bool Unique { get; set; }

        public virtual bool ReadOnly { get; set; }

        public virtual EbScript DisableExpr { get; set; }

        public virtual bool DoNotPersist { get; set; }

        public virtual bool Required { get; set; }

        public virtual EbScript ValueExpr { get; set; }

        public virtual EbScript DefaultValueExpression { get; set; }

        public virtual List<EbMobileValidator> Validators { get; set; }

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

        public virtual object SQLiteToActual(object value) { return value; }

        public View XControl { set; get; }

        protected Color XBackground => this.ReadOnly ? ReadOnlyBackground : Color.Transparent;

        public virtual void InitXControl() { }

        public virtual void InitXControl(FormMode mode, NetworkMode network)
        {
            this.FormRenderMode = mode;
            this.NetworkType = network;
        }

        private Label validationLabel;

        private StackLayout xview;

        public virtual StackLayout XView
        {
            get
            {
                if (xview == null)
                {
                    var formatted = new FormattedString { Spans = { new Span { Text = this.Label } } };

                    if (this.Required)
                        formatted.Spans.Add(new Span { Text = " *", FontSize = 16, TextColor = Color.Red });

                    validationLabel = new Label { Style = (Style)HelperFunctions.GetResourceValue("ControlValidationLable") };

                    xview = new StackLayout
                    {
                        Padding = new Thickness(15, 10, 15, 10),
                        IsVisible = !(this.Hidden),
                        Children = { new Label { FormattedText = formatted }, XControl, validationLabel }
                    };
                }
                return xview;
            }
        }

        public FormMode FormRenderMode { set; get; }

        public NetworkMode NetworkType { set; get; }

        public bool DefaultExprEvaluated { set; get; }

        public virtual object GetValue() { return null; }

        public virtual void SetValue(object value) { }

        public virtual void SetAsReadOnly(bool disable)
        {
            this.XControl.IsEnabled = !disable;
        }

        public virtual void SetVisibilty(bool visible)
        {
            this.XView.IsVisible = visible;
        }

        public virtual void Reset() { }

        public virtual bool Validate()
        {
            if (this.Required && GetValue() == null)
                return false;

            return true;
        }

        public virtual void ValueChanged(string source = null)
        {
            if (source != null && EbFormHelper.ContainsInValExpr(this.Name, source))
                return;

            EbFormHelper.ControlValueChanged(this.Name);
        }

        public virtual MobileTableColumn GetMobileTableColumn()
        {
            object value = this.GetValue();

            if (value == null) return null;

            return new MobileTableColumn
            {
                Name = this.Name,
                Type = this.EbDbType,
                Value = value
            };
        }

        protected Dictionary<string, string> ScriptMethodMap = new Dictionary<string, string>
        {
            { "getValue","GetValue"}
        };

        public object InvokeDynamically(string method, object[] parameters = null)
        {
            try
            {
                if (!ScriptMethodMap.TryGetValue(method, out string member))
                {
                    throw new Exception($"Invalid method found : '{method}()'");
                }

                MethodInfo info = this.GetType().GetMethod(member);
                return info.Invoke(this, parameters);
            }
            catch (Exception ex)
            {
                EbLog.Info("Dynamic invokation failed in eb control : " + this.Name);
                EbLog.Info(ex.Message);
            }
            return null;
        }

        public virtual void SetValidation(bool status, string message)
        {
            if (validationLabel == null) return;

            validationLabel.Text = message;
            validationLabel.IsVisible = !status;
        }
    }
}
