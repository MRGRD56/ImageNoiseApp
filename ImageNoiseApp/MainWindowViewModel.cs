using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageNoiseApp
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private ImageSource _resultImageSource;
        private string _originalImageSource;

        public string OriginalImageSource
        {
            get => _originalImageSource;
            set
            {
                _originalImageSource = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ResultImageSource
        {
            get => _resultImageSource;
            set
            {
                _resultImageSource = value;
                OnPropertyChanged();
            }
        }

        public int NoiseLevel { get; set; } = 100;

        public MainWindowViewModel()
        {

        }

        public Command LoadFileCommand => new(_ =>
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*";
            var result = dialog.ShowDialog();
            if (result != true) return;
            OriginalImageSource = dialog.FileName;
        });

        public Command LoadScreenshotCommand => new(_ => { });

        public Command GenerateCommand => new(_ => { });

        public Command SaveResultCommand => new(_ => { });
    }
}
