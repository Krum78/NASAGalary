using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace NASAGallery.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isBusy;
        private bool _isDataAvailable;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool IsDataAvailable
        {
            get => _isDataAvailable;
            set
            {
                if(_isDataAvailable == value) return;
                _isDataAvailable = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (Device.IsInvokeRequired)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
