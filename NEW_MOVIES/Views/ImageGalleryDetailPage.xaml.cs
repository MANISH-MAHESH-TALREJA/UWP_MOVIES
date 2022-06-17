using System;

using NEW_MOVIES.Models;
using NEW_MOVIES.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NEW_MOVIES.Views
{
    public sealed partial class ImageGalleryDetailPage : Page
    {
        private ImageGalleryDetailViewModel ViewModel
        {
            get { return DataContext as ImageGalleryDetailViewModel; }
        }

        public ImageGalleryDetailPage()
        {
            InitializeComponent();
            ViewModel.SetImage(previewImage);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.InitializeAsync(e.Parameter as PostItem, e.NavigationMode);
            showFlipView.Begin();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                previewImage.Visibility = Visibility.Visible;
                // ViewModel.SetAnimation();
            }
        }
    }
}
