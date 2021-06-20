using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClassicPoint = System.Drawing.Point;
using ClassicPointF = System.Drawing.PointF;

using RenderPoint = Microsoft.Xna.Framework.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Size = Tortoise.Shared.Drawing.Size;

namespace Tortoise.Shared.Drawing
{
    public struct PointF
    {
        public static readonly PointF Empty;
        public static readonly PointF Zero = new PointF(0,0);

        private float _x;
        private float _y;

        /*
        public ClassicPoint ToPoint
        {
            get
            {
                return new ClassicPoint(_x, _y);
            }
        }*/

        public ClassicPointF ToPointF
        {
            get
            {
                return new ClassicPointF(_x, _y);
            }
        }

        /*public RenderPoint ToRenderPoint
        {
            get
            {
                return new RenderPoint(_x, _y);
            }
        }*/

        public static float Distance(PointF point1, PointF point2)
        {
            return (float)Math.Sqrt(_power2(point1.X - point2.X) + _power2(point1.Y - point2.Y));
        }

        private static float _power2(float i)
        {
            return i * i;
        }

        public Vector2 ToVector2
        {
            get
            {
                return new Vector2(_x, _y);
            }
        }

        public static PointF FromPointF(ClassicPointF ptf)
        {
            return new PointF(ptf.X, ptf.Y);
        }

        public static PointF FromPoint(ClassicPoint pt)
        {
            return new PointF(pt.X, pt.Y);
        }

        public static PointF FromPoint(RenderPoint pt)
        {
            return new PointF(pt.X, pt.Y);
        }

        public static PointF FromVector2(Vector2 pt)
        {
            return new PointF(pt.X, pt.Y);
        }


        //public Point(int dw);
        public PointF(Size sz)
        {
            _x = sz.Width;
            _y = sz.Height;
        }


        public PointF(Point sz)
        {
            _x = sz.X;
            _y = sz.Y;
        }

        public PointF(int x, int y)
        {
            _x = x;
            _y = y;
        }


        public PointF(PointF sz)
        {
            _x = sz.X;
            _y = sz.Y;
        }

        public PointF(float x, float y)
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
        public static PointF operator -(PointF pt, Size sz)
        {
            return new PointF(sz.Width - pt.X, sz.Height - pt.Y);

        }
        public static PointF operator +(PointF pt, Size sz)
        {
            return new PointF(sz.Width + pt.X, sz.Height + pt.Y);
        }


        public static PointF operator -(PointF pt1, PointF pt2)
        {
            return new PointF(pt1.X - pt2.X, pt1.Y - pt2.Y);

        }

        public static PointF operator +(PointF pt1, PointF pt2)
        {
            return new PointF(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }


        public static PointF operator -(PointF pt1, float value)
        {
            return new PointF(pt1.X - value, pt1.Y - value);
        }


        public static PointF operator -(float value, PointF pt1)
        {
            return new PointF(value - pt1.X, value - pt1.Y);
        }

        public static PointF operator +(PointF pt1, float value)
        {
            return new PointF(pt1.X + value, pt1.Y + value);
        }


        public static PointF operator *(PointF pt1, PointF pt2)
        {
            return new PointF(pt1.X * pt2.X, pt1.Y * pt2.Y);
        }

        public static PointF operator *(PointF pt1, float value)
        {
            return new PointF(pt1.X * value, pt1.Y * value);
        }

        public static PointF operator /(PointF pt1, PointF pt2)
        {
            return new PointF(pt1.X / pt2.X, pt1.Y / pt2.Y);
        }

        public static PointF operator /(PointF pt1, float value)
        {
            return new PointF(pt1.X / value, pt1.Y / value);
        }


        public static PointF operator /(float value, PointF pt1)
        {
            return new PointF(value / pt1.X, value / pt1.Y);
        }


        public static bool operator ==(PointF left, PointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        /*
        public static bool operator ==(PointF left, ClassicPointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        */
        public static bool operator !=(PointF left, PointF right)
        {
            return !(left.X == right.X && left.Y == right.Y);
        }
        /*
        public static bool operator !=(PointF left, ClassicPointF right)
        {
            return !(left.X == right.X && left.Y == right.Y);
        }*/


       /* public static explicit operator Size(PointF p)
        {
            return new Size(p);
        }*/



        //public static implicit operator PointFF(PointF p);
        public static explicit operator PointF(ClassicPointF p)
        {
            return PointF.FromPointF(p);
        }

        public static explicit operator PointF(RenderPoint p)
        {
            return PointF.FromPoint(p);
        }


        /*


        public static implicit operator ClassicPointF(PointF p)
        {
            return new ClassicPointF(p._x, p._y);
        }
        */
        public bool IsEmpty { get { return this == Empty; } }
        public float X { get { return _x; } set { _x = value; } }
        public float Y { get { return _y; } set { _y = value; } }


        //public static PointF Add(PointF pt, Size sz)
        //public static PointF Ceiling(PointFF value);
        public override bool Equals(object obj)
        {
            if (obj is PointF || obj is ClassicPointF)
            {
                return this == (PointF)obj;
            }
            return false;
        }



        public override int GetHashCode()
        {
            unchecked
            {
                return _x.GetHashCode() ^ _y.GetHashCode();
            }
        }
        //public void Offset(PointF p);

        //public void Offset(int dx, int dy);
        //public static PointF Round(PointFF value);
        //public static PointF Subtract(PointF pt, Size sz);
        public override string ToString()
        {
            return "{X=" + X + ",Y=" + Y + "}";
        }
        //public static PointF Truncate(PointFF value);
    }
}
