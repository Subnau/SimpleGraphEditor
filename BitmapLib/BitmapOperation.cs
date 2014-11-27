using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace BitmapLib
{

    public enum BitmapOperations
    {
        Invert,
        Flip
    }

    public static class BitmapOperation
    {
        /// <summary>
        /// Operation on bitmap
        /// </summary>
        /// <param name="sIn">Stream with bitmap</param>
        /// <param name="bitmapOperation">Operation type (invert)</param>
        /// <param name="onProgressAction">Action for view percentage and text </param>
        /// <param name="token">Cancellation Token for async execute</param>
        /// <returns></returns>
        public static MemoryStream Operation(Stream sIn, BitmapOperations bitmapOperation,
            Action<int, string> onProgressAction, CancellationToken token)
        {
            return invert(sIn, onProgressAction, token);
        }

        //this operation was created just to show background proccessing 
        private static MemoryStream invert(Stream sIn, Action<int, string> onProgressAction, CancellationToken token)
        {
            var bm = new Bitmap(sIn);
            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    Color c = bm.GetPixel(x, y);
                    bm.SetPixel(x, y, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                    //Thread.Sleep(5);
                    if (token != CancellationToken.None)
                        if (token.IsCancellationRequested)
                        {
                            if (onProgressAction != null)
                                onProgressAction(0, "Canceled");
                            return null;
                        }
                }
                if (onProgressAction != null)
                {
                    var perc = (int)(x * 100.0 / bm.Width);
                    onProgressAction(perc, perc + "%");
                }
            }
            var sOut = new MemoryStream();
            bm.Save(sOut, ImageFormat.Bmp);
            if (onProgressAction != null)
                onProgressAction(100, "Complete");
            return sOut;
        }

    }
}
