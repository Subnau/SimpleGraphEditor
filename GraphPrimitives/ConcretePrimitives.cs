using System.Windows;

namespace GraphPrimitives
{
    //concrete primitives realization

    internal class GraphPen : GraphPrimitive
    {

        public override void MouseMove(bool leftPressed, bool rightPressed, Point p)
        {
            if (Points.Count > 0 && leftPressed)
            {
                Points.Add(p);
                RaiseDrawPrimitiveInteractiveState(p);
            }
            else if (Points.Count > 0)
            {
                RaiseDrawPrimitiveInBitmap();
                Reset();
            }
        }

        public override void MouseDown(bool leftPressed, bool rightPressed, Point p)
        {
            if (leftPressed)
            {
                Points.Add(p);
                RaiseDrawPrimitiveInteractiveState(p);
            }
        }

        public override void MouseUp(bool leftPressed, bool rightPressed, Point p)
        {
            if (!leftPressed)
            {
                RaiseDrawPrimitiveInBitmap();
                Reset();
            }

        }

        public GraphPen(PrimitiveTypes primitiveType, string displayName)
            : base(primitiveType, displayName)
        {

        }
    }

    internal class GraphLine : GraphPrimitive
    {

        public override void MouseDown(bool leftPressed, bool rightPressed, Point p)
        {
            Points.Add(p);
            if (Points.Count > 1)
            {
                RaiseDrawPrimitiveInBitmap();
                Reset();
            }
        }

        public override void MouseMove(bool leftPressed, bool rightPressed, Point p)
        {
            if (Points.Count == 1)
            {
                RaiseDrawPrimitiveInteractiveState(p);
            }

        }

        public GraphLine(PrimitiveTypes primitiveType, string displayName)
            : base(primitiveType, displayName)
        {

        }
    }
}
