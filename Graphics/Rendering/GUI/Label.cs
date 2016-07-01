/*
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */

using System;
using System.Runtime.CompilerServices;
//using System.Drawing;
using Tortoise.Graphics.Input;


using Tortoise.Shared.Exceptions;
using Tortoise.Shared.Drawing;
using Color = System.Drawing.Color;
using XColor = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using MonoGame.Extended.BitmapFonts;

namespace Tortoise.Graphics.Rendering.GUI
{
    /// <summary>
    /// ButtonBase is the base for any control which has text, and an acton is performed when it is clicked.
    /// </summary>
    public class Label : Control
    {
        protected string _text;
        protected FontInfo _fontInfo;
        protected TextAlignement _align;
        protected Color _textColor;
        //protected GorgonText _gorgonText;

        public TextAlignement TextAlignement
        {
            get { return _align; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _align = value;
                _redrawPreRenderd = true;
            }
        }
        //private bool _textChanged;
        public string Text
        {
            get { return _text; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _text = value;
                //_textChanged = true;
                _redrawPreRenderd = true;
            }

        }

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _textColor = value;
                //_textChanged = true;
                _redrawPreRenderd = true;
            }

        }



        public Label(TGraphics graphics, string name, string text, int x, int y, int width, int height)
            : this(graphics, name, text, new Rectangle(x, y, width, height), FontInfo.GetInstance(graphics, 22, FontTypes.Sans))
        {

        }
        public Label(TGraphics graphics, string name, string text, Point location, Size size)
            : this(graphics, name, text, new Rectangle(location, size), FontInfo.GetInstance(graphics, 22, FontTypes.Sans))
        {

        }
        public Label(TGraphics graphics, string name, string text, int x, int y, int width, int height, FontInfo fontInfo)
            : this(graphics, name, text, new Rectangle(x, y, width, height), fontInfo)
        {

        }
        public Label(TGraphics graphics, string name, string text, Point location, Size size, FontInfo fontInfo)
            : this(graphics, name, text, new Rectangle(location, size), fontInfo)
        {

        }
        public Label(TGraphics graphics, string name, string text, Rectangle area, FontInfo fontInfo)
            : base(graphics, name, area)
        {
            Text = text;
            _fontInfo = fontInfo;
            //_gorgonText = _graphics.Renderer2D.Renderables.CreateText(name + "_label", fontInfo.GFont, text);
        }

        int test = 0;
        protected override void Redraw_PreRenderd()
        {
            //Required for thread enforcement 
            base.Redraw_PreRenderd();



            _preRenderdSurface.BeginChanges();

            _preRenderdSurface.Fill(Color.Transparent);

            _graphics.SpriteBatch.DrawString(_fontInfo.Bitmap, _text, new Vector2(0, 0), XColor.Black);

            _preRenderdSurface.EndChanges();



            return;
/*
            //Required for thread enforcement 
            base.Redraw_PreRenderd();

            _preRenderdSurface.BeginChanges();

            _graphics.Renderer2D.Clear(Color.White);

            _preRenderdSurface.Fill(Color.White);

            _gorgonText.Text = _text;
            _gorgonText.Color = Color.Black;
            _gorgonText.Position = new SlimMath.Vector2(0, 0);


            if (_backgroundImage != null)
                _preRenderdSurface.Blit(_backgroundImage);

            _gorgonText.Draw();

            _graphics.Renderer2D.Render();

            _preRenderdSurface.EndChanges();


            _preRenderdSurface.Save(@"C:\testing\debug" + (test++).ToString() + ".png");


    */



            // if (_backgroundColor == Color.Transparent)
            //    _preRenderdSurface = Window.Instance.GenerateTransparentSurface(Size);
            //else


            //_preRenderdSurface.Fill(Color.Red);



        }

        //protected Point TextDestination()
        //{
        //    Size textSize = _fontInfo.Font.SizeText(_text);
        //    Point position = new Point(0,0);
        //    if(TextAlignement == TextAlignement.Center || TextAlignement == TextAlignement.Top || TextAlignement == TextAlignement.Bottom)
        //    {
        //        position.X = (Size.Width / 2) - (textSize.Width / 2);
        //    }

        //    if(TextAlignement == TextAlignement.Center || TextAlignement == TextAlignement.Left || TextAlignement == TextAlignement.Right)
        //    {
        //        position.Y = (Size.Height / 2) - (textSize.Height / 2);
        //    }

        //    if(TextAlignement == TextAlignement.BottomLeft || TextAlignement == TextAlignement.Bottom || TextAlignement == TextAlignement.BottomRight)
        //    {
        //        position.Y = Size.Height - textSize.Height;
        //    }

        //    if(TextAlignement == TextAlignement.TopLeft ||  TextAlignement == TextAlignement.Left || TextAlignement == TextAlignement.BottomLeft)
        //    {
        //        position.X = Size.Width - textSize.Width;
        //    }
        //    return position;
        //}


        internal void ForceUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
