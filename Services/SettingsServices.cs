﻿using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Mobile.Services
{
    public class SettingsServices
    {
        public SolutionInfo CurrentSolution { set; get; }

        public User CurrentUser { set; get; }

        public string Sid
        {
            get { return CurrentSolution?.SolutionName; }
        }

        public string RootUrl
        {
            get { return "https://" + CurrentSolution?.RootUrl; }
        }

        public string UserName
        {
            get { return CurrentUser?.Email; }
        }

        public string UserDisplayName
        {
            get { return CurrentUser?.FullName; }
        }

        public int UserId
        {
            get { return CurrentUser.UserId; }
        }

        public string RToken { set; get; }

        public string BToken { set; get; }

        public AppData CurrentApplication { set; get; }

        public int AppId
        {
            get
            {
                if (CurrentApplication != null)
                    return CurrentApplication.AppId;
                else
                    return 0;
            }
        }

        // Summary:
        //     Gets the current loc object.
        public EbLocation CurrentLocation { set; get; }

        // Summary:
        //     Gets the current location id from current loc object.
        public int CurrentLocId
        {
            get { return CurrentLocation.LocId; }
        }

        public async Task Resolve()
        {
            try
            {
                CurrentSolution = this.GetSolution();

                if(CurrentSolution != null)
                {
                    App.DataDB.SetDbPath(CurrentSolution.SolutionName);
                }

                RToken = await GetRToken();
                BToken = await GetBToken();

                CurrentApplication = GetCurrentApplication();

                CurrentUser = GetUser();
                CurrentLocation = GetCurrentLocation();
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        private SolutionInfo GetSolution()
        {
            return Store.GetJSON<SolutionInfo>(AppConst.SOLUTION_OBJ);
        }

        private AppData GetCurrentApplication()
        {
            return Store.GetJSON<AppData>(AppConst.CURRENT_APP);
        }

        private async Task<string> GetRToken()
        {
            return await Store.GetValueAsync(AppConst.RTOKEN);
        }

        private async Task<string> GetBToken()
        {
            return await Store.GetValueAsync(AppConst.BTOKEN);
        }

        private User GetUser()
        {
            return Store.GetJSON<User>(AppConst.USER_OBJECT);
        }

        private EbLocation GetCurrentLocation()
        {
            return Store.GetJSON<EbLocation>(AppConst.CURRENT_LOCOBJ);
        }
    }
}