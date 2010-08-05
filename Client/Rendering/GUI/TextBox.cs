/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/3/2010
 * Time: 8:24 PM
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
using System.Text;

using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;

using Tortoise.Client.Extension.AgateLib.Geometry;

namespace Tortoise.Client.Rendering.GUI
{
	/// <summary>
	/// Desctiption of TextBox.
	/// </summary>
	public class TextBox : Control
	{
		private string _text = "";
		private string _visibleText = "";
		private Size _textSize;
		private int _markerPosition = 0;
		private FontSurface _fontSurface;
		private char _passwordChar = '*';
		private bool _usePasswordChar = false;
		private int _cursorPosition;
		private Timing.StopWatch _flasherTimer;
		private bool _showMarker;
		
		public bool UsePasswordCharacter
		{
			get{return _usePasswordChar;}
			set
			{
				EnforceThreadSafty();
				_usePasswordChar = value;
			}
		}
		
		public char PasswordCharacter
		{
			get{return _passwordChar;}
			set
			{
				EnforceThreadSafty();
				_passwordChar = value;
			}
		}
		
		public int CursorPosition
		{
			get{return _cursorPosition;}
			set
			{
				EnforceThreadSafty();
				if(value < 0 || value > Text.Length)
					throw new ArgumentException("CursorPosition must be between 0 and Text.Length");
				_cursorPosition = value;
			}
		}
		
		public string Text
		{
			get{return _text;}
			set
			{
				EnforceThreadSafty();
				_text = value;
				//_textChanged = true;
				_redrawPreRenderd = true;
			}
			
		}
		

		public TextBox(string name, int x, int y, int width, int height)
			: this(name, new Rectangle(x, y, width, height))
		{

		}
		public TextBox(string name, Point location, Size size)
			: this(name, new Rectangle(location, size))
		{

		}
		public TextBox(string name, Rectangle area)
			: base(name, area)
		{
			_fontSurface = FontSurface.AgateSans10;
			_flasherTimer = new Timing.StopWatch();
		}
		
		
		protected internal override void Tick(TickEventArgs e)
		{
			_visibleText = Text;
			if (_usePasswordChar)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < _visibleText.Length; i++)
					sb.Append(_passwordChar);

				_visibleText = sb.ToString();
			}

			_textSize = _fontSurface.MeasureString(_visibleText);
			
			
			if (HasFocus)
			{
				if (_flasherTimer.TotalMilliseconds > 500)
				{
					_flasherTimer.Reset();
					_showMarker = !_showMarker;
				}
			}
			/*
 			//Rectangle rSize = TransferInfo.FontDraw.MeasureString(null, tText, DrawTextFormat.None,Color.White);
			VisualPart = tText;
			SubSelPos = SelPos;

			
            if (rSize.Width >= tWidth)
            //start choppin it up
            {

                while (TransferInfo.FontDraw.MeasureString(null, VisualPart.Substring(SubSelPos), DrawTextFormat.None, Color.White).Width > tWidth)
                {
                    VisualPart = VisualPart.Remove(0, 1);
                    SubSelPos--;
                }




            }*/
			base.Tick(e);
		}
		
		internal override bool OnMouseUp(MouseEventArgs e)
		{
			EnforceThreadSafty();
			if(IsPointOver(e.MousePosition))
			{
				this.HasFocus = true;
				doMouseDown(e);
				return true;
			}
			return false;
		}
		
		internal override bool OnKeyboardDown(MouseEventArgs e)
		{
			if(!HasFocus) return false;
			return doKeybordDown(e);
		}
		
		internal override bool OnKeyboardUp(MouseEventArgs e)
		{
			if(!HasFocus) return false;
			//we base all work on the up part.
			

			doKeybordDown(e);
			return true;
		}
		
		protected internal override void Render()
		{
			if(!_visible)
				return;
			base.Render();

			if (_showMarker && HasFocus)
			{
				Point drawPos = GetDrawPosition();
				drawPos.X += _markerPosition;
				drawPos = drawPos.Add(RealLocation);
				_fontSurface.DrawText(drawPos, "|");
			}
		}
		
		protected internal override void Redraw_PreRenderd()
		{
			if (_preRenderd != null)
			{
				_preRenderd.Dispose();
				_preRenderd = null;
			}
			
			_preRenderd = new FrameBuffer(Size);
			Display.RenderTarget = _preRenderd;
			Display.BeginFrame();

			if(_backgroundColor != Color.Transparent)
				Display.Clear(_backgroundColor);
			_fontSurface.Color = Color.Black;
			_fontSurface.DrawText(GetDrawPosition(), _visibleText);
			
			Point[] linePoints = new Point[5];
			linePoints[0] = new Point(0, 0);
			linePoints[1] = new Point(0, Height - 2);
			linePoints[2] = new Point(Width - 2, Height - 2);
			linePoints[3] = new Point(Width - 2, 0);
			linePoints[0] = new Point(0, 0);
			
			Display.DrawLines(linePoints, Color.Gray);
			
			linePoints[0] = new Point(1, 1);
			linePoints[1] = new Point(1, Height - 1);
			linePoints[2] = new Point(Width - 1, Height - 1);
			linePoints[3] = new Point(Width - 1, 1);
			linePoints[0] = new Point(1, 1);

			Display.DrawLines(linePoints, Color.Black);
			
			Display.EndFrame();
			Display.FlushDrawBuffer();
			Display.RenderTarget = Window.MainWindow.FrameBuffer;
		}
		private Point GetDrawPosition()
		{
			return new Point(5,(Size.Height / 2) - (_textSize.Height / 2));
		}
	}
	

}