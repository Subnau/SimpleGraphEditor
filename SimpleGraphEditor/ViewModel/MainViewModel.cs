using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GraphPrimitives;

namespace SimpleGraphEditor.ViewModel
{

    public partial class MainViewModel : ViewModelBase
    {
        //action when cancel background task
        private Action<Task> _cancelWork;
        //background task
        private Task _tsk;


        #region Poperties for binding with view

        private readonly List<GraphPrimitive> _primitives = new List<GraphPrimitive>();
        /// <summary>
        /// Supported primitives
        /// </summary>
        public List<GraphPrimitive> Primitives
        {
            get { return _primitives; }
        }

        private GraphPrimitive _selectedPrimitive;
        /// <summary>
        /// Current primitive
        /// </summary>
        public GraphPrimitive SelectedPrimitive
        {
            get { return _selectedPrimitive; }
            set
            {
                if (_selectedPrimitive != value)
                {
                    if (_selectedPrimitive != null)
                        _selectedPrimitive.Reset();
                    _selectedPrimitive = value;
                }
            }
        }

        private RenderTargetBitmap _bitmapImage;
        private const string BitmapImageName = "BitmapImage";
        /// <summary>
        /// Image
        /// </summary>
        public RenderTargetBitmap BitmapImage
        {
            get { return _bitmapImage; }
            private set
            {
                _bitmapImage = value;
                RaisePropertyChanged(BitmapImageName);
            }
        }


        private readonly List<Brush> _availiableColors = new List<Brush>();
        /// <summary>
        /// Supported colors
        /// </summary>
        public List<Brush> AvailiableColors
        {
            get
            {
                return _availiableColors;
            }
        }

        private Brush _foreColor = new SolidColorBrush(Colors.Gold);
        private const string ForeColorName = "ForeColor";
        /// <summary>
        /// Color for border of primitive
        /// </summary>
        public Brush ForeColor
        {
            get { return _foreColor; }
            set
            {
                _foreColor = value;
                RaisePropertyChanged(ForeColorName);
            }
        }

