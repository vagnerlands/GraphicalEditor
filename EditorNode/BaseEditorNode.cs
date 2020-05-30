using System.Drawing;
using System.Windows.Forms;

namespace editor
{
    class BaseEditorNode : IEditorNode
    {
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

        private Point _pixelLocation;
        public Point Location
        {
            get => _pixelLocation;
            set
            {
                _pixelLocation = value;
            }
        }

        private Size _pixelSize;
        public Size Size
        {
            get => _pixelSize;
            set
            {
                _pixelSize = value;
            }
        }

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

    }
}
