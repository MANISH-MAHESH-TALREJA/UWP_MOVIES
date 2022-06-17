﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NEW_MOVIES.Helpers;
using NEW_MOVIES.Models;
using NEW_MOVIES.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace NEW_MOVIES.ViewModels
{
    public class MovieDetailViewModel : ViewModelBase
    {
        public MovieDetailViewModel()
        {
            MovieDetail = new MovieItemDetail();
            Source = new ObservableCollection<PostItem>();
        }
        public MovieItemDetail movieDetail;
        public MovieItemDetail MovieDetail {
            get => movieDetail;
            set => Set(ref movieDetail, value);
        }

        public Visibility GetVisibility {
            get {
                return MovieDetail.ID == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public void AddData()
        {
            MovieDetail = TimeAPIService.CurrentDetail;
            Source = new ObservableCollection<PostItem>(movieDetail.Images);
            RaisePropertyChanged("GetVisibility");
        }

        public void SaveData(ref OnBackgroundEnteringEventArgs e)
        {
            e.SuspensionState.Data = TimeAPIService.CurrentDetail;
            e.Target = this.GetType();
        }

        public const string ImageGallerySelectedIdKey = "ImageGallerySelectedIdKey";
        public const string ImageGalleryAnimationOpen = "ImageGallery_AnimationOpen";
        public const string ImageGalleryAnimationClose = "ImageGallery_AnimationClose";

        private ObservableCollection<PostItem> _source;
        private GridView _imagesGridView;

        private ICommand _itemSelectedCommand;
        private ICommand _videoSelectedCommand;
        private ICommand _openUriCommand;
        private ICommand _shareMovieCommand;

        public ObservableCollection<PostItem> Source {
            get => _source;
            set => Set(ref _source, value);
        }

        public ICommand ItemSelectedCommand => _itemSelectedCommand ?? (_itemSelectedCommand = new RelayCommand<ItemClickEventArgs>(OnsItemSelected));

        public ICommand VideoSelectedCommand => _videoSelectedCommand ?? (_videoSelectedCommand = new RelayCommand<ItemClickEventArgs>(OnVideoSelected));

        public ICommand OpenUri => _openUriCommand ?? (_openUriCommand = new RelayCommand(OpenTheUri));

        public ICommand ShareMovieCommand => _shareMovieCommand ?? (_shareMovieCommand = new RelayCommand(ShareMovie));
        



        public void Initialize(GridView imagesGridView)
        {
            _imagesGridView = imagesGridView;
        }

        public async Task LoadAnimationAsync()
        {
            var selectedImageId = await ApplicationData.Current.LocalSettings.ReadAsync<string>(ImageGallerySelectedIdKey);
            if (!string.IsNullOrEmpty(selectedImageId))
            {
                var item = _imagesGridView.Items.FirstOrDefault(i => ((PostItem)i).ID == selectedImageId);
                if (item != null) _imagesGridView.ScrollIntoView(item);
                ApplicationData.Current.LocalSettings.SaveString(ImageGallerySelectedIdKey, string.Empty);
            }
        }

        private NavigationServiceEx NavigationService {
            get {
                return CommonServiceLocator.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            }
        }

        private void OnsItemSelected(ItemClickEventArgs args)
        {
            var selected = args.ClickedItem as PostItem;
            _imagesGridView.PrepareConnectedAnimation(ImageGalleryAnimationOpen, selected, "galleryImage");
            NavigationService.Navigate(typeof(ImageGalleryDetailViewModel).FullName, args.ClickedItem);
        }

        private void OnVideoSelected(ItemClickEventArgs args)
        {
            var selected = args.ClickedItem as VideoItem;
            NavigationService.Navigate(typeof(MediaPlayerViewModel).FullName, args.ClickedItem);

        }

        private async void OpenTheUri()
        {
            await Launcher.LaunchUriAsync(new Uri(movieDetail.Url));
        }

        private void ShareMovie()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

            MovieItemDetail movie = TimeAPIService.CurrentDetail;
            string title = ResourceLoader.GetForCurrentView().GetString("MovieDetailViewModel_Title/Text");
            string description = ResourceLoader.GetForCurrentView().GetString("MovieDetailViewModel_Description/Text");
            string reqText = ResourceLoader.GetForCurrentView().GetString("MovieDetailViewModel_Request/Text");
            dataTransferManager.DataRequested += (s, args) => {
                DataRequest request = args.Request;
                request.Data.Properties.Title = string.Format(title, movie.TitleCn);
                request.Data.Properties.Description = description;
                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromUri(new Uri(movie.Image));
                request.Data.Properties.Thumbnail = imageStreamRef;
                request.Data.SetBitmap(imageStreamRef);
                request.Data.SetText(string.Format(reqText, movie.Story));
                request.Data.SetWebLink(new Uri(movie.Url));
            };
            DataTransferManager.ShowShareUI();
        }

        public void AddFavorite(string note, MovieItemDetail data)
        {
            data.Note = note;
            int flag = 0;
            // Test
            //Singleton<MyCollectService>.Instance.Collections.Add(data);
            //Singleton<MyCollectService>.Instance.SaveToStorage();

            foreach(MovieItem movie in Singleton<MyCollectService>.Instance.Collections)
            {
                if(movie.ID == data.ID)
                {
                    movie.Note = note;
                    flag = 1;
                    break;
                }
            }
            if(flag == 0)
            {
                Singleton<MyCollectService>.Instance.Collections.Add(data);
                Singleton<MyCollectService>.Instance.SaveToStorage();
            }
        }

        public void DeleteFavorite(MovieItemDetail data)
        {
            foreach (MovieItem movie in Singleton<MyCollectService>.Instance.Collections)
            {
                if (movie.ID == data.ID)
                {
                    Singleton<MyCollectService>.Instance.Collections.Remove(movie);
                    break;
                }
            }
        }
    }
}
