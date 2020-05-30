using GlmNet;
using System;
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

        /// <summary>
        /// Get the Real and the Display locations of an image.
        /// </summary>
        /// <param name="i">An image of type PictureNode</param>
        /// <param name="RealTopLeft">returns the top left world location of this object</param>
        /// <param name="RealBottomRight">returns the bottom right world location of this object</param>
        /// <param name="DisplayTopLeft">returns the top left location for display</param>
        /// <param name="DisplayBottomRight">returns the bottom right location for display</param>
        static public void GetProjectedPoint(IEditorNode i, ref GlmNet.vec3 RealTopLeft, ref GlmNet.vec3 RealBottomRight, ref GlmNet.vec3 DisplayTopLeft, ref GlmNet.vec3 DisplayBottomRight)
        {
            GlmNet.vec3 topLeft = new GlmNet.vec3(i.RealLocation.X, i.RealLocation.Y, 0);
            GlmNet.vec3 bottomRight = new GlmNet.vec3(i.RealLocation.X + i.RealSize.Width, i.RealLocation.Y + i.RealSize.Height, 0);

            // generates a new vertex
            DisplayTopLeft = GlmNet.glm.project(
                topLeft,
                i.GetModel(),
                Camera.Instance().GetProjectionView(),
                Camera.Instance().GetViewport());


            DisplayBottomRight = GlmNet.glm.project(
                bottomRight,
                i.GetModel(),
                Camera.Instance().GetProjectionView(),
                Camera.Instance().GetViewport());

            // finds the world location of this object based on the 
            Camera.Instance().FromPixelToWorld(topLeft, bottomRight, ref RealTopLeft, ref RealBottomRight);

        }

        internal static Point ScreenToWorld(Point screenPoint, IEditorNode i)
        {
            // converts it to vec3
            GlmNet.vec3 scrPt = Utilities.PointToVec3(screenPoint);

            // project from screen to world location this point, according to the current 
            // projection_view matrix.
            GlmNet.vec3 unprojectedPoint1 = GlmNet.glm.unProject(
                scrPt,
                i.GetModel(),
                Camera.Instance().GetProjectionView(),
                Camera.Instance().GetViewport());

            // converts to point
            return new Point((int)unprojectedPoint1.x, (int)unprojectedPoint1.y);
        }
    }
}
