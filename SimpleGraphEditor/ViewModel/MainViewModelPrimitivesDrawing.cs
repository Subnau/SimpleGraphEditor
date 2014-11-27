using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using GraphPrimitives;

namespace SimpleGraphEditor.ViewModel
{

    //Primitives drawing logic 
    public partial class MainViewModel
    {

        private Visibility setPoints(List<Point> points, Point p)
        {
            if (points.Count > 0)
            {
                LineX1 = points[0].X;
                LineY1 = points[0].Y;
                LineX2 = p.X;
                LineY2 = p.Y;
                RaisePropertyChanged(PrimitiveLeftName);
                RaisePropertyChanged(PrimitiveWidthName);
                RaisePropertyChanged(PrimitiveTopName);
                RaisePropertyChanged(PrimitiveHeightName);
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        private void drawEllipseInteractiveState(List<Point> points, Point p)
        {
            EllipseVisibility = setPoints(points, p);
        }

        private void drawLineInteractiveState(List<Point> points, Point p)
        {
            LineVisibility = setPoints(points, p);
        }

        private void drawRectangleInteractiveState(List<Point> points, Point p)
        {
            RectangleVisibility = setPoints(points, p);
        }

        private void drawPenInteractiveState(List<Point> points, Point p)
        {
            PolylineVisibility = Visibility.Visible;
            Points = new PointCollection(points);
        }

        private void drawOnBitmap(PrimitiveTypes primitiveType, List<Point> points, int minPointsNumberForDraw)
        {
            if (points != null && points.Count > minPointsNumberForDraw)
            {
                var drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    switch (primitiveType)
                    {
                        case PrimitiveTypes.Pen:
                            if (points.Count == 1)
                                drawingContext.DrawEllipse(null, new Pen(ForeColor, 1), points[0], 1, 1);
                            else
                                for (int i = 0; i < points.Count - 1; i++)
                                {
                                    drawingContext.DrawLine(new Pen(ForeColor, 1), points[i], points[i + 1]);
                                }
                            break;
                        case PrimitiveTypes.Line:
                            drawingContext.DrawLine(new Pen(ForeColor, 1), points[0], points[1]);
                            break;
                        case PrimitiveTypes.Rectangle:
                            drawingContext.DrawRectangle(BackColor, new Pen(ForeColor, 1), new Rect(points[0], points[1]));
                            break;
                        case PrimitiveTypes.Ellipse:
                            drawingContext.DrawEllipse(BackColor, new Pen(ForeColor, 1), points[0],
                                PrimitiveRaduis,
                                PrimitiveRaduis
                                );
                            break;
                    }
                }
                BitmapImage.Render(drawingVisual);
            }
        }


        private void drawEllipseOnBitmap(List<Point> points)
        {
            drawOnBitmap(PrimitiveTypes.Ellipse, points, 1);
        }

        private void drawRectangleOnBitmap(List<Point> points)
        {
            drawOnBitmap(PrimitiveTypes.Rectangle, points, 1);
        }

        private void drawLineOnBitmap(List<Point> points)
        {
            drawOnBitmap(PrimitiveTypes.Line, points, 1);
        }

        private void drawPenOnBitmap(List<Point> points)
        {
            drawOnBitmap(PrimitiveTypes.Pen, points, 0);
        }

        private DrawingVisual createDrawingVisualFromImage(ImageSource source)
        {
            var drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(source, new Rect(new Size(source.Width, source.Height)));
                drawingContext.Close();
            }
            return drawingVisual;
        }
    }
}