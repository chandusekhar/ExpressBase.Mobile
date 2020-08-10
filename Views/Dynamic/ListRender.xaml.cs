﻿using ExpressBase.Mobile.ViewModels.Dynamic;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.CustomControls;

namespace ExpressBase.Mobile.Views.Dynamic
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListRender : ContentPage, IRefreshable
    {
        private int pageCount = 1;

        private bool isRendered;

        private readonly ListViewModel viewModel;

        private bool HasLink => viewModel.Visualization.HasLink();

        public ListRender(EbMobilePage Page)
        {
            InitializeComponent();
            BindingContext = viewModel = new ListViewModel(Page);
            this.Loader.IsVisible = true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!isRendered)
            {
                await viewModel.InitializeAsync();
                this.ToggleLinks();
                this.UpdatePaginationBar();
            }
            isRendered = true;
            this.Loader.IsVisible = false;
        }

        private void ToggleLinks()
        {
            if (this.HasLink && viewModel.Visualization.ShowNewButton)
            {
                EbMobilePage page = HelperFunctions.GetPage(viewModel.Visualization.LinkRefId);

                if (page != null && page.Container is EbMobileForm)
                    AddLinkData.IsVisible = true;
            }

            this.ToggleDataLength();
        }

        private void ToggleDataLength()
        {
            if (viewModel.DataCount <= 0)
            {
                PagingContainer.IsVisible = false;
                EmptyMessage.IsVisible = true;
            }
            else
            {
                PagingContainer.IsVisible = true;
                EmptyMessage.IsVisible = false;
            }
        }

        private void FilterButton_Clicked(object sender, EventArgs e)
        {
            this.FilterView.Show();
        }

        private async void PagingPrevButton_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Offset <= 0)
                return;
            else
            {
                viewModel.Offset -= viewModel.Visualization.PageLength;
                this.pageCount--;
                await viewModel.RefreshDataAsync();
            }
        }

        private async void PagingNextButton_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Offset + viewModel.Visualization.PageLength >= viewModel.DataCount)
                return;
            else
            {
                viewModel.Offset += viewModel.Visualization.PageLength;
                this.pageCount++;
                await viewModel.RefreshDataAsync();
            }
        }

        private void UpdatePaginationBar()
        {
            try
            {
                int pageLength = viewModel.Visualization.PageLength;
                int totalEntries = viewModel.DataCount;
                int offset = viewModel.Offset + 1;
                int length = pageLength + offset - 1;

                if (totalEntries < pageLength || pageLength + offset > totalEntries)
                    length = totalEntries;

                this.PagingMeta.Text = $"{offset} - {length}/{totalEntries}";
                this.PagingPageCount.Text = $"{pageCount}/{(int)Math.Ceiling((double)totalEntries / pageLength)}";
            }
            catch (Exception ex)
            {
                EbLog.Write(ex.Message);
            }
        }

        public void Refreshed()
        {
            this.UpdatePaginationBar();
            this.ToggleDataLength();
        }

        public void UpdateRenderStatus()
        {
            isRendered = false;
        }
    }
}