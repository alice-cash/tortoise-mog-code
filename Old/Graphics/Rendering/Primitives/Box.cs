using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise.Shared.Drawing;
using Color = System.Drawing.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Tortoise.Graphics.Rendering.Primitives
{
    class Box: PrimitiveBase
    {

        public static void DrawBox(TGraphics tgraphics, Point point1, Point point2, Color color, int width = 1)
        {
            Rectangle rec = new Rectangle(point1, point2);
            DrawBox(tgraphics, rec, color, width);
        }

        public static void DrawBox(TGraphics tgraphics, Rectangle rec, Color color, int width = 1)
        {
            Point p1, p2, p3, p4;
            p1 = new Point(rec.X, rec.Y);
            p2 = new Point(rec.X + rec.Width, rec.Y);
            p3 = new Point(rec.X + rec.Width, rec.Y + rec.Height);
            p4 = new Point(rec.X, rec.Y + rec.Height);

            Line.DrawLine(tgraphics, p1, p2, color, width);
            Line.DrawLine(tgraphics, p2, p3, color, width);
            Line.DrawLine(tgraphics, p3, p4, color, width);
            Line.DrawLine(tgraphics, p4, p1, color, width);
        }
    }
}
