
using System.Drawing;
using System.Windows.Forms;

//using mat

namespace editor
{
    class PictureNode : PictureBox, IEditorNode
    {
        public enum eHighlightOptions
        {
            Highlight_None = 0,
            Highlight_Border,
            Highlight_Glowing
        }

        public static Panel mBluePrint = null;

        private GlmNet.mat4 mModel = new GlmNet.mat4(1);

        private PictureBox mSocketPic;

        private int mNextId = 0;

        private Point _realLocation;
        public Point RealLocation
        {
            get => _realLocation;
            set
            {
                _realLocation = value;
            }
        }

        private Size _realSize;
        public Size RealSize
        {
            get => _realSize;
            set
            {
                _realSize = value;
            }
        }

        private int getUniqueId()
        {
            return ++mNextId;
        }

        public override string ToString()
        {
            return Name + " world location " + RealLocation.ToString() + " pixel location " + Location.ToString();
        }

        public PictureNode(Bitmap image, Point location, Size dimension, MouseEventHandler OnMouseDown, MouseEventHandler OnMouseUp)
        {
            // model
            mModel = GlmNet.mat4.identity();
            // create main background image
            BackgroundImage = image;
            BackgroundImageLayout = ImageLayout.Stretch;
            Location = location;
            RealLocation = location;
            Name = "object" + getUniqueId();
            Size = dimension;
            RealSize = dimension;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            Cursor = Cursors.Cross;
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            mBluePrint.Controls.Add(this);

            mSocketPic = new PictureBox();
            mSocketPic.BackgroundImage = Properties.Resources.socket_empty;
            mSocketPic.BackgroundImageLayout = ImageLayout.Stretch;
            mSocketPic.Name = "object" + getUniqueId() + "_s";
            mSocketPic.BackColor = Color.Transparent;
            adjustSocketSizeAndLocation();

            mBluePrint.Controls.Add(mSocketPic);
            mBluePrint.Controls.SetChildIndex(mSocketPic, 0);
        }

        public GlmNet.mat4 GetModel()
        {
            return mModel;
        }

        public void SetPosition(Point upperLeft, Point bottomRight)
        {
            Size = new Size(bottomRight.X - upperLeft.X, bottomRight.Y - upperLeft.Y);
            Location = new Point(upperLeft.X -Size.Width/2, upperLeft.Y - Size.Height / 2);

            // socket icon
            adjustSocketSizeAndLocation();
        }

        private void adjustSocketSizeAndLocation()
        {
            mSocketPic.Location = new Point(
                Location.X + 1,
                Location.Y + (Size.Height / 2));
            mSocketPic.Size = new Size(Size.Width / 5, Size.Height / 5);
        }

        public void Translate(Point newLocation, float viewRatio)
        {
            // adjust real location according to the resolution ratio
            RealLocation = new Point(newLocation.X , newLocation.Y );
        }

        public void Render()
        {
            // [out parameters]
            Point DisplayTopLeft = new Point();
            Point DisplayBottomRight = new Point();

            // finds where the point should be displayed and where should be its real location
            // since resolution, screen size, position and more may change, this should be done
            Utilities.GetProjectedPoint(this, ref DisplayTopLeft, ref DisplayBottomRight);

            // update the display position of the object
            SetPosition(DisplayTopLeft, DisplayBottomRight);
        }

        public void Highlight(eHighlightOptions option)
        {
            // do something
        }
    }
}