        private Brush _backColor = new SolidColorBrush(Colors.Brown);
        private const string BackColorName = "BackColor";
        /// <summary>
        /// Background color for primitives rectangle, ellipse etc
        /// </summary>
        public Brush BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                RaisePropertyChanged(BackColorName);
            }
        }

        private PointCollection _points;
        private const string PointsName = "Points";
        /// <summary>
        ///  Binding collection of points for multipoints primitive (polyline)
        /// </summary>
        public PointCollection Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                RaisePropertyChanged(PointsName);
            }
        }


        private double _lineX1;
        private const string LineX1Name = "LineX1";
        /// <summary>
        /// Binding X1 for 2-points (rectangle, line) primitives
        /// </summary>
        public double LineX1
        {
            get { return _lineX1; }
            set
            {
                _lineX1 = value;
                RaisePropertyChanged(LineX1Name);
            }
        }

        private double _lineY1;
        private const string LineY1Name = "LineY1";
        /// Binding Y1 for 2-points (rectangle, line) primitives
        public double LineY1
        {
            get { return _lineY1; }
            set
            {
                _lineY1 = value;
                RaisePropertyChanged(LineY1Name);
            }
        }

        private double _lineX2;
        private const string LineX2Name = "LineX2";
        /// Binding X2 for 2-points (rectangle, line) primitives
        public double LineX2
        {
            get { return _lineX2; }
            set
            {
                _lineX2 = value;
                RaisePropertyChanged(LineX2Name);
            }
        }

        private double _lineY2;
        private const string LineY2Name = "LineY2";
        /// Binding Y2 for 2-points (rectangle, line) primitives
        public double LineY2
        {
            get { return _lineY2; }
            set
            {
                _lineY2 = value;
                RaisePropertyChanged(LineY2Name);
            }
        }

        /// <summary>
        /// Calculated radius 
        /// </summary>
        public double PrimitiveRaduis
        {
            get
            {
                return Math.Sqrt(Math.Pow(Math.Abs(LineX2 - LineX1), 2) + Math.Pow(Math.Abs(LineY2 - LineY1), 2));
            }
        }

        private const string PrimitiveTopName = "PrimitiveTop";
        public double PrimitiveTop
        {
            get
            {
                return LineY1 - PrimitiveRaduis;
            }
        }

        private const string PrimitiveLeftName = "PrimitiveLeft";
        public double PrimitiveLeft
        {
            get
            {
                return LineX1 - PrimitiveRaduis;
            }
        }

        private const string PrimitiveWidthName = "PrimitiveWidth";
        public double PrimitiveWidth
        {
            get
            {
                return PrimitiveRaduis * 2.0;
            }
        }

        private const string PrimitiveHeightName = "PrimitiveHeight";
        public double PrimitiveHeight
        {
            get
            {
                return PrimitiveRaduis * 2.0;
            }
        }

        private Visibility _lineVisibility = Visibility.Hidden;
        private const string LineVisibilityName = "LineVisibility";
        public Visibility LineVisibility
        {
            get { return _lineVisibility; }
            set
            {
                _lineVisibility = value;
                RaisePropertyChanged(LineVisibilityName);
            }
        }

        private Visibility _rectangleVisibility = Visibility.Hidden;
        private const string RectangleVisibilityName = "RectangleVisibility";
        public Visibility RectangleVisibility
        {
            get { return _rectangleVisibility; }
            set
            {
                _rectangleVisibility = value;
                RaisePropertyChanged(RectangleVisibilityName);
            }
        }

        private Visibility _ellipseVisibility = Visibility.Hidden;
        private const string EllipseVisibilityName = "EllipseVisibility";
        public Visibility EllipseVisibility
        {
            get { return _ellipseVisibility; }
            set
            {
                _ellipseVisibility = value;
                RaisePropertyChanged(EllipseVisibilityName);
            }
        }

        private Visibility _polylineVisibility = Visibility.Hidden;
        private const string PolylineVisibilityName = "PolylineVisibility";
        public Visibility PolylineVisibility
        {
            get { return _polylineVisibility; }
            set
            {
                _polylineVisibility = value;
                RaisePropertyChanged(PolylineVisibilityName);
            }
        }

        private int _progressValueInPercentages;
        private const string ProgressValueInPercentagesName = "ProgressValueInPercentages";
        public int ProgressValueInPercentages
        {
            get { return _progressValueInPercentages; }
            set
            {
                _progressValueInPercentages = value;
                RaisePropertyChanged(ProgressValueInPercentagesName);
            }
        }

        private string _taskTimeText = "";
        private const string TaskTimeTextName = "TaskTimeText";
        public string TaskTimeText
        {
            get { return _taskTimeText; }
            set
            {
                _taskTimeText = value;
                RaisePropertyChanged(TaskTimeTextName);
            }
        }

        private bool _isTaskRunning;
        private const string IsTaskRunningName = "IsTaskRunning";
        public bool IsTaskRunning
        {
            get
            {
                return _isTaskRunning;
            }
            set
            {
                _isTaskRunning = value;
                RaisePropertyChanged(IsTaskRunningName);
                RaisePropertyChanged(IsNotTaskRunningName);
            }
        }

        private const string IsNotTaskRunningName = "IsNotTaskRunning";
        public bool IsNotTaskRunning
        {
            get
            {
                return !_isTaskRunning;
            }
        }


        #endregion

        public MainViewModel(IPrimitives primitives)
        {
            _availiableColors.Add(new SolidColorBrush(Colors.Gold));
            _availiableColors.Add(new SolidColorBrush(Colors.Black));
            _availiableColors.Add(new SolidColorBrush(Colors.Brown));
            _availiableColors.Add(new SolidColorBrush(Colors.Coral));
            _availiableColors.Add(new SolidColorBrush(Colors.ForestGreen));

            Primitives.AddRange(primitives.GetPrimitives());

            //setup callbacks 
            foreach (var graphPrimitive in Primitives)
            {
                if (graphPrimitive.PrimitiveType == PrimitiveTypes.Pen)
                {
                    graphPrimitive.OnDrawPrimitiveInBitmap += drawPenOnBitmap;
                    graphPrimitive.OnDrawPrimitiveInteractiveState += drawPenInteractiveState;
                }
                if (graphPrimitive.PrimitiveType == PrimitiveTypes.Line)
                {
                    graphPrimitive.OnDrawPrimitiveInBitmap += drawLineOnBitmap;
                    graphPrimitive.OnDrawPrimitiveInteractiveState += drawLineInteractiveState;
                }
                if (graphPrimitive.PrimitiveType == PrimitiveTypes.Rectangle)
                {
                    graphPrimitive.OnDrawPrimitiveInBitmap += drawRectangleOnBitmap;
                    graphPrimitive.OnDrawPrimitiveInteractiveState += drawRectangleInteractiveState;
                }
                if (graphPrimitive.PrimitiveType == PrimitiveTypes.Ellipse)
                {
                    graphPrimitive.OnDrawPrimitiveInBitmap += drawEllipseOnBitmap;
                    graphPrimitive.OnDrawPrimitiveInteractiveState += drawEllipseInteractiveState;
                }
            }

            SelectedPrimitive = Primitives[0];
        }
    }
}