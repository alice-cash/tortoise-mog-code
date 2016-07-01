using Vector2 = Microsoft.Xna.Framework.Vector2;
using XColor = Microsoft.Xna.Framework.Color;
using Color = System.Drawing.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Tortoise.Shared.Drawing;

namespace Tortoise.Graphics.Rendering.Primitives
{
    class Line : PrimitiveBase
    {
        public static void DrawLine(TGraphics tgraphics, Point point, int length, float angle, Color color, int width = 1)
        {
            Surface pixel = getPixel(tgraphics);
            pixel.Draw(point, angle, Point.Zero, new Point(length, width), color);
        }


        public static void DrawLine(TGraphics tgraphics, Point point1, Point point2, Color color, int width = 1)
        {
            int length = Point.Distance(point1, point2);

            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(tgraphics, point1,  length, angle, color, width);
        }


    }


}

