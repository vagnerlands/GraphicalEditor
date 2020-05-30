using System;
using System.Drawing;
using System.Windows.Forms;

namespace editor
{
    public partial class ComputerNode : UserControl
    {
        private Size mOriginalSize = new Size();
        public GlmNet.mat4 GetModel()
        {
            return mModel;
        }

        public void SetPosition(Point upperLeft, Point bottomRight)
        {
            Size = new Size(bottomRight.X - upperLeft.X, bottomRight.Y - upperLeft.Y);
            Location = new Point(upperLeft.X - Size.Width / 2, upperLeft.Y - Size.Height / 2);
        }

        public void Translate(Point newLocation, float viewRatio)
        {
            // adjust real location according to the resolution ratio
            RealLocation = new Point(newLocation.X, newLocation.Y);
        }


        public GlmNet.mat4 mModel = new GlmNet.mat4(1);

        public static Panel mBluePrint = null;

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

        static private int _inc = 0;

        public ComputerNode(MouseEventHandler onMouseDown, MouseEventHandler onMouseUp)
        {
            InitializeComponent();

            MouseDown += onMouseDown;
            MouseUp += onMouseUp;
            RealLocation = new Point(0, 0);
            mOriginalSize = RealSize = Size;
            lblComputerName.Text = "comp_"+ _inc++;

            Cursor = Cursors.Cross;
            //DoubleBuffered = true;
            mBluePrint.Controls.Add(this);
        }


        private void ComputerNode_SizeChanged(object sender, EventArgs e)
        {
            float resizeOnX = (float)(Size.Width) / mOriginalSize.Width;
            float resizeOnY = (float)(Size.Height) / mOriginalSize.Height;
            int lastYPosition = 0;
            foreach (Control c in Controls)
            {
                c.Location = new Point(c.Location.X, lastYPosition);
                c.Font = new Font(c.Font.FontFamily, 8.25f * resizeOnY, FontStyle.Regular);
                lastYPosition = c.Location.Y + c.Size.Height + 3;
            }
        }
    }
}
