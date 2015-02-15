using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClassicRectangle = System.Drawing.Rectangle;
using ClassicRectangleF = System.Drawing.RectangleF;

namespace Tortoise.Shared.Drawing
{
    public struct Rectangle
    {

        public static readonly Rectangle Empty;

        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }


        public Rectangle(Point location, Size size) : this(location.X, location.Y, size.Width, size.Height) { }

        public Rectangle(int x, int y, int width, int height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return
                left.Left != right.Left ||
                left.Top != right.Top ||
                left.Right != right.Right ||
                left.Bottom != right.Bottom;
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return
                left.Left == right.Left &&
                left.Top == right.Top &&
                left.Right == right.Right &&
                left.Bottom == right.Bottom;
        }

        public bool IsEmpty { get { return this == Empty; } }

        public int Left { get { return X; } }
        public int Right { get { return X + Width; } }
        public int Top { get { return Y; } }
        public int Bottom { get { return Y + Height; } }

        public Size Size
        {
            get { return new Drawing.Size(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        public Point Location
        {
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }


        public ClassicRectangle ToSystem()
        {
            return new ClassicRectangle(X, Y, Width, Height);
        }

        public ClassicRectangleF ToSystemF()
        {
            return new ClassicRectangleF(X, Y, Width, Height);
        }

        //public static Rectangle Ceiling(RectangleF value);
        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }
        public bool Contains(Rectangle rect)
        {
            return this == Empty ? false : Intersect(this, rect) == this;
        }
        public bool Contains(int x, int y)
        {
            return x > Left && x < Right && y > Top && y < Bottom;
        }

        public override bool Equals(object obj)
        {
            return obj is Rectangle ? ((Rectangle)obj) == this : false;
        }

        public static Rectangle FromLTRB(int left, int top, int right, int bottom)
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }

        public override int GetHashCode()
        {
            return unchecked(Left * Right * Top * Bottom);
        }

        public void Inflate(Size size)
        {
            Inflate(size.Width, size.Height);
        }

        public void Inflate(int width, int height)
        {
            this.Width += width;
            this.Height += height;
        }

        public static Rectangle Inflate(Rectangle rect, int width, int height)
        {
            Rectangle result = rect.Copy();
            result.Inflate(width, height);
            return result;
        }

        public Rectangle Copy()
        {
            return new Rectangle(this.Location, this.Size);
        }

        public void Intersect(Rectangle rect)
        {
            this = Intersect(this, rect);
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            Rectangle result = FromLTRB(
                max(a.Left, b.Left),
                max(a.Top, b.Top),
                min(a.Right, b.Right),
                min(a.Bottom, b.Bottom));

            return isNegitiveArea(result) ? Empty : result;
        }

        private static bool isNegitiveArea(Rectangle rect)
        {
            return rect.Width < 0 || rect.Height < 0;
        }

        private static int min(int a, int b)
        {
            return a > b ? b : a;
        }

        private static int max(int a, int b)
        {
            return a < b ? b : a;
        }

        public bool IntersectsWith(Rectangle rect)
        {
            return this == Empty ? false : Intersect(this, rect) != Empty;
        }


        public void Offset(Point pos)
        {
            Offset(pos.X, pos.Y);
        }


        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }


        // public static Rectangle Round(RectangleF value);


        public override string ToString()
        {
            return string.Format("[Width=\"{0}\", Height=\"{1}\", X=\"{2}\", Y=\"{3}\"]", Width, Height,X,Y);
        }


        //public static Rectangle Truncate(RectangleF value);


        //public static Rectangle Union(Rectangle a, Rectangle b);

    }
}
