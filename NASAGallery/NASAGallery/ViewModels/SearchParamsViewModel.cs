using System;
using System.Collections.Generic;
using System.Text;
using NASAGallery.Common;
using NASAGallery.Repository;
using Xamarin.Forms;

namespace NASAGallery.ViewModels
{
    public class SearchParamsViewModel : ViewModelBase
    {
        private string _searchQuery;
        private string _titleQuery;

        private MediaType _includedTypes = MediaType.Image | MediaType.Video;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if(value == _searchQuery) return;
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        public string TitleQuery
        {
            get => _titleQuery;
            set
            {
                if (_titleQuery == value) return;
                _titleQuery = value;
                OnPropertyChanged();
            }
        }

        public bool IncludeImages
        {
            get => _includedTypes.HasFlag(MediaType.Image);
            set
            {
                if(value == _includedTypes.HasFlag(MediaType.Image)) return;

                if (value)
                {
                    _includedTypes |= MediaType.Image;
                }
                else
                {
                    _includedTypes &= ~MediaType.Image;
                }
            }
        }

        public bool IncludeVideo
        {
            get => _includedTypes.HasFlag(MediaType.Video);
            set
            {
                if (value == _includedTypes.HasFlag(MediaType.Video)) return;

                if (value)
                {
                    _includedTypes |= MediaType.Video;
                }
                else
                {
                    _includedTypes &= ~MediaType.Video;
                }
            }
        }

        public bool IncludeAudio
        {
            get => _includedTypes.HasFlag(MediaType.Audio);
            set
            {
                if (value == _includedTypes.HasFlag(MediaType.Audio)) return;

                if (value)
                {
                    _includedTypes |= MediaType.Audio;
                }
                else
                {
                    _includedTypes &= ~MediaType.Audio;
                }
            }
        }

        public Command SearchCommand { get; set; }

        public SearchParamsViewModel()
        {
            SearchCommand = new Command(() => {});
        }
    }
}
