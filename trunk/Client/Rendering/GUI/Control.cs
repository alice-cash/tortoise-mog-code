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
using Thread = System.Threading.Thread;

namespace Tortoise.Client.Rendering.GUI
{
	/// <summary>
	/// Description of Control.
	/// </summary>
	public abstract class Control
	{
		#region internal Varables
		internal Rectangle _area = new Rectangle();
		internal Color _backgroundColor = Color.White;
		internal bool _visible = true;

		//Mark items that have changed so they can be updated next time their renderd.
		/*
		internal bool _changed = false;
		internal bool _chancedLocation = false;
		internal bool _chancedSize = false;
		internal bool _chancedBackgroundColor = false;
		 */
		//internal ControlContainer _parent = null;

		internal bool _inited = false;
		internal bool _loaded = false;
		
		internal Container _parent;
		
		internal FrameBuffer _preRenderd;
		internal bool _redrawPreRenderd = false;
		
		internal bool _enforceThreadSafeCalls = false;
		internal int _safeThreadID;
		
		internal bool _hasFocus;
		
		private Queue<InvokeItem> _invokeList;
		#endregion
		
		#region Event Handlers
		public EventHandler<MouseEventArgs> MouseDown;
		public EventHandler<MouseEventArgs> MouseUp;
		public EventHandler<MouseEventArgs> MouseMove;
		public EventHandler<MouseEventArgs> KeybordDown;
		public EventHandler<MouseEventArgs> KeybordUp;
		public EventHandler<ResizeEventArgs> Resized;
		public EventHandler<MovedEventArgs> Moved;
		public EventHandler FocusChanged;
		
		public System.EventHandler TickEvent;

		internal bool doMouseDown(MouseEventArgs e)
		{
			if (MouseDown != null)
				MouseDown(this, e);
			return MouseDown != null;
		}
		internal bool doMouseUp(MouseEventArgs e)
		{
			if (MouseUp != null)
				MouseUp(this, e);
			return MouseUp != null;
		}
		internal bool doMouseMove(MouseEventArgs e)
		{
			if (MouseMove != null)
				MouseMove(this, e);
			return MouseMove != null;
		}
		internal bool doKeybordDown(KeyEventArgs e)
		{
			//Keybord events should never be triggerd in the base control event.
			return false;
			//if (KeybordDown != null)
			//    KeybordDown(this, e);
			//return KeybordDown != null;
		}
		internal bool doKeybordUp(KeyEventArgs e)
		{
			//Keybord events should never be triggerd in the base control event.
			return false;
			//if (KeybordUp != null)
			//    KeybordUp(this, e);
			//return KeybordUp != null;
		}
		internal void doResize(ResizeEventArgs e)
		{
			if (Resized != null)
				Resized(this, e);
		}
		internal void doMove(MovedEventArgs e)
		{
			if (Moved != null)
				Moved(this, e);
		}
		#endregion
		
		#region Properties
		public string Name { get; internal set; }
		
		public bool HasFocus
		{
			get{return _hasFocus;}
			set
			{
				EnforceThreadSafty();
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
				EnforceThreadSafty();
				if (_backgroundColor != value)
				{
					_backgroundColor = value;
					//_chancedBackgroundColor = true;
					_redrawPreRenderd = true;
				}

			}
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
				EnforceThreadSafty();
				_visible = value;
			}
		}

		public Container Parent
		{
			get { return _parent; }
			internal set
			{
				EnforceThreadSafty();
				_parent = value;
			}
		}

		public Rectangle Area
		{
			get { return _area; }
			set
			{
				EnforceThreadSafty();
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
				EnforceThreadSafty();
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
				EnforceThreadSafty();
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
				EnforceThreadSafty();
				_area.X = value;
			}
		}

		public int Y
		{
			get { return _area.Y; }
			set
			{
				EnforceThreadSafty();
				_area.Y = value;
			}
		}

		public int Width
		{
			get { return _area.Width; }
			set
			{
				EnforceThreadSafty();
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
				EnforceThreadSafty();
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
			_area = area;
			Name = name;
			_safeThreadID = GetManagedThreadId();
			_invokeList = new Queue<InvokeItem>();
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
			EnforceThreadSafty();
			if (_redrawPreRenderd || _preRenderd == null)
			{
				Redraw_PreRenderd();
				_redrawPreRenderd = false;
			}
			lock(_invokeList)
			{
				InvokeItem item;
				while(_invokeList.Count > 0)
				{
					item = _invokeList.Dequeue();
					item.Action(item.UserData);
				}
				
			}
			if(TickEvent != null)
				TickEvent(this, EventArgs.Empty);
		}
		
		
		/// <summary>
		/// Renders the control to the screen.
		/// </summary>
		internal virtual void Render()
		{
			EnforceThreadSafty();
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
		internal virtual void Redraw_PreRenderd()
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
			Display.EndFrame();
			Display.RenderTarget = Window.MainWindow.FrameBuffer;

		}
		
		
		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseDown(MouseEventArgs e)
		{
			EnforceThreadSafty();
			if(!IsPointOver(e.MousePosition)) return false;
			return doMouseDown(e);
		}
		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseUp(MouseEventArgs e)
		{
			EnforceThreadSafty();
			if(!IsPointOver(e.MousePosition)) return false;
			return doMouseUp(e);
		}
		/// <summary>
		/// A MouseMove Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseMove(MouseEventArgs e)
		{
			EnforceThreadSafty();
			if(!IsPointOver(e.MousePosition)) return false;
			return doMouseMove(e);
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnKeyboardDown(KeyEventArgs e)
		{
			EnforceThreadSafty();
			//Keybord events should never be triggerd in the base control event.
			return false; //doKeybordDown(e);
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnKeyboardUp(KeyEventArgs e)
		{
			EnforceThreadSafty();
			//Keybord events should never be triggerd in the base control event.
			return false; //doKeybordUp(e);
		}
		#endregion
		
		#region Public Methods
		public void InvokeMethod(InvokeDelegate methodToInvoke, object userData)
		{
			if(CheckThreadSafty())
			{
				//if its true we just run it.
				methodToInvoke.Invoke(userData);
				return;
			}
			//Othherwise we lock it and add it.
			lock(_invokeList)
			{
				_invokeList.Enqueue(new InvokeItem(methodToInvoke, userData));
			}
		}
		
		public bool InvokeRequired()
		{
			return CheckThreadSafty();
		}
		#endregion
		
		#region internal Methods
		internal bool IsPointOver(Point pos)
		{
			return Area.Contains(pos);
		}
		internal void EnforceThreadSafty()
		{
			if(!CheckThreadSafty()) throw new InvalidOperationException(string.Format("Crossthread Access to '{0}' is not permited.",Name));
		}
		internal bool CheckThreadSafty()
		{
			//If enforce threadsafty is on, we return based on if the thread is the same as the parent,
			//otherwise we just return true because we want whatevers using this to suceed.
			return _enforceThreadSafeCalls ? _safeThreadID == GetManagedThreadId(): true;
		}
		
		internal int GetManagedThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}
		#endregion
	}
}
