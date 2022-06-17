﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NEW_MOVIES.Models;
using NEW_MOVIES.Services;
using NEW_MOVIES.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace NEW_MOVIES.Views
{
    public sealed partial class ComingMoviePage : Page
    {
        private ComingMovieViewModel ViewModel {
            get {
                return DataContext as ComingMovieViewModel;
            }
        }

        public ComingMoviePage()
        {
            InitializeComponent();
            Loaded += ComingMoviePage_Loaded;
        }

        private async void ComingMoviePage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadData();
        }


        public NavigationServiceEx NavigationService {
            get {
                return CommonServiceLocator.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LocationMovieList.IsEnabled = true;
            LoadToOther.Visibility = Visibility.Collapsed;
        }

        private async void LocationMovieList_ItemClick(object sender, ItemClickEventArgs e)
        {
            LocationMovieList.IsEnabled = false;
            LoadToOther.Visibility = Visibility.Visible;
            MovieItemDetail data;
            string movieId = (e.ClickedItem as MovieItemComing).ID;
            if (TimeAPIService.GetedDetail != null && TimeAPIService.GetedDetail.ContainsKey(movieId))
            {
                data = TimeAPIService.GetedDetail[movieId];
            }
            else
            {
                data = await TimeAPIService.GetMovieDetail(movieId);
            }
            LocationMovieList.PrepareConnectedAnimation("Image", e.ClickedItem as MovieItemComing, "ImageMovie");
            NavigationService.Navigate(typeof(MovieDetailViewModel).FullName, data, new SuppressNavigationTransitionInfo());
        }
    }
}
