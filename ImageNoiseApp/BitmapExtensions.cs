using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageNoiseApp
{
    public static class BitmapExtensions
    {
        public static void AddNoise(this Bitmap image, int level)
        {
            var random = new Random();
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var pixel = image.GetPixel(x, y);
                    int r = pixel.R;
                    int g = pixel.G;
                    int b = pixel.B;

                    void DeformColorPart(ref int part, int noiseLevel)
                    {
                        var maxDownNoice = new List<int> { part, noiseLevel }.Min();
                        var maxUpNoice = new List<int> { 255 - part, noiseLevel }.Min();
                        part += random.Next(-maxDownNoice, maxUpNoice);
                    }

                    void AddNoiseToColor(int noiseLevel)
                    {
                        if (noiseLevel is < 0 or > 255)
                        {
                            throw new ArgumentException("Value must be [0; 255]", nameof(noiseLevel));
                        }
                        DeformColorPart(ref r, noiseLevel);
                        DeformColorPart(ref g, noiseLevel);
                        DeformColorPart(ref b, noiseLevel);
                    }

                    AddNoiseToColor(level);

                    image.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
        }
    }
}
