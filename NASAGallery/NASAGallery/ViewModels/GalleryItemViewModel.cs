using System;
using System.Globalization;
using System.Threading.Tasks;
using NASAGallery.Repository;
using Xamarin.Forms;

namespace NASAGallery.ViewModels
{
    public class GalleryItemViewModel : ViewModelBase
    {
        private const string DateFormat = "yyyy-MM-dd";
        private string _copyright;
        private string _date;
        private string _title;
        private string _explanation;
        private MediaType? _mediaType;
        private string _hdUrl;
        private string _url;

        public string Copyright
        {
            get => string.IsNullOrWhiteSpace(_copyright) ? "Public Domain" : _copyright;
            set
            {
                if (_copyright == value) return;
                _copyright = value;
                OnPropertyChanged();
            }
        }

        public string Date
        {
            get => _date;
            set
            {
                if(_date == value) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public bool IsNexDayAvailable
        {
            get
            {
                if (DateTime.TryParseExact(Date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime currentDateTime))
                {
                    return !currentDateTime.Date.Equals(DateTime.Now.Date);
                }

                return false;
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if(_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Explanation
        {
            get => _explanation;
            set
            {
                if(_explanation == value) return;
                _explanation = value;
                OnPropertyChanged();
            }
        }

        public MediaType? MediaType
        {
            get => _mediaType;
            set
            {
                if(_mediaType == value) return;
                _mediaType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsVideoSource));
            }
        }

        public string HdUrl
        {
            get => _hdUrl;
            set
            {
                if (_hdUrl == value) return;
                _hdUrl = value;
                OnPropertyChanged();
            }
        }

        public string Url
        {
            get => _url;
            set
            {
                if(_url == value) return;
                _url = value;
                OnPropertyChanged();
            }
        }

        public string VideoUrl => IsVideoSource ? Url : string.Empty;
        public string ImageUrl => !IsVideoSource ? Url : ThumbUrl;

        public string ThumbUrl { get; set; }

        public bool IsVideoSource => MediaType == Repository.MediaType.Video;
        
        public Command ShowPreviousDayCommand { get; }
        public Command ShowNextDayCommand { get; }
        public Command OpenInBrowserCommand { get; }

        public GalleryItemViewModel():this(true)
        {
        }

        public GalleryItemViewModel(bool loadCurrentDay)
        {
            ShowPreviousDayCommand = new Command(async () => await ShowPreviousDay(), () => !IsBusy);
            ShowNextDayCommand = new Command(async () => await ShowNextDay(), () => !IsBusy && IsNexDayAvailable);
            OpenInBrowserCommand = new Command(() => Device.OpenUri(new Uri(Url)));
            if (loadCurrentDay)
                ShowCurrentDay();
        }

        private void ShowCurrentDay()
        {
            Task.Run(async () => await GetApodData());
        }

        private async Task ShowPreviousDay()
        {
            if (DateTime.TryParseExact(Date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime currentDateTime))
            {
                await GetApodData(currentDateTime.AddDays(-1).ToString(DateFormat));
            }
        }

        private async Task ShowNextDay()
        {
            if (DateTime.TryParseExact(Date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime currentDateTime))
            {
                await GetApodData(currentDateTime.AddDays(1).ToString(DateFormat));
            }
        }

        private async Task GetApodData(string forDate = null)
        {
            UpdateBusyState(true);

            await Task.Run(async () =>
            {
                try
                {
                    var apodModel = await ApiClient.RequestApodAsync(forDate);

                    Title = apodModel?.Title;
                    MediaType = apodModel?.MediaType;
                    Url = apodModel?.Url;
                    HdUrl = apodModel?.HdUrl;
                    Copyright = apodModel?.Copyright;
                    Explanation = apodModel?.Explanation;
                    Date = apodModel?.Date ?? forDate;

                    IsDataAvailable = apodModel != null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    UpdateBusyState(false);
                }
            });
        }

        private void UpdateBusyState(bool isBusy)
        {
            if (Device.IsInvokeRequired)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = isBusy;
                    ShowPreviousDayCommand.ChangeCanExecute();
                    ShowNextDayCommand.ChangeCanExecute();
                });
                return;
            }

            IsBusy = isBusy;
            ShowPreviousDayCommand.ChangeCanExecute();
            ShowNextDayCommand.ChangeCanExecute();
        }
    }
}
