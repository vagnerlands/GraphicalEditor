
using System.Drawing;
using System.Windows.Forms;

//using mat

namespace editor
{
    class PictureNode : PictureBox
    {
        public enum eHighlightOptions
        {
            Highlight_None = 0,
            Highlight_Border,
            Highlight_Glowing
        }

        public GlmNet.mat4 mModel = new GlmNet.mat4(1);

        public static Panel mBluePrint = null;

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

        public string GetId()
        {
            return Name;
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

        public void SetPosition(GlmNet.vec3 upperLeft, GlmNet.vec3 bottomRight)
        {
            Size = new Size((int)(bottomRight.x - upperLeft.x), (int)(bottomRight.y - upperLeft.y));
            Location = new Point((int)upperLeft.x -Size.Width/2, (int)upperLeft.y - Size.Height / 2);

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

        public new void Move(Point newLocation, float viewRatio)
        {
            //int dX = (int)System.Math.Round((newLocation.X - Location.X) * viewRatio);
            //int dY = (int)System.Math.Round((newLocation.Y - Location.Y) * viewRatio);
            // adjust view location
            //Location = new Point(newLocation.X - (Size.Width / 2), newLocation.Y - (Size.Height / 2));
            // adjust real location according to the resolution ratio
            RealLocation = new Point(newLocation.X , newLocation.Y );
            //Location = new Point(RealLocation.X + mBluePrint.Width / 2 - RealSize.Width/2, RealLocation.Y + mBluePrint.Width / 2 - RealSize.Height / 2);
            //adjustSocketSizeAndLocation();
        }

        //public new void Move2(Point newLocation, Point realPoint)
        //{
        //    // adjust view location
        //    Location = new Point(newLocation.X - (Size.Width / 2), newLocation.Y - (Size.Height / 2));
        //    // adjust real location according to the resolution ratio

        //    RealLocation = realPoint;
        //    adjustSocketSizeAndLocation();
        //}

        public void Highlight(eHighlightOptions option)
        {
            // do something
        }
    }
}
