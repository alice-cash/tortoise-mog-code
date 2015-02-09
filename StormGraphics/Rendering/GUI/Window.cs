using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;



namespace Tortoise.Graphics.Rendering.GUI
{
    public class Window : Container
    {

        private bool _isMouseMoved;

        public Window(string name, string text, int x, int y, int width, int height)
            : this(name, text, new Rectangle(x, y, width, height))
        {
        }
        public Window(string name, string text, Point location, Size size)
            : this(name, text, new Rectangle(location, size))
        {

        }

        public Window(string name, string text, Rectangle area)
            : base(name, area)
        {
            BackgroundColor = Color.White;
        }


        internal override bool OnMouseDown(MouseEventArgs e)
        {
            int Y = (e.Position - RealLocation).Y;


            return base.OnMouseDown(e);
        }

        internal override bool OnMouseMove(MouseEventArgs e)
        {
            return base.OnMouseMove(e);
        }

        internal override bool OnMouseUp(MouseEventArgs e)
        {
            return base.OnMouseUp(e);
        }



        protected override void Redraw_PreRenderd()
        {
            base.Redraw_PreRenderd();

            _preRenderdSurface.BeginChanges();

            Program.GameLogic.Renderer2D.Drawing.DrawRectangle(new RectangleF(0, 0, Width, 25), Color.Gray);


        }
    }
}
