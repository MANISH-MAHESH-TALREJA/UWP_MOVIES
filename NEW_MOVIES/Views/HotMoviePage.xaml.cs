﻿using System;
using GalaSoft.MvvmLight.Command;
using NEW_MOVIES.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using NEW_MOVIES.Models;
using NEW_MOVIES.Services;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;

namespace NEW_MOVIES.Views
{
    public sealed partial class HotMoviePage : Page
    {
        private HotMovieViewModel ViewModel {
            get {
                return DataContext as HotMovieViewModel;
            }
        }

        public HotMoviePage()
        {
            InitializeComponent();
            Loaded += HotMoviePage_Loaded;
        }

        private async void HotMoviePage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadData();
            if (ViewModel.SortedType == HotMovieViewModel.Sort.Date)
            {
                RadioDate.IsChecked = true;
            }
            else if (ViewModel.SortedType == HotMovieViewModel.Sort.Rating)
            {
                RadioRating.IsChecked = true;
            }
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
            string movieId = (e.ClickedItem as MovieItemNow).ID;
            if (TimeAPIService.GetedDetail != null && TimeAPIService.GetedDetail.ContainsKey(movieId))
            {
                data = TimeAPIService.GetedDetail[movieId];
            }
            else
            {
                data = await TimeAPIService.GetMovieDetail(movieId);
            }
            LocationMovieList.PrepareConnectedAnimation("Image", e.ClickedItem as MovieItemNow, "ImageMovie");
            NavigationService.Navigate(typeof(MovieDetailViewModel).FullName, data, new SuppressNavigationTransitionInfo());
        }

        private void RadioButtonRating_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SortedType = HotMovieViewModel.Sort.Rating;
            ViewModel.MovieItems = new ObservableCollection<MovieItemNow>(ViewModel.MovieItems.OrderBy(i => -i.Rating));
        }

        private void RadioButtonDate_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SortedType = HotMovieViewModel.Sort.Date;
            ViewModel.MovieItems = new ObservableCollection<MovieItemNow>(ViewModel.MovieItems.OrderByDescending(i =>
            {
                return DateTime.ParseExact(i.Date, "yyyy-M-d", CultureInfo.InvariantCulture);
            }));
        }
    }
}
