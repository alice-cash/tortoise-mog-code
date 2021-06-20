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
using System.Drawing;
using GorgonLibrary.Renderers;
using GorgonLibrary.Graphics;

using Tortoise.Shared.Exceptions;

namespace Tortoise.Client.Rendering.GUI
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
        protected GorgonText _gorgonText;

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



        public Label(string name, string text, int x, int y, int width, int height)
            : this(name, text, new Rectangle(x, y, width, height), FontInfo.GetInstance(12, FontTypes.Sans))
        {

        }
        public Label(string name, string text, Point location, Size size)
            : this(name, text, new Rectangle(location, size), FontInfo.GetInstance(12, FontTypes.Sans))
        {

        }
        public Label(string name, string text, int x, int y, int width, int height, FontInfo fontInfo)
            : this(name, text, new Rectangle(x, y, width, height), fontInfo)
        {

        }
        public Label(string name, string text, Point location, Size size, FontInfo fontInfo)
            : this(name, text, new Rectangle(location, size), fontInfo)
        {

        }
        public Label(string name, string text, Rectangle area, FontInfo fontInfo)
            : base(name, area)
        {
            Text = text;
            _fontInfo = fontInfo;
            _gorgonText = Program.GameLogic.Renderer2D.Renderables.CreateText(name + "_label", fontInfo.GFont, text);
        }

        int test = 0;
        protected override void Redraw_PreRenderd()
        {
            //Required for thread enforcement 
            base.Redraw_PreRenderd();



            _preRenderdSurface.BeginChanges();

            Program.GameLogic.Renderer2D.Clear(Color.Transparent);

            _gorgonText.Text = _text;
            _gorgonText.Color = Color.Black;
            _gorgonText.Position = new SlimMath.Vector2(0, 0);

            _gorgonText.Draw();



            _preRenderdSurface.EndChanges();



            return;

            //Required for thread enforcement 
            base.Redraw_PreRenderd();

            _preRenderdSurface.BeginChanges();

            Program.GameLogic.Renderer2D.Clear(Color.White);

            _preRenderdSurface.Fill(Color.White);

            _gorgonText.Text = _text;
            _gorgonText.Color = Color.Black;
            _gorgonText.Position = new SlimMath.Vector2(0, 0);


            if (_backgroundImage != null)
                _preRenderdSurface.Blit(_backgroundImage);

            _gorgonText.Draw();

            Program.GameLogic.Renderer2D.Render();

            _preRenderdSurface.EndChanges();


            _preRenderdSurface.Save(@"C:\testing\debug" + (test++).ToString() + ".png");






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
