using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace editor
{
    public partial class Form1 : Form
    {
        List<object> mImages = null;
        private object mDragObject = null;
        private bool mIsDraggingPanel = false;
        // affects how big the icons should be...
        private int mCameraToBlueprintDistance = 1;
        private bool _mustRender = false;

        // all tricks are done using a orthogonal matrix
        //GlmNet.mat4 mOrthoMat = GlmNet.mat4.identity();
        //GlmNet.vec3 mCameraLocation = new GlmNet.vec3();
        //GlmNet.mat4 mLookat = GlmNet.mat4.identity();
        private Camera mCamera = new Camera();
        //GlmNet.vec4 mViewport = new GlmNet.vec4() ;
        //private GlmNet.mat4 mProjectionView = GlmNet.mat4.identity();
        //private GlmNet.mat4 mWorldToCamera = GlmNet.mat4.identity();
        private Cursor grabbing = Cursors.NoMove2D;
        private Cursor nograbbing = Cursors.Hand;
        private Cursor oldcursor = Cursors.Arrow;
        private GlmNet.vec3 previousScrPt = new GlmNet.vec3();

        public Form1()
        {
            InitializeComponent();
            mImages = new List<object>();
            PictureNode.mBluePrint = panel1;

            mCamera.SetViewport(panel1.Width, panel1.Height);

            trackBar1.Maximum = panel1.Width - 1;
            trackBar1.Value = mCameraToBlueprintDistance;
            trackBar1.Minimum = 1;

            label1.Text = trackBar1.Value.ToString();

            //refreshMatrices();
            RenderBlueprint();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // create new icon and push to the database
            PictureNode newPic = new PictureNode(
                Properties.Resources.laptop, 
                new Point(0,0), 
                new Size(40,40), 
                new MouseEventHandler(pictureBox1_MouseDown), 
                new MouseEventHandler(panel1_MouseUp));
            // keeps a reference for manipulation, animations and so on...
            mImages.Add(newPic);
            //refreshMatrices();
            RenderBlueprint();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (mDragObject == null)
            {
                // selects the current object
                mDragObject = sender;
                PictureNode hPicture = (PictureNode)mDragObject;
                hPicture.Highlight(PictureNode.eHighlightOptions.Highlight_Border);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mDragObject != null)
            {
                PictureNode hPicture = (PictureNode)mDragObject;
                hPicture.Highlight(PictureNode.eHighlightOptions.Highlight_None);
                mDragObject = null;
            }

            if (mIsDraggingPanel)
            {
                panel1.Cursor = oldcursor;
                mIsDraggingPanel = false;

                // unproject the screen point into world space
                // find the cursor position on this application
                Point screenPoint = PointToClient(new Point(Cursor.Position.X - panel1.Location.X, Cursor.Position.Y - panel1.Location.Y));
                // converts it to vec3
                GlmNet.vec3 scrPt = Utilities.PointToVec3(screenPoint);

                GlmNet.vec3 movement = previousScrPt - scrPt;
                movement *= 0.1f;

                mCamera.StrifeCamera(movement);

                System.Console.WriteLine(" Cursor    : " + scrPt.ToString());
                System.Console.WriteLine(" Delta     : " + movement.ToString());
                System.Console.WriteLine(" Camera    : " + mCamera.GetCameraPosition().ToString());

                // refresh matrices...
                //refreshMatrices();
                _mustRender = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (mDragObject != null)
                { 
                    // do something complex here, such as animations
                    PictureNode i = (PictureNode)mDragObject;

                    // find the cursor position on this application
                    Point screenPoint = PointToClient(new Point(Cursor.Position.X - panel1.Location.X, Cursor.Position.Y - panel1.Location.Y));

                    // find the adjusted pointer location over the panel1 object
                    Point newRealPoint = new Point(screenPoint.X- panel1.Width / 2, screenPoint.Y - panel1.Height/2);

                    // converts it to vec3
                    GlmNet.vec3 scrPt = Utilities.PointToVec3(screenPoint);

                    // project from screen to world location this point, according to the current 
                    // projection_view matrix.
                    GlmNet.vec3 unprojectedPoint1 = GlmNet.glm.unProject(
                        scrPt,
                        i.mModel,
                        mCamera.GetProjectionView(),
                        mCamera.GetViewport());

                    System.Console.WriteLine(" Unprojected " + unprojectedPoint1.ToString());
                    System.Console.WriteLine(" Screen X[" + screenPoint.X + "] Y[" + screenPoint.Y + "]");
                    System.Console.WriteLine(" Adjusted Screen [" + scrPt.ToString() + "]");

                    // update the "real" location of this object in world space
                    i.Move(new Point((int)unprojectedPoint1.x, (int)unprojectedPoint1.y), 1);
                    
                    // flags it to be updated
                    _mustRender = true;
                }

                if (_mustRender)
                {
                    _mustRender = false;
                    // converts all world space objects into screen space
                    RenderBlueprint();
                }
            }
            catch (Exception )
            {
                int i = 0;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            mCamera.MoveCameraOnZ(trackBar1.Value- mCameraToBlueprintDistance);
            mCameraToBlueprintDistance = trackBar1.Value;
            _mustRender = true;

            //refreshMatrices();

        }

        /// <summary>
        /// Get the Real and the Display locations of an image.
        /// </summary>
        /// <param name="i">An image of type PictureNode</param>
        /// <param name="RealTopLeft">returns the top left world location of this object</param>
        /// <param name="RealBottomRight">returns the bottom right world location of this object</param>
        /// <param name="DisplayTopLeft">returns the top left location for display</param>
        /// <param name="DisplayBottomRight">returns the bottom right location for display</param>
        private void GetProjectedPoint(PictureNode i, ref GlmNet.vec3 RealTopLeft, ref GlmNet.vec3 RealBottomRight, ref GlmNet.vec3 DisplayTopLeft, ref GlmNet.vec3 DisplayBottomRight)
        {
            GlmNet.vec3 topLeft = new GlmNet.vec3(i.RealLocation.X, i.RealLocation.Y, 0);
            GlmNet.vec3 bottomRight = new GlmNet.vec3(i.RealLocation.X + i.RealSize.Width, i.RealLocation.Y + i.RealSize.Height, 0);

            // generates a new vertex
            DisplayTopLeft = GlmNet.glm.project(
                topLeft,
                i.mModel,
                mCamera.GetProjectionView(),
                mCamera.GetViewport());


            DisplayBottomRight = GlmNet.glm.project(
                bottomRight,
                i.mModel,
                mCamera.GetProjectionView(),
                mCamera.GetViewport());

            // finds the world location of this object based on the 
            mCamera.FromPixelToWorld(topLeft, bottomRight, ref RealTopLeft, ref RealBottomRight);

        }


        /// <summary>
        /// Based on current camera location, render the objects according to its location.
        /// </summary>
        private void RenderBlueprint()
        {
            // https://www.scratchapixel.com/lessons/3d-basic-rendering/computing-pixel-coordinates-of-3d-point/mathematics-computing-2d-coordinates-of-3d-points

            // iterate throug all objects and readjust their vertexes according to the current ortho matrix
            foreach (object im in mImages)
            {
                // down-cast to picturenode - not safe
                PictureNode i = ((PictureNode)im);

                // [out parameters]
                GlmNet.vec3 RealTopLeft = new GlmNet.vec3();
                GlmNet.vec3 RealBottomRight = new GlmNet.vec3();
                GlmNet.vec3 DisplayTopLeft = new GlmNet.vec3();
                GlmNet.vec3 DisplayBottomRight = new GlmNet.vec3();
                // finds where the point should be displayed and where should be its real location
                // since resolution, screen size, position and more may change, this should be done
                GetProjectedPoint(i, ref RealTopLeft, ref RealBottomRight, ref DisplayTopLeft, ref DisplayBottomRight);

                // change the real location of the object
               // i.Move(new Point((int)RealTopLeft.x, (int)RealTopLeft.y), 1);
                // update the display position of the object
                i.SetPosition(DisplayTopLeft, DisplayBottomRight);

                System.Console.WriteLine("Display : TL: " + DisplayTopLeft.ToString() + "          BR: " + DisplayBottomRight.ToString());
                System.Console.WriteLine("Real    : TL: " + RealTopLeft.ToString() + "          BR: " + RealTopLeft.ToString());
            }
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            oldcursor = Cursor.Current;
            panel1.Cursor = nograbbing;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.Cursor = oldcursor;
            mIsDraggingPanel = false;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mIsDraggingPanel = true;
            panel1.Cursor = grabbing;
            // unproject the screen point into world space
            // find the cursor position on this application
            Point screenPoint = PointToClient(new Point(Cursor.Position.X - panel1.Location.X, Cursor.Position.Y - panel1.Location.Y));
            // converts it to vec3
            previousScrPt= Utilities.PointToVec3(screenPoint);
        }


        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDragObject != null)
            {
                // do nothing
            }

            if (mIsDraggingPanel)
            {
                // most move the camera location and refresh all rendering
            }
        }

    }
}
