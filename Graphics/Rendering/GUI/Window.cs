using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;

using Color = System.Drawing.Color;

using Tortoise.Shared.Drawing;
using Tortoise.Graphics.Input;

namespace Tortoise.Graphics.Rendering.GUI
{
    public class Window : Container
    {

        private bool _isMouseMoved;

        public Window(TGraphics graphics, string name, string text, int x, int y, int width, int height)
            : this(graphics, name, text, new Rectangle(x, y, width, height))
        {
        }
        public Window(TGraphics graphics, string name, string text, Point location, Size size)
            : this(graphics, name, text, new Rectangle(location, size))
        {

        }

        public Window(TGraphics graphics, string name, string text, Rectangle area)
            : base(graphics, name, area)
        {
            BackgroundColor = Color.White;
        }


        internal override bool OnMouseDown(MouseEventArgs e)
        {
            int Y = (e.MouseData.Position - RealLocation).Y;


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

            Primitives.Box.DrawBox(_graphics, new Rectangle(0, 0, Width, 25), Color.Gray);
        }
    }
}
