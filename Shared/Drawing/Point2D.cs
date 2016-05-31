using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClassicPoint = System.Drawing.Point;
using ClassicPointF = System.Drawing.PointF;

using RenderPoint = Microsoft.Xna.Framework.Point;

using Size = Tortoise.Shared.Drawing.Size;

namespace Tortoise.Shared.Drawing
{
    public struct Point
    {
        public static readonly Point Empty;

        private int _x;
        private int _y;

        public ClassicPoint ToPoint
        {
            get
            {
                return new ClassicPoint(_x, _y);
            }
        }

        public ClassicPointF ToPointF
        {
            get
            {
                return new ClassicPointF(_x, _y);
            }
        }

        public RenderPoint ToRenderPoint
        {
            get
            {
                return new RenderPoint(_x, _y);
            }
        }

        public static Point FromPointF(ClassicPointF ptf, bool round = false)
        {
            int x, y;
            if (round)
            {
                x = (int)Math.Round(ptf.X);
                y = (int)Math.Round(ptf.Y);
            }
            else
            {
                x = (int)ptf.X;
                y = (int)ptf.Y;
            }
            return new Point(x, y);
        
        }

        public static Point FromPoint(ClassicPoint pt)
        {
            return new Point(pt.X, pt.Y);
        }

        public static Point FromPoint(RenderPoint pt)
        {
            return new Point(pt.X, pt.Y);
        }


        //public Point(int dw);
        public Point(Size sz)
        {
            _x = sz.Width;
            _y = sz.Height;
        }


        public Point(Point sz)
        {
            _x = sz.X;
            _y = sz.Y;
        }

        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }
        /*
        public Point2D(ClassicPoint point)
        {
            _x = point.X;
            _y = point.Y;
        }

        */
        public static Point operator -(Point pt, Size sz)
        {
            return new Point(sz.Width - pt.X, sz.Height - pt.Y);

        }
        public static Point operator +(Point pt, Size sz)
        {
            return new Point(sz.Width + pt.X, sz.Height + pt.Y);
        }
        

        public static Point operator -(Point pt1, Point pt2)
        {
            return new Point(pt1.X - pt2.X, pt1.Y - pt2.Y);

        }
        public static Point operator +(Point pt1, Point pt2)
        {
            return new Point(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }


        public static Point operator *(Point pt1, Point pt2)
        {
            return new Point(pt1.X * pt2.X, pt1.Y * pt2.Y);

        }
        public static Point operator /(Point pt1, Point pt2)
        {
            return new Point(pt1.X / pt2.X, pt1.Y / pt2.Y);
        }

        

        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        /*
        public static bool operator ==(Point left, ClassicPoint right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        */
        public static bool operator !=(Point left, Point right)
        {
            return !(left.X == right.X && left.Y == right.Y);
        }
        /*
        public static bool operator !=(Point left, ClassicPoint right)
        {
            return !(left.X == right.X && left.Y == right.Y);
        }*/


        public static explicit operator Size(Point p)
        {
            return new Size(p);
        }



        //public static implicit operator PointF(Point p);
        public static explicit operator Point(ClassicPoint p)
        {
            return Point.FromPoint(p);
        }

        public static explicit operator Point(RenderPoint p)
        {
            return Point.FromPoint(p);
        }


        /*


        public static implicit operator ClassicPoint(Point p)
        {
            return new ClassicPoint(p._x, p._y);
        }
        */
        public bool IsEmpty { get { return this == Empty; } }
        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }


        //public static Point Add(Point pt, Size sz)
        //public static Point Ceiling(PointF value);
        public override bool Equals(object obj)
        {
            if (obj is Point || obj is ClassicPoint)
            {
                return this == (Point)obj;
            }
            return false;
        }



        public override int GetHashCode()
        {
            unchecked
            {
                return _x ^ _y;
            }
        }
        //public void Offset(Point p);

        //public void Offset(int dx, int dy);
        //public static Point Round(PointF value);
        //public static Point Subtract(Point pt, Size sz);
        public override string ToString()
        {
            return "{X=" + X + ",Y=" + Y + "}";
        }
        //public static Point Truncate(PointF value);
    }
}
