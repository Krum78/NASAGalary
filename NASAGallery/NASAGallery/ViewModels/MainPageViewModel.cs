using System.Windows.Input;
using NASAGallery.Views;
using Xamarin.Forms;

namespace NASAGallery.ViewModels
{
    public class MainPageViewModel
    {
        public ICommand GotoApodCommand { get; }
        public ICommand GotoSearchCommand { get; }

        public MainPageViewModel()
        {
            GotoApodCommand = new Command(async () =>
            {
                Application.Current.MainPage.IsBusy = true;
                await App.MainNavigation.PushAsync(new ApodView());
                Application.Current.MainPage.IsBusy = false;
            });

            GotoSearchCommand = new Command(async () =>
            {
                Application.Current.MainPage.IsBusy = true;
                await App.MainNavigation.PushAsync(new SearchLibraryView());
                Application.Current.MainPage.IsBusy = false;
            });
        }
    }
}
