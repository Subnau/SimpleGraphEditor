using System.Collections.Generic;

namespace GraphPrimitives
{
    public enum PrimitiveTypes
    {
        Pen,
        Line,
        Rectangle,
        Ellipse
    }


    public interface IPrimitives
    {
        List<GraphPrimitive> GetPrimitives();
    }

    /// <summary>
    /// Incapsulate all available primitives
    /// </summary>
    public class Primitives : IPrimitives
    {
        public List<GraphPrimitive> GetPrimitives()
        {
            var primitives = new List<GraphPrimitive>();
            primitives.Add(new GraphPen(PrimitiveTypes.Pen, "Pen"));
            primitives.Add(new GraphLine(PrimitiveTypes.Line, "Line"));
            primitives.Add(new GraphLine(PrimitiveTypes.Rectangle, "Rectangle"));
            primitives.Add(new GraphLine(PrimitiveTypes.Ellipse, "Ellipse"));
            return primitives;
        }

        public Primitives()
        {}
    }
}
