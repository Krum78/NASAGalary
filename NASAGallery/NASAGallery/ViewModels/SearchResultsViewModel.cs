using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NASAGallery.Repository;
using NASAGallery.Views;
using Xamarin.Forms;

namespace NASAGallery.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private ObservableCollection<SearchResultItemViewModel> _items;
        private SearchResultItemViewModel _selectedItem;
        public SearchResultModel SearchResult { get; set; }

        public ObservableCollection<SearchResultItemViewModel> Items
        {
            get => _items;
            set
            {
                if(value == _items) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public SearchResultItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public string NextPageUrl => SearchResult?.Collection?.Links?.FirstOrDefault(l => l.Rel == "next")?.Href;
        public string PrevPageUrl => SearchResult?.Collection?.Links?.FirstOrDefault(l => l.Rel == "prev")?.Href;

        public Command NextPageCommand { get; set; }

        public Command PrevPageCommand { get; set; }

        public Command ShowItemCommand { get; set; }
        public Command SelectItemCommand { get; set; }


        public SearchResultsViewModel()
        {
            NextPageCommand = new Command(async () => await LoadNextPage(), () => !string.IsNullOrWhiteSpace(NextPageUrl));
            PrevPageCommand = new Command(async () => await LoadPrevPage(), () => !string.IsNullOrWhiteSpace(PrevPageUrl));
            ShowItemCommand = new Command<SearchResultItemViewModel>(async (item) => await OpenSelectedItem(item));
            SelectItemCommand = new Command<SearchResultItemViewModel>(SelectItem);
            Items = new ObservableCollection<SearchResultItemViewModel>();
        }

        public SearchResultsViewModel(SearchResultModel searchResult) : this()
        {
            SearchResult = searchResult;
            RefreshList();
        }

        private void RefreshList()
        {
            UpdateBusyState(true);
            Items = new ObservableCollection<SearchResultItemViewModel>(SearchResult.Collection.Items
                .Select(i => new SearchResultItemViewModel(i)).ToList());
            UpdateBusyState(false);
        }

        private async Task LoadNextPage()
        {
            await LoadPage(NextPageUrl);
        }

        private async Task LoadPrevPage()
        {
            await LoadPage(PrevPageUrl);
        }

        private async Task LoadPage(string url)
        {
            if(string.IsNullOrWhiteSpace(url))
                return;

            try
            {
                UpdateBusyState(true);

                await Task.Run(async () =>
                {
                    try
                    {
                        SearchResult = await ApiClient.RequestModelAsync<SearchResultModel>(url);
                        Items = new ObservableCollection<SearchResultItemViewModel>(SearchResult.Collection.Items
                            .Select(i => new SearchResultItemViewModel(i)).ToList());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
            }
            finally
            {
                UpdateBusyState(false);
            }
        }

        private void SelectItem(SearchResultItemViewModel item)
        {
            SelectedItem = item;
        }

        private async Task OpenSelectedItem(SearchResultItemViewModel item)
        {
            if (item == null)
                return;

            try
            {
                UpdateBusyState(true);

                GalleryItemViewModel viewModel = null;

                await Task.Run(async () =>
                {
                    try
                    {
                        var collection = await ApiClient.RequestModelAsync<List<string>>(item.ItemModel.Href);

                        if (collection == null || collection.Count == 0)
                        {
                            var assetUrl =
                                $"https://images-api.nasa.gov/asset/{item.ItemModel.Data[0].NasaId}";
                            var asset = await ApiClient.RequestModelAsync<AssetModel>(assetUrl);

                            var assetItems = asset?.Collection?.Items;

                            if (assetItems == null || assetItems.Count == 0)
                                return;

                            collection = asset.Collection.Items.Select(i => i.Href).ToList();
                        }

                        var data = item.ItemModel.Data[0];
                        
                        if (data == null)
                            return;

                        string url = ApiClient.GetAssetUrlByPriority(collection, "~medium.", "~mobile.", "~small.", "~orig.");
                        string hdUrl = ApiClient.GetAssetUrlByPriority(collection, "~orig.", "~medium.", "~mobile.", "~small.");
                        string thumbUrl = ApiClient.GetAssetUrlByPriority(collection, "~medium_thumb_", "~mobile_thumb_", "~small_thumb_", "~preview_thumb_", "~large_thumb_");

                        viewModel = new GalleryItemViewModel (false)
                        {
                            MediaType = data.MediaType,
                            Date = data.DateCreated.ToString(),
                            Explanation = data.Description,
                            Copyright = data.Center,
                            Title = data.Title,
                            Url = url,
                            HdUrl = hdUrl,
                            ThumbUrl = thumbUrl,
                            IsDataAvailable = true
                        };
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });

                if(viewModel != null)
                    await App.MainNavigation.PushAsync(new GalleryItemView { BindingContext = viewModel });
            }
            finally
            {
                UpdateBusyState(false);
            }
        }

        private void UpdateBusyState(bool isBusy)
        {
            if (Device.IsInvokeRequired)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = isBusy;
                    PrevPageCommand.ChangeCanExecute();
                    NextPageCommand.ChangeCanExecute();
                });
                return;
            }

            IsBusy = isBusy;

            PrevPageCommand.ChangeCanExecute();
            NextPageCommand.ChangeCanExecute();
        }
    }
}
