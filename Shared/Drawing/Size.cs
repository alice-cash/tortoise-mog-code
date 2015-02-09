using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ClassicSize = System.Drawing.Size;

namespace Tortoise.Shared.Drawing
{
    public struct Size
    {
        public static readonly Size Empty;

        private int _width;
        private int _height;

        //public Point(int dw);
        public Size(Point pt)
        {
            _width = pt.X;
            _height = pt.Y;
        }
        public Size(int width, int height)
        {
            _width = width;
            _height = height;
        }

        /*
        public Size(ClassicSize sz)
        {
            _width = sz.Width;
            _height = sz.Height;
        }*/

        public static Size operator -(Size sz, Point pt)
        {
            return new Size(sz.Width - pt.X, sz.Height - pt.Y);
        }
        public static Size operator +(Size sz, Point pt)
        {
            return new Size(sz.Width + pt.X, sz.Height + pt.Y);
        }



        public static bool operator ==(Size left, Size right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }
        /*
        public static bool operator ==(Size left, ClassicSize right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }*/

        public static bool operator !=(Size left, Size right)
        {
            return !(left.Width == right.Width && left.Height == right.Height);
        }
        /*
        public static bool operator !=(Size left, ClassicSize right)
        {
            return !(left.Width == right.Width && left.Height == right.Height);
        }*/


        
        public static explicit operator Point(Size sz)
        {
            return new Point(sz);
        }

        /*
        public static explicit operator Size(ClassicSize p)
        {
            return new Size(p);
        }
        */
        /*
        public static implicit operator ClassicSize(Size p)
        {
            return new ClassicSize(p._width, p._height);
        }*/
        
        public bool IsEmpty { get { return this == Empty; } }
        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }


        //public static Point Add(Point pt, Size sz)
        //public static Point Ceiling(PointF value);
        public override bool Equals(object obj)
        {
            if (obj is Size)// || obj is ClassicSize)
            {
                return this == (Size)obj;
            }
            return false;
        }
        //public override int GetHashCode();
        //public void Offset(Point p);

        //public void Offset(int dx, int dy);
        //public static Point Round(PointF value);
        //public static Point Subtract(Point pt, Size sz);
        public override string ToString()
        {
            return "{Width=" + Width + ",Height=" + Height + "}";
        }
        //public static Point Truncate(PointF value);
    }
}
