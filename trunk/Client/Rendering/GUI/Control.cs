/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/1/2010
 * Time: 3:37 PM
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
using System.Collections.Generic;
using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.InputLib;
using Tortoise.Client.Extension.AgateLib.Geometry;
using Tortoise.Shared.Threading;

namespace Tortoise.Client.Rendering.GUI
{
	/// <summary>
	/// Description of Control.
	/// </summary>
	public abstract class Control
	{
		#region protected Varables
		protected Rectangle _area = new Rectangle();
		protected Color _backgroundColor = Color.White;
		protected Surface _backgroundImage = null;
		protected bool _visible = true;

		//Mark items that have changed so they can be updated next time their renderd.
		/*
		protected bool _changed = false;
		protected bool _chancedLocation = false;
		protected bool _chancedSize = false;
		protected bool _chancedBackgroundColor = false;
		 */
		//protected ControlContainer _parent = null;

		protected bool _inited = false;
		protected bool _loaded = false;
		
		protected Container _parent;
		
		protected FrameBuffer _preRenderd;
		protected bool _redrawPreRenderd = false;
		
		
		protected bool _hasFocus;
		
		protected ThreadSafetyEnforcer _threadSafety;
		protected Invoker _invoker;
		#endregion
		
		#region Event Handlers
		public event EventHandler<MouseEventArgs> MouseDown;
		public event EventHandler<MouseEventArgs> MouseUp;
		public event EventHandler<MouseEventArgs> MouseMove;
		public event EventHandler<MouseEventArgs> KeybordDown;
		public event EventHandler<MouseEventArgs> KeybordUp;
		public event EventHandler<ResizeEventArgs> Resized;
		public event EventHandler<MovedEventArgs> Moved;
		public event EventHandler FocusChanged;
		
		public System.EventHandler TickEvent;

		protected bool doMouseDown(MouseEventArgs e)
		{
			if (MouseDown != null)
				MouseDown(this, e);
			return MouseDown != null;
		}
		protected bool doMouseUp(MouseEventArgs e)
		{
			if (MouseUp != null)
				MouseUp(this, e);
			return MouseUp != null;
		}
		protected bool doMouseMove(MouseEventArgs e)
		{
			if (MouseMove != null)
				MouseMove(this, e);
			return MouseMove != null;
		}
		protected bool doKeybordDown(KeyEventArgs e)
		{
			//Keybord events should never be triggerd in the base control event.
			return false;
			//if (KeybordDown != null)
			//    KeybordDown(this, e);
			//return KeybordDown != null;
		}
		protected bool doKeybordUp(KeyEventArgs e)
		{
			//Keybord events should never be triggerd in the base control event.
			return false;
			//if (KeybordUp != null)
			//    KeybordUp(this, e);
			//return KeybordUp != null;
		}
		protected void doResize(ResizeEventArgs e)
		{
			if (Resized != null)
				Resized(this, e);
		}
		protected void doMove(MovedEventArgs e)
		{
			if (Moved != null)
				Moved(this, e);
		}
		
		protected void doFocusChange()
		{
			if(FocusChanged != null)
				FocusChanged(this, EventArgs.Empty);
		}
		#endregion
		
		#region Properties
		public string Name { get; protected set; }
		
