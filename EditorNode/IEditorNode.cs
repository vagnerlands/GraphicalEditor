using System.Drawing;
using GlmNet;

namespace editor
{
    interface IEditorNode
    {
        Point RealLocation { get; set; }
        Size RealSize { get; set; }
        //string Identification { get; set; }

        mat4 GetModel();

        void SetPosition(Point upperLeft, Point bottomRight);

        void Translate(Point newLocation, float viewRatio);

    }
}
