using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageNoiseApp
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private ImageSource _resultImageSource;
        private ImageSource _originalImageSource;

        public ImageSource OriginalImageSource
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
            OriginalImageSource = new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute));
        });

        public Command LoadScreenshotCommand => new(_ => 
        {
            var screenSize = new System.Drawing.Size(Convert.ToInt32(SystemParameters.PrimaryScreenWidth), Convert.ToInt32(SystemParameters.PrimaryScreenHeight));
            var bitmap = new Bitmap(screenSize.Width, screenSize.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, screenSize);
            OriginalImageSource = GetImageSource(bitmap);
        });

        public Command GenerateCommand => new(_ => 
        {
            var resultImage = GetBitmap(OriginalImageSource);
            resultImage.AddNoise(NoiseLevel);
            ResultImageSource = GetImageSource(resultImage);
        });

        public Command SaveResultCommand => new(_ => 
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.png|*.png";
            if (saveFileDialog.ShowDialog() != true) return;
            var file = saveFileDialog.FileName;
            SaveBitmap(ResultImageSource, file);
        });

        private static ImageSource GetImageSource(Bitmap bitmap)
        {
            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Bmp);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private static Bitmap GetBitmap(ImageSource imageSource)
        {
            using var memoryStream = new MemoryStream();
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)imageSource));
            encoder.Save(memoryStream);
            return new Bitmap(memoryStream);
        }

        private static void SaveBitmap(ImageSource imageSource, string file)
        {
            using var fileStream = new FileStream(file, FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)imageSource));
            encoder.Save(fileStream);
        }
    }
}
