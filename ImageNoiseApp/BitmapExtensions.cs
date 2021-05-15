using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageNoiseApp
{
    public static class BitmapExtensions
    {
        public static unsafe void AddNoise(this Bitmap image, int level)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            var bits = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            var random = new Random();
            for (var y = 0; y < image.Height; y++)
            {
                var positionPtr = (byte*) bits.Scan0 + y * bits.Stride;
                for (var x = 0; x < image.Width; x++)
                {
                    var rPtr = positionPtr + 2;
                    var gPtr = positionPtr + 1;
                    var bPtr = positionPtr + 0;

                    void DeformColorPart(byte* partPtr, int noiseLevel)
                    {
                        var part = Convert.ToInt32(*partPtr);
                        var minNoise = part < noiseLevel ? part : noiseLevel;//new List<int> { part, noiseLevel }.Min();
                        var maxNoise = (255 - part) < noiseLevel ? 255 - part : noiseLevel;//new List<int> { 255 - part, noiseLevel }.Min();
                        part += random.Next(-minNoise, maxNoise);
                        *partPtr = Convert.ToByte(part);
                    }

                    void AddNoiseToColor(int noiseLevel)
                    {
                        if (noiseLevel is < 0 or > 255)
                        {
                            throw new ArgumentException("Value must be [0; 255]", nameof(noiseLevel));
                        }
                        DeformColorPart(rPtr, noiseLevel);
                        DeformColorPart(gPtr, noiseLevel);
                        DeformColorPart(bPtr, noiseLevel);
                    }

                    AddNoiseToColor(level);

                    positionPtr += 3;
                }
            }
            
            image.UnlockBits(bits);
            
            stopwatch.Stop();
            Debug.WriteLine($"AddNoise: {stopwatch.ElapsedMilliseconds / 1000D} secs");
        }
    }
}
