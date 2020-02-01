﻿using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Data;
using ExpressBase.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ExpressBase.Mobile
{
    public class EbMobileDashBoardControls : EbMobilePageBase
    {
        public virtual View XView { set; get; }

        public virtual void InitXControl() { }

        public virtual void InitXControl(EbDataRow DataRow) { }
    }

    public class EbMobileTableView : EbMobileDashBoardControls
    {
        public string DataSourceRefId { set; get; }

        public EbScript OfflineQuery { set; get; }

        //mob prop
        private EbDataTable Data { set; get; }

        private EbDataRow LinkedDataRow { set; get; }

        public override View XView { set; get; }

        public override void InitXControl(EbDataRow DataRow)
        {
            LinkedDataRow = DataRow;
            SetData();
            InitXView();
        }

        private void InitXView()
        {
            this.XView = new CustomShadowFrame
            {
                HasShadow = true,
                CornerRadius = 4,
                Content = new WebView
                {
                    Source = new HtmlWebViewSource
                    {
                        Html = $"<html><body>{GetHtml()}</body></html>"
                    }
                }
            };
        }

        private void SetData()
        {
            try
            {
                byte[] b = Convert.FromBase64String(this.OfflineQuery.Code);
                string sql = HelperFunctions.WrapSelectQuery(System.Text.Encoding.UTF8.GetString(b));

                List<DbParameter> _DbParams = new List<DbParameter>();

                List<string> _Params = HelperFunctions.GetSqlParams(sql);

                if (_Params.Count > 0)
                {
                    this.GetParameterValues(_DbParams, _Params);
                }
                Data = App.DataDB.DoQuery(sql, _DbParams.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void GetParameterValues(List<DbParameter> _DbParams, List<string> _Params)
        {
            try
            {
                foreach (string _p in _Params)
                {
                    _DbParams.Add(new DbParameter
                    {
                        ParameterName = _p,
                        Value = this.LinkedDataRow[_p],
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string GetHtml()
        {
            string Html = "<table><thead><tr>";

            foreach (EbDataColumn col in this.Data.Columns)
            {
                Html += $"<th style='border:1px solid #333'>{col.ColumnName}</th>";
            }

            Html += $"</tr></thead><tbody>";

            foreach (EbDataRow row in this.Data.Rows)
            {
                Html += "<tr>";
                foreach (object item in row)
                {
                    Html += $"<td style='border:1px solid #333'>{item.ToString()}</td>";
                }
                Html += "</tr>";
            }

            Html += "</tbody></table>";

            return Html;
        }
    }
}
