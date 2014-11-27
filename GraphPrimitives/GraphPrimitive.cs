using System;
using System.Collections.Generic;
using System.Windows;

namespace GraphPrimitives
{

    /// <summary>
    /// Base class for primitive
    /// </summary>
    public abstract class GraphPrimitive
    {

        private readonly string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
        }

        public List<Point> Points { get; private set; }

        private readonly PrimitiveTypes _primitiveType;
        public PrimitiveTypes PrimitiveType
        {
            get { return _primitiveType; }
        }

        public virtual void MouseDown(bool leftPressed, bool rightPressed, Point p)
        {

        }

        public virtual void MouseUp(bool leftPressed, bool rightPressed, Point p)
        {

        }

        public virtual void MouseMove(bool leftPressed, bool rightPressed, Point p)
        {

        }

        public virtual void Reset()
        {
            Points.Clear();
            RaiseDrawPrimitiveInteractiveState(new Point(0, 0));
        }

        /// <summary>
        /// Rendering primitive on bitmap
        /// </summary>
        public event Action<List<Point>> OnDrawPrimitiveInBitmap = delegate { };

        /// <summary>
        /// Drawing primitive with not completed state 
        /// </summary>
        public event Action<List<Point>, Point> OnDrawPrimitiveInteractiveState = delegate { };

        protected void RaiseDrawPrimitiveInBitmap()
        {
            OnDrawPrimitiveInBitmap(Points);
        }

        protected void RaiseDrawPrimitiveInteractiveState(Point p)
        {
            OnDrawPrimitiveInteractiveState(Points, p);
        }

        protected GraphPrimitive(PrimitiveTypes primitiveType, string displayName)
        {
            _primitiveType = primitiveType;
            _displayName = displayName;
            Points = new List<Point>();
        }
    }
}
