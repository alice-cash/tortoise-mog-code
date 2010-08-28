/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/1/2010
 * Time: 10:28 PM
 * 
 * Copyright 2010 Matthew Cash. All rights reserved.
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
using AgateLib.DisplayLib;
using AgateLib.DisplayLib.ImplementationBase;
using AgateLib.Geometry;
using AgateLib.Resources;

namespace Tortoise.Client.Rendering.GUI
{
	/// <summary>
	/// ButtonBase is the base for any control which has text, and an acton is performed when it is clicked.
	/// </summary>
	public class Label : Control
	{
        protected string _text;
		protected FontSurface _fontSurface;
        protected TextAlignement _align;
		
		
		public TextAlignement TextAlignement
		{
			get{return _align;}
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
			get{return _text;}
			set
			{
				_threadSafety.EnforceThreadSafety();
				_text = value;
				//_textChanged = true;
				_redrawPreRenderd = true;
			}
			
		}

		

		public Label(string name, string text, int x, int y, int width, int height)
			: this(name, text, new Rectangle(x, y, width, height), FontSurface.AgateSans10)
		{

		}
		public Label(string name, string text, Point location, Size size)
			: this(name, text, new Rectangle(location, size), FontSurface.AgateSans10)
		{

		}
		public Label(string name, string text, int x, int y, int width, int height, FontSurface fontsurface)
			: this(name, text, new Rectangle(x, y, width, height), fontsurface)
		{

		}
		public Label(string name, string text, Point location, Size size, FontSurface fontsurface)
			: this(name, text, new Rectangle(location, size), fontsurface)
		{

		}
		public Label(string name, string text, Rectangle area, FontSurface fontsurface)
			: base(name, area)
		{
			Text = text;
			_fontSurface = fontsurface;
		}
		
		protected override void Redraw_PreRenderd()
		{
			if (_preRenderd != null)
			{
				_preRenderd.Dispose();
				_preRenderd = null;
			}


            FrameBuffer previousBuffer = Display.RenderTarget;
            _preRenderd = new FrameBuffer(Size);
			Display.RenderTarget = _preRenderd;
			Display.BeginFrame();

			if(_backgroundColor != Color.Transparent)
				Display.Clear(_backgroundColor);
			if(_backgroundImage != null)
				_backgroundImage.Draw();
			_fontSurface.Color = Color.Black;
			_fontSurface.DrawText(TextDestination(), _text);
			
			Display.EndFrame();
			Display.FlushDrawBuffer();
            Display.RenderTarget = previousBuffer;
		}
		
		protected Point TextDestination()
		{
			Size textSize = _fontSurface.MeasureString(_text);
			Point position = new Point(0,0);
			if(TextAlignement == TextAlignement.Center || TextAlignement == TextAlignement.Top || TextAlignement == TextAlignement.Bottom)
			{
				position.X = (Size.Width / 2) - (textSize.Width / 2);
			}

			if(TextAlignement == TextAlignement.Center || TextAlignement == TextAlignement.Left || TextAlignement == TextAlignement.Right)
			{
				position.Y = (Size.Height / 2) - (textSize.Height / 2);
			}
			
			if(TextAlignement == TextAlignement.BottomLeft || TextAlignement == TextAlignement.Bottom || TextAlignement == TextAlignement.BottomRight)
			{
				position.Y = Size.Height - textSize.Height;
			}

			if(TextAlignement == TextAlignement.TopLeft ||  TextAlignement == TextAlignement.Left || TextAlignement == TextAlignement.BottomLeft)
			{
				position.X = Size.Width - textSize.Width;
			}
			return position;
		}

	}
}
