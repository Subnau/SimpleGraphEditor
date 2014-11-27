using System.IO;
using BitmapLib;
using NUnit.Framework;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace BtimapLib.UnitTests
{
    [TestFixture]
    public class OperationsTests
    {
        private bool bitmapsCompare(Bitmap bm1, Bitmap bm2)
        {
            for (int x = 0; x < bm1.Width; x++)
            {
                for (int y = 0; y < bm1.Height; y++)
                {
                    Color c1 = bm1.GetPixel(x, y);
                    Color c2 = bm2.GetPixel(x, y);
                    if (c1.R != c2.R || c1.G != c2.G || c1.B != c2.B)
                        return false;
                }
            }
            return true;
        }

        [Test]
        public void OpearationInvert_DoubleInvertion_ArraysMustBeEqual()
        {
          
            var bmIn = new Bitmap(10, 10);

            var sIn = new MemoryStream();
            bmIn.Save(sIn, ImageFormat.Bmp);
            //arrIn = sIn.ToArray();
            MemoryStream sOut1 = BitmapOperation.Operation(sIn, BitmapOperations.Invert, null, CancellationToken.None);
            sIn.Close();
            //arrOut1 = sOut.ToArray();
            //sOut.Close();
            //sIn = new MemoryStream(arrOut1);
            sOut1.Position = 0;
            MemoryStream sOut2 = BitmapOperation.Operation(sOut1, BitmapOperations.Invert, null, CancellationToken.None);
            //arrOut2 = sOut.ToArray();
            Bitmap bmOut = new Bitmap(sOut2);
            sOut1.Close();
            sOut2.Close();

            Assert.IsTrue(bitmapsCompare(bmIn, bmOut));

        }
    }
}
