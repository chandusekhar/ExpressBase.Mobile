﻿using ExpressBase.Mobile.Data;
using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Extensions;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using ExpressBase.Mobile.Services;
using ExpressBase.Mobile.Structures;
using System;
using System.Collections.Generic;

namespace ExpressBase.Mobile
{
    public class DbTypedValue
    {
        public EbDbTypes Type { set; get; }

        private object _value;

        public object Value
        {
            set { _value = value; }
            get
            {
                if (Type == EbDbTypes.DateTime)
                    return Convert.ToDateTime(_value).ToString("yyyy-MM-dd");
                else if (Type == EbDbTypes.Date)
                    return Convert.ToDateTime(_value).ToString("yyyy-MM-dd");
                else
                    return _value;
            }
        }

        public DbTypedValue() { }

        public DbTypedValue(string Name, object Value, EbDbTypes Type)
        {
            if (Name == "eb_created_at_device")
                this.Type = EbDbTypes.DateTime;
            else
            {
                this.Type = Type;
                this.Value = Value;
            }
        }
    }

    public class EbMobileContainer : EbMobilePageBase
    {

    }

    public class EbMobileVisualization : EbMobileContainer
    {
        public string DataSourceRefId { set; get; }

        public string SourceFormRefId { set; get; }

        public EbScript OfflineQuery { set; get; }

        public EbMobileTableLayout DataLayout { set; get; }

        public string LinkRefId { get; set; }

        public List<EbMobileDataColumn> Filters { set; get; }

        public WebFormDVModes FormMode { set; get; }

        public int PageLength { set; get; } = 30;

        //mobile property
        public string GetQuery { get { return HelperFunctions.B64ToString(this.OfflineQuery.Code); } }

        public EbDataSet GetData(int offset = 0)
        {
            EbDataSet Data = new EbDataSet();
            try
            {
                string sql = HelperFunctions.WrapSelectQuery(GetQuery);

                DbParameter[] dbParameters = {
                    new DbParameter{ ParameterName = "@limit", Value = PageLength, DbType = (int)EbDbTypes.Int32 },
                    new DbParameter{ ParameterName = "@offset", Value = offset, DbType = (int)EbDbTypes.Int32 },
                };

                Data = App.DataDB.DoQueries(sql, dbParameters);
            }
            catch (Exception ex)
            {
                Log.Write("EbMobileVisualization.GetData---" + ex.Message);
            }
            return Data;
        }

        public EbDataSet GetData(List<DbParameter> dbParameters, int offset = 0)
        {
            EbDataSet Data = new EbDataSet();
            try
            {
                var userParam = dbParameters.Find(item => item.ParameterName == "current_userid");

                if (userParam != null)
                {
                    userParam.Value = Settings.UserId;
                    userParam.DbType = (int)EbDbTypes.Int32;
                }

                string sql = HelperFunctions.WrapSelectQuery(GetQuery, dbParameters);

                dbParameters.Add(new DbParameter { ParameterName = "@limit", Value = PageLength, DbType = (int)EbDbTypes.Int32 });
                dbParameters.Add(new DbParameter { ParameterName = "@offset", Value = offset, DbType = (int)EbDbTypes.Int32 });

                Data = App.DataDB.DoQueries(sql, dbParameters.ToArray());
            }
            catch (Exception ex)
            {
                Log.Write("EbMobileVisualization.GetData with params---" + ex.Message);
            }
            return Data;
        }
    }

    public class EbMobileDashBoard : EbMobileContainer
    {
        public List<EbMobileDashBoardControls> ChiledControls { set; get; }

        public List<EbMobileDashBoardControls> ChildControls { get { return ChiledControls; } set { } }
    }
}
