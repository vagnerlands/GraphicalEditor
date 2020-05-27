using GlmNet;
using System.Drawing;

namespace editor
{
    class Utilities
    {
        public static vec3 PointToVec3(Point p1)
        {
            return new vec3(p1.X, p1.Y, 0);
        }

        public static Point Vec3ToPoint(vec3 p1)
        {
            return new Point((int)p1.x, (int)p1.y);
        }
    }
}
