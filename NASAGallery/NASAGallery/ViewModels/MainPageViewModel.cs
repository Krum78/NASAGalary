using System.Windows.Input;
using NASAGallery.Repository;
using NASAGallery.Views;
using Xamarin.Forms;

namespace NASAGallery.ViewModels
{
    public class MainPageViewModel
    {
        public ICommand TestApiCommand { get; }
        public ICommand GotoApodCommand { get; }

        public MainPageViewModel()
        {
            TestApiCommand = new Command(async () =>
            {
                var model = await ApiClient.SearchAsync("hubble");
            });

            GotoApodCommand = new Command(async () =>
            {
                Application.Current.MainPage.IsBusy = true;
                await App.MainNavigation.PushAsync(new ApodView());
                Application.Current.MainPage.IsBusy = false;
            });
        }
    }
}
