using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using BitmapLib;
using GalaSoft.MvvmLight.Threading;

namespace SimpleGraphEditor.ViewModel
{

    /// <summary>
    /// Commands 
    /// </summary>
    public partial class MainViewModel
    {

        private RelayCommand _openFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand(() =>
                {
                    //todo: dialog take out to service
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == true)
                        if (File.Exists(ofd.FileName))
                        {
                            try
                            {
                                BitmapSource bs = new BitmapImage(new Uri(ofd.FileName));
                                var rtb = new RenderTargetBitmap(bs.PixelWidth, bs.PixelHeight,
                                    bs.DpiX, bs.DpiY, PixelFormats.Pbgra32);
                                rtb.Render(createDrawingVisualFromImage(bs));
                                BitmapImage = rtb;
                                if (SelectedPrimitive != null)
                                    SelectedPrimitive.Reset();
                            }
                            catch (NotSupportedException)
                            {
                                MessageBox.Show("Image format not supported");
                            }
                        }

                }));
            }
        }

        private RelayCommand _saveFileCommand;
        public RelayCommand SaveFileCommand
        {
            get
            {
                return _saveFileCommand ?? (_saveFileCommand = new RelayCommand(() =>
                {
                    if (BitmapImage != null)
                    {
                        //todo: dialog take out to service
                        var sfd = new SaveFileDialog { DefaultExt = "bmp" };
                        if (sfd.ShowDialog() == true)
                        {
                            using (var stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                var encoder = new BmpBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(BitmapImage));
                                encoder.Save(stream);
                            }
                        }
                    }
                }));
            }
        }


        private RelayCommand _newImageCommand;
        public RelayCommand NewImageCommand
        {
            get
            {
                return _newImageCommand ?? (_newImageCommand = new RelayCommand(() =>
                    {
                        BitmapImage = new RenderTargetBitmap(1000, 500, 100, 100, PixelFormats.Pbgra32);
                        var drawingVisual = new DrawingVisual();
                        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                        {
                            drawingContext.DrawRectangle(Brushes.White, null,
                                new Rect(0, 0, BitmapImage.Width, BitmapImage.Height));
                        }
                        BitmapImage.Render(drawingVisual);
                    }));
            }
        }

        private RelayCommand _longInvertCommand;
        public RelayCommand LongInvertCommand
        {
            get
            {
                return _longInvertCommand ?? (_longInvertCommand = new RelayCommand(longInvert));
            }
        }

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(() =>
                {
                    if (_cancelWork != null) _cancelWork(_tsk);
                }));
            }
        }

        private Point getPointFromMouseArgs(MouseEventArgs e)
        {
            return e.GetPosition((IInputElement)e.Source);
        }

        private RelayCommand<MouseButtonEventArgs> _mouseDownCommand;
        public RelayCommand<MouseButtonEventArgs> MouseDownCommand
        {
            get
            {
                return _mouseDownCommand ?? (_mouseDownCommand = new RelayCommand<MouseButtonEventArgs>(
                    e => SelectedPrimitive.MouseDown(e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, getPointFromMouseArgs(e))
                    , e => IsNotTaskRunning));
            }
        }

        private RelayCommand<MouseButtonEventArgs> _mouseUpCommand;
        public RelayCommand<MouseButtonEventArgs> MouseUpCommand
        {
            get
            {
                return _mouseUpCommand ?? (_mouseUpCommand = new RelayCommand<MouseButtonEventArgs>(
                    e => SelectedPrimitive.MouseUp(e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, getPointFromMouseArgs(e))
                    , e => IsNotTaskRunning));
            }
        }

        private RelayCommand<MouseEventArgs> _mouseMoveCommand;
        public RelayCommand<MouseEventArgs> MouseMoveCommand
        {
            get
            {
                return _mouseMoveCommand ?? (_mouseMoveCommand = new RelayCommand<MouseEventArgs>(
                    e => SelectedPrimitive.MouseMove(e.LeftButton == MouseButtonState.Pressed, e.RightButton == MouseButtonState.Pressed, getPointFromMouseArgs(e))
                    , e => IsNotTaskRunning));
            }
        }

        private void progressChange(int perc, string text)
        {
            //change text in UI thread
            DispatcherHelper.CheckBeginInvokeOnUI(() => ProgressValueInPercentages = perc);
            DispatcherHelper.CheckBeginInvokeOnUI(() => TaskTimeText = text);
        }

        /// <summary>
        /// async invert operation
        /// </summary>
        private void longInvert()
        {
            if (BitmapImage == null)
                return;
            if (SelectedPrimitive != null)
                SelectedPrimitive.Reset();
            TaskTimeText = "";
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            //working with BitmapImage before task
            var encoder = new BmpBitmapEncoder();
            var stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(BitmapImage));
            encoder.Save(stream);

            IsTaskRunning = true; //disable UI controls

            _tsk = Task.Factory.StartNew(() =>
            {
                using (stream)
                {
                    MemoryStream s = BitmapOperation.Operation(stream, BitmapOperations.Invert, progressChange, token);
                    if (s != null)
                    {
                        byte[] arr = s.ToArray(); //arr for UI thread
                        s.Close();
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var bi = new BitmapImage(); //new BitmapImage must be created in UI thread
                            bi.BeginInit();
                            bi.StreamSource = new MemoryStream(arr);
                            bi.EndInit();
                            var rtb = new RenderTargetBitmap(bi.PixelWidth, bi.PixelHeight,
                                bi.DpiX, bi.DpiY, PixelFormats.Pbgra32);
                            rtb.Render(createDrawingVisualFromImage(bi));
                            BitmapImage = rtb;
                        });
                    }
                }
                IsTaskRunning = false; //enable UI contorls
            }, token);

            //Cancel action, will run when CancelCommand occured
            _cancelWork = t =>
            {
                cts.Cancel();
                t.Wait();
                IsTaskRunning = false;
            };
        }

    }
}