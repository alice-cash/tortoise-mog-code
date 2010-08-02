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
		#region protected internal Varables
		protected internal Rectangle _area = new Rectangle();
		protected internal Color _backgroundColor = Color.White;
		protected internal bool _visible = true;

		//Mark items that have changed so they can be updated next time their renderd.
		protected internal bool _changed = false;
		protected internal bool _chancedLocation = false;
		protected internal bool _chancedSize = false;
		protected internal bool _chancedBackgroundColor = false;

		//protected internal ControlContainer _parent = null;

		protected internal bool _inited = false;
		protected internal bool _loaded = false;
		
		protected internal Container _parent;
		
		protected internal Surface _preRenderd;
		protected internal bool _redrawPreRenderd = false;
		
		protected internal bool _enforceThreadSafeCalls = false;
		protected internal int _safeThreadID;
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

		protected internal bool doMouseDown(MouseEventArgs e)
		{
			if (MouseDown != null)
				MouseDown(this, e);
			return MouseDown != null;
		}
		protected internal bool doMouseUp(MouseEventArgs e)
		{
			if (MouseUp != null)
				MouseUp(this, e);
			return MouseUp != null;
		}
		protected internal bool doMouseMove(MouseEventArgs e)
		{
			if (MouseMove != null)
				MouseMove(this, e);
			return MouseMove != null;
		}
		protected internal bool doKeybordDown(MouseEventArgs e)
		{
			//Keybord events should never be triggerd in the base control event.
			return false;
			//if (KeybordDown != null)
			//    KeybordDown(this, e);
			//return KeybordDown != null;
		}
		protected internal bool doKeybordUp(MouseEventArgs e)
		{
			//Keybord events should never be triggerd in the base control event.
			return false;
			//if (KeybordUp != null)
			//    KeybordUp(this, e);
			//return KeybordUp != null;
		}
		protected internal void doResize(ResizeEventArgs e)
		{
			if (Resized != null)
				Resized(this, e);
		}
		protected internal void doMove(MovedEventArgs e)
		{
			if (Moved != null)
				Moved(this, e);
		}
		#endregion
		
		#region Properties
		public string Name { get; protected internal set; }

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				EnforceThreadSafty();
				if (_backgroundColor != value)
				{
					_backgroundColor = value;
					_chancedBackgroundColor = true;
					_redrawPreRenderd = true;
				}

			}
		}

		public bool Loaded
		{
			get { return _loaded; }
			set
			{
				EnforceThreadSafty();
				_loaded = value;
			}
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
			protected internal set
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

				if (_area.X != value.X || _area.Y != value.Y)
					_chancedLocation = true;
				if (_area.Width != value.Width || _area.Height != value.Height)
					_chancedSize = true;

				if (_chancedSize || _chancedLocation)
					_area = value;

				if (_chancedSize)
					doResize(new ResizeEventArgs(OldRec.Size, _area.Size));

				if (_chancedLocation)
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
					_chancedLocation = true;
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
					_chancedSize = true;

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
				_chancedLocation = true;
			}
		}

		public int Y
		{
			get { return _area.Y; }
			set
			{
				EnforceThreadSafty();
				_area.Y = value;
				_chancedLocation = true;
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
					_chancedSize = true;
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
					_chancedSize = true;
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
			Loaded = true;
			_changed = true;
			_chancedLocation = true;
			_chancedSize = true;
			_chancedBackgroundColor = true;
			_preRenderd = null;
			_redrawPreRenderd = true;
		}

		public virtual void Unload()
		{
			Loaded = false;
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

		public virtual void Tick(TickEventArgs e)
		{
			EnforceThreadSafty();
			if (_chancedBackgroundColor || _preRenderd == null)
			{
				Redraw_PreRenderd();
				_redrawPreRenderd = false;
				_chancedBackgroundColor = false;
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
		}
		
		
		/// <summary>
		/// Renders the control to the screen.
		/// </summary>
		public virtual void Render()
		{
			EnforceThreadSafty();
			if (!_visible)
				return;
			//If its not loaded, LOAD IT!!!!
			if (!Loaded)
				Load();
			
			_preRenderd.Draw(RealLocation);
		}


		/// <summary>
		/// Redraws the PreRenderd Surface, recomended to override this instead of Render.
		/// </summary>
		protected internal virtual void Redraw_PreRenderd()
		{
			if (_preRenderd != null)
			{
				_preRenderd.Dispose();
				_preRenderd = null;
			}
			
			_preRenderd = new Surface(Size);
			Display.RenderTarget = _preRenderd;
			Display.BeginFrame();
			Display.Clear(_backgroundColor);
			Display.EndFrame();
			Display.RenderTarget = Display.CurrentWindow;

		}
		
		
		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseButtonDown(MouseEventArgs e)
		{
			EnforceThreadSafty();
			return doMouseDown(e);
		}
		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseButtonUp(MouseEventArgs e)
		{
			EnforceThreadSafty();
			return doMouseUp(e);
		}
		/// <summary>
		/// A MouseMove Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnMouseMove(MouseEventArgs e)
		{
			EnforceThreadSafty();
			return doMouseMove(e);
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnKeyboardDown(MouseEventArgs e)
		{
			EnforceThreadSafty();
			//Keybord events should never be triggerd in the base control event.
			return false; //doKeybordDown(e);
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
		internal virtual bool OnKeyboardUp(MouseEventArgs e)
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
		#endregion
		
		#region Internal Protected Methods
		internal protected void EnforceThreadSafty()
		{
			if(!CheckThreadSafty()) throw new InvalidOperationException(string.Format("Crossthread Access to '{0}' is not permited.",Name));
		}
		internal protected bool CheckThreadSafty()
		{
			//If enforce threadsafty is on, we return based on if the thread is the same as the parent,
			//otherwise we just return true because we want whatevers using this to suceed.
			return _enforceThreadSafeCalls ? _safeThreadID == GetManagedThreadId(): true;
		}
		
		internal protected int GetManagedThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}
		#endregion
	}
}