		public bool HasFocus
		{
			get{return _hasFocus;}
			set
			{
				_threadSafety.EnforceThreadSafety();
				bool hasChanged = _hasFocus != value;
				_hasFocus = value;
				if(hasChanged && FocusChanged != null)
					FocusChanged(this, EventArgs.Empty);
			}
		}

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				if (_backgroundColor != value)
				{
					_backgroundColor = value;
					//_chancedBackgroundColor = true;
					_redrawPreRenderd = true;
				}

			}
		}
		
		public Surface BackgroundImage
		{
			get{return _backgroundImage;}
			set{_backgroundImage = value;}
		}

		public bool Loaded
		{
			get { return _loaded; }
		}

		public bool Visible
		{
			get { return _visible; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				_visible = value;
			}
		}

		public Container Parent
		{
			get { return _parent; }
            internal set
			{
				_threadSafety.EnforceThreadSafety();
				_parent = value;
			}
		}

		public Rectangle Area
		{
			get { return _area; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				Rectangle OldRec = _area;

				if (_area.X != value.X || _area.Y != value.Y || _area.Width != value.Width || _area.Height != value.Height)
				{
					_area = value;
					_redrawPreRenderd = true;
				}

				if (_area.X != value.X || _area.Y != value.Y )
					doResize(new ResizeEventArgs(OldRec.Size, _area.Size));

				if (_area.Width != value.Width || _area.Height != value.Height)
					doMove(new MovedEventArgs(OldRec.Location, _area.Location));
			}
		}

		public Point Location
		{
			get { return _area.Location; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				if (_area.X != value.X || _area.Y != value.Y)
				{
					Point OldLocation = _area.Location;
					_area.Location = value;
					doMove(new MovedEventArgs(OldLocation, Location));
				}
			}
		}

		public Size Size
		{
			get { return _area.Size; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				if (_area.Width != value.Width || _area.Height != value.Height)
				{

					Size OldSize = Size;
					_area.Size = value;
					doResize(new ResizeEventArgs(OldSize, Size));
					_redrawPreRenderd = true;
				}
			}
		}

		public int X
		{
			get { return _area.X; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				_area.X = value;
			}
		}

		public int Y
		{
			get { return _area.Y; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				_area.Y = value;
			}
		}

		public int Width
		{
			get { return _area.Width; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				if (_area.Width != value)
				{
					Size OldSize = Size;
					_area.Width = value;
					doResize(new ResizeEventArgs(OldSize, Size));
					_redrawPreRenderd = true;
				}
			}
		}

		public int Height
		{
			get { return _area.Height; }
			set
			{
				_threadSafety.EnforceThreadSafety();
				if (_area.Height != value)
				{
					Size OldSize = Size;
					_area.Height = value;
					doResize(new ResizeEventArgs(OldSize, Size));
					_redrawPreRenderd = true;
				}
			}
		}

		public int Top
		{
			get { return _area.Top; }
		}

		public int Left
		{
			get { return _area.Left; }
		}

		public int Right
		{
			get { return _area.Right; }
		}

		public int Bottom
		{
			get { return _area.Bottom; }
		}

		public Point RealLocation
		{
			get
			{
				return (Parent == null ? new Point(0, 0) : Parent.RealLocation).Add(Location);
			}
		}
		#endregion
		
		#region Constructors
		public Control(string name, Point location, Size size)
			: this(name, new Rectangle(location, size))
		{

		}

		public Control(string name, int x, int y, int width, int height)
			: this(name, new Rectangle(x, y, width, height))
		{

		}

		public Control(string name, Rectangle area)
		{
			_threadSafety = new ThreadSafetyEnforcer(name);
			_invoker = new Invoker(_threadSafety);
			_area = area;
			Name = name;
		}
		#endregion

		#region Virtural Methods
		public virtual void Init()
		{
			_inited = true;
		}

		public virtual void Load()
		{
			if(!_inited)
				Init();
			_loaded = true;
			//_changed = true;
			//_chancedLocation = true;
			//_chancedSize = true;
			//_chancedBackgroundColor = true;
			_preRenderd = null;
			_redrawPreRenderd = true;
		}

		public virtual void Unload()
		{
			_loaded = false;
			if (_preRenderd != null)
			{
				_preRenderd.Dispose();
				_preRenderd = null;
			}
		}

		public virtual void Dispose()
		{
			if (Loaded)
				Unload();
		}

        internal virtual void Tick(TickEventArgs e)
		{
			_threadSafety.EnforceThreadSafety();
			
			_invoker.PollInvokes();
			
			if (_redrawPreRenderd || _preRenderd == null)
			{
				Redraw_PreRenderd();
				_redrawPreRenderd = false;
			}
			
			if(TickEvent != null)
				TickEvent(this, EventArgs.Empty);
		}
		
		
		/// <summary>
		/// Renders the control to the screen.
		/// </summary>
		internal virtual void Render()
		{
			_threadSafety.EnforceThreadSafety();
			if (!_visible)
				return;
			//If its not loaded, LOAD IT!!!!
			if (!Loaded)
				Load();
			
			_preRenderd.RenderTarget.Draw(RealLocation);
		}


		/// <summary>
		/// Redraws the PreRenderd Surface, recomended to override this instead of Render.
		/// </summary>
		protected virtual void Redraw_PreRenderd()
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
			Display.EndFrame();
			Display.RenderTarget = previousBuffer;

		}
		
		
		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseDown(MouseEventArgs e)
		{
			_threadSafety.EnforceThreadSafety();
			if(!IsPointOver(e.MousePosition)) return false;
			return doMouseDown(e);
		}
		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseUp(MouseEventArgs e)
		{
			_threadSafety.EnforceThreadSafety();
			if(!IsPointOver(e.MousePosition)) return false;
			return doMouseUp(e);
		}
		/// <summary>
		/// A MouseMove Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseMove(MouseEventArgs e)
		{
			_threadSafety.EnforceThreadSafety();
			if(!IsPointOver(e.MousePosition)) return false;
			return doMouseMove(e);
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnKeyboardDown(KeyEventArgs e)
		{
			_threadSafety.EnforceThreadSafety();
			//Keybord events should never be triggerd in the base control event.
			return false; //doKeybordDown(e);
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnKeyboardUp(KeyEventArgs e)
		{
			_threadSafety.EnforceThreadSafety();
			//Keybord events should never be triggerd in the base control event.
			return false; //doKeybordUp(e);
		}
		#endregion
		
		#region Public Methods
		public void InvokeMethod(System.Action<object> methodToInvoke, object userData)
		{
			_invoker.InvokeMethod(methodToInvoke, userData);
		}
		
		public bool InvokeRequired()
		{
			return _invoker.InvokeRequired();
		}
		#endregion
		
		#region protected Methods
		protected bool IsPointOver(Point pos)
		{
			return Area.Contains(pos);
		}

		#endregion
	}
}
