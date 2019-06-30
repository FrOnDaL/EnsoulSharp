using EnsoulSharp.SDK;

using SharpDX;

using Path = System.Collections.Generic.List<EnsoulSharp.SDK.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<EnsoulSharp.SDK.IntPoint>>;

namespace FrOnDaL_AIO.Common.Utilities.SPrediction
{
    /// <summary>
    /// SPrediciton Clipper wrapper class
    /// </summary>
    internal static class ClipperWrapper
    {
        /// <summary>
        /// Checks if polygons are intersecting
        /// </summary>
        /// <param name="p1">Subject polygon</param>
        /// <param name="p2">Clip polygon(s)</param>
        /// <returns>true if intersects</returns>
        public static bool IsIntersects(Paths p1, params Paths[] p2)
        {
            Clipper c = new Clipper();
            Paths solution = new Paths();
            c.AddPaths(p1, PolyType.ptSubject, true);

            for (int i = 0; i < p2.Length; i++)
                c.AddPaths(p2[i], PolyType.ptClip, true);

            c.Execute(ClipType.ctIntersection, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            return solution.Count != 0;
        }

        /// <summary>
        /// Defines Rectangle Polygon
        /// </summary>
        /// <param name="start">Start Vector in 2D</param>
        /// <param name="end">End Vector in 2D</param>
        /// <param name="scale">Scale of rectangle</param>
        /// <returns>Polygon of rectangle</returns>
        public static Geometry.Polygon DefineRectangle(Vector2 start, Vector2 end, float scale)
        {
            return new Geometry.Rectangle(start, end, scale).Polygons;
        }

        /// <summary>
        /// Defines Circle Polygon
        /// </summary>
        /// <param name="c">Circle's center vector in 2D</param>
        /// <param name="r">Circle's radius</param>
        /// <returns>Polygon of Circle</returns>
        public static Geometry.Polygon DefineCircle(Vector2 c, float r)
        {
            return new Geometry.Circle(c, r).Polygons;
        }

        /// <summary>
        /// Defines Sector Polygon
        /// </summary>
        /// <param name="center">Sector's center vector in 2D</param>
        /// <param name="direction">Sector's direction vector in 2D</param>
        /// <param name="angle">Sector's angle</param>
        /// <param name="radius">Sector's radius</param>
        /// <returns>Polygon of Sector</returns>
        public static Geometry.Polygon DefineSector(Vector2 center, Vector2 direction, float angle, float radius)
        {
            return new Geometry.Sector(center, direction, angle, radius).Polygons;
        }

        /// <summary>
        /// Defines Arc Polygon
        /// </summary>
        /// <param name="c">Arc's center vector in 2D</param>
        /// <param name="direction">Arc's direction vector in 2D</param>
        /// <param name="angle">Arc's angle</param>
        /// <param name="w">Arc's width</param>
        /// <param name="h">Arc's height</param>
        /// <returns>Polygon of Arc</returns>
        public static Geometry.Polygon DefineArc(Vector2 c, Vector2 direction, float angle, float w, float h)
        {
            return new Geometry.Arc(c, direction, angle, w, h).Polygons;
        }

        /// <summary>
        /// Creates Paths from Polygon list for Clipper
        /// </summary>
        /// <param name="plist">Polygon</param>
        /// <returns>Clipper Paths of Polygons</returns>
        public static Paths MakePaths(params Geometry.Polygon[] plist)
        {
            Paths ps = new Paths(plist.Length);
            for (int i = 0; i < plist.Length; i++)
                ps.Add(plist[i].ToClipperPath());
            return ps;
        }

        /// <summary>
        /// Creates Paths from polygon for Clipper
        /// </summary>
        /// <param name="val">Polygon</param>
        /// <returns>Clipper Paths of Polygon</returns>
        public static Path ToClipperPath(this Geometry.Polygon val)
        {
            var result = new Path(val.Points.Count);

            foreach (var point in val.Points)
                result.Add(new IntPoint(point.X, point.Y));

            return result;
        }

        /// <summary>
        /// Checks if the point is outside of polygon
        /// </summary>
        /// <param name="val">Polygon to check</param>
        /// <param name="point">Point to check</param>
        /// <returns>true if point is outside of polygon</returns>
        public static bool IsOutside(this Geometry.Polygon val, Vector2 point)
        {
            var p = new IntPoint(point.X, point.Y);
            return Clipper.PointInPolygon(p, val.ToClipperPath()) != 1;
        }
    }
}
