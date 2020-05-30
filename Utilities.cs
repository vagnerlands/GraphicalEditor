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
        static public void GetProjectedPoint(
            IEditorNode i,
            ref Point DisplayTopLeft, 
            ref Point DisplayBottomRight)
        {
            vec3 topLeft = new vec3(i.RealLocation.X, i.RealLocation.Y, 0);
            vec3 bottomRight = new vec3(i.RealLocation.X + i.RealSize.Width, i.RealLocation.Y + i.RealSize.Height, 0);
            vec3 screenTopLeft = new vec3();
            vec3 screenBottomRight = new vec3();

            // generates a new vertex
            screenTopLeft = glm.project(
                topLeft,
                i.GetModel(),
                Camera.Instance().GetProjectionView(),
                Camera.Instance().GetViewport());


            screenBottomRight = glm.project(
                bottomRight,
                i.GetModel(),
                Camera.Instance().GetProjectionView(),
                Camera.Instance().GetViewport());

            // finds the world location of this object based on the 
            //Camera.Instance().WorldToPixel(topLeft, bottomRight, ref screenTopLeft, ref screenBottomRight);

            DisplayTopLeft = new Point((int)screenTopLeft.x, (int)screenTopLeft.y);
            DisplayBottomRight = new Point((int)screenBottomRight.x, (int)screenBottomRight.y);

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
