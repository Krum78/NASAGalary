using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NASAGallery.Repository;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace NASAGallery.ViewModels
{
    public class ApodViewModel : ViewModelBase
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
        public string ImageUrl => !IsVideoSource ? Url : string.Empty;

        public bool IsVideoSource => MediaType == Repository.MediaType.Video;
        
        public Command ShowPreviousDayCommand { get; }
        public Command ShowNextDayCommand { get; }
        public Command OpenInBrowserCommand { get; }

        public ApodViewModel()
        {
            ShowPreviousDayCommand = new Command(ShowPreviousDay, () => !IsBusy);
            ShowNextDayCommand = new Command(ShowNextDay, () => !IsBusy && IsNexDayAvailable);
            OpenInBrowserCommand = new Command(() => Device.OpenUri(new Uri(Url)));
            ShowCurrentDay();
        }

        private void ShowCurrentDay()
        {
            GetApodData();
        }

        private void ShowPreviousDay()
        {
            if (DateTime.TryParseExact(Date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime currentDateTime))
            {
                GetApodData(currentDateTime.AddDays(-1).ToString(DateFormat));
            }
        }

        private void ShowNextDay()
        {
            if (DateTime.TryParseExact(Date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime currentDateTime))
            {
                GetApodData(currentDateTime.AddDays(1).ToString(DateFormat));
            }
        }

        private void GetApodData(string forDate = null)
        {
            try
            {
                IsBusy = true;
                ShowPreviousDayCommand.ChangeCanExecute();
                ShowNextDayCommand.ChangeCanExecute();

                Task.Run(async () =>
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
                }).Wait();
            }
            finally
            {
                IsBusy = false;
                ShowPreviousDayCommand.ChangeCanExecute();
                ShowNextDayCommand.ChangeCanExecute();
            }
        }

        
    }
}
