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
using System.Collections.Generic;
//using System.Drawing;
using Color = System.Drawing.Color;
//using Tortoise.Client.Extension.System.Drawing;
using StormLib.Threading;

using Tortoise.Graphics.Input;

using StormLib.Exceptions;

using Tortoise.Shared.Drawing;




namespace Tortoise.Graphics.Rendering.GUI
{
    /// <summary>
    /// Description of Control.
    /// </summary>
    public abstract class Control: Invokable
    {
        #region protected Variables
        protected Rectangle _area = new Rectangle();
        protected Color _backgroundColor = Color.White;
        protected Surface _backgroundImage = null;
        protected bool _visible = true;

        //Mark items that have changed so they can be updated next time they are rendered.
        /*
        protected bool _changed = false;
        protected bool _chancedLocation = false;
        protected bool _chancedSize = false;
        protected bool _chancedBackgroundColor = false;
         */
        //protected ControlContainer _parent = null;

        protected bool _loaded = false;

        protected Container _parent;

        protected Surface _preRenderdSurface;
        protected bool _redrawPreRenderd = false;

        protected TGraphics _graphics;

        protected bool _hasFocus;

        protected ControlAnchor _anchor;

        protected ThreadSafetyEnforcer _threadSafety;
       // protected Invoker _invoker;
        #endregion

        #region Event Handlers
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> KeyboardDown;
        public event EventHandler<MouseEventArgs> KeyboardUp;
        public event EventHandler<ResizeEventArgs> Resized;
        public event EventHandler<MovedEventArgs> Moved;
        public event EventHandler FocusChanged;

        public System.EventHandler TickEvent;

        protected bool DoMouseDown(MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
            return MouseDown != null;
        }
        protected bool DoMouseUp(MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
            return MouseUp != null;
        }
        protected bool DoMouseMove(MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
            return MouseMove != null;
        }
        protected bool DoKeyboardDown(KeyEventArgs e)
        {
            //Keyboard events should never be triggered in the base control event.
            return false;
            //if (KeyboardDown != null)
            //    KeyboardDown(this, e);
            //return KeyboardDown != null;
        }
        protected bool DoKeyboardUp(KeyEventArgs e)
        {
            //Keyboard events should never be triggered in the base control event.
            return false;
            //if (KeyboardUp != null)
            //    KeyboardUp(this, e);
            //return KeyboardUp != null;
        }
        protected bool DoKeyboardPress(KeyEventArgs e)
        {
            return false;
        }
        public virtual void DoResize(ResizeEventArgs e)
        {
            Resized?.Invoke(this, e);
        }
        protected void DoMove(MovedEventArgs e)
        {
            Moved?.Invoke(this, e);
        }

        protected void DoFocusChange()
        {
            FocusChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public ControlAnchor Anchor {
            get { return _anchor; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _anchor = value;
            }
        }
        public bool HasFocus
        {
            get { return _hasFocus; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                bool hasChanged = _hasFocus != value;
                _hasFocus = value;
                if (hasChanged && FocusChanged != null)
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
            get { return _backgroundImage; }
            set { _backgroundImage = value; }
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

                if (_area.X != value.X || _area.Y != value.Y)
                    DoResize(new ResizeEventArgs(OldRec.Size, _area.Size));

                if (_area.Width != value.Width || _area.Height != value.Height)
                    DoMove(new MovedEventArgs(OldRec.Location, _area.Location));
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
                    Point OldLocation =_area.Location;
                    _area.Location = value;
                    DoMove(new MovedEventArgs(OldLocation, _area.Location));
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
                    DoResize(new ResizeEventArgs(OldSize, Size));
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
                _redrawPreRenderd = true;
            }
        }

        public int Y
        {
            get { return _area.Y; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _area.Y = value;
                _redrawPreRenderd = true;
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
                    DoResize(new ResizeEventArgs(OldSize, Size));
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
                    DoResize(new ResizeEventArgs(OldSize, Size));
                    _redrawPreRenderd = true;
                }
            }
        }

        public int Top
        {
            get { return Y; }
            set { Y = value; }
        }

        public int Left
        {
            get { return X; }
            set { X = value; }
        }

        public int Right
        {
            get { return X + Width; }
            set { Width = value - X; }
        }

        public int Bottom
        {
            get { return Y + Height; }
            set { Height = value - Y; }
        }

        public Point RealLocation
        {
            get
            {
                return (Parent == null ? new Point(0, 0) : Parent.RealLocation) + (Location);
            }
        }
        #endregion

        #region Constructors
        public Control(TGraphics graphics, string name, Point location, Size size)
            : this(graphics, name, new Rectangle(location, size))
        {

        }

        public Control(TGraphics graphics, string name, int x, int y, int width, int height)
            : this(graphics, name, new Rectangle(x, y, width, height))
        {

        }

        public Control(TGraphics graphics, string name, Rectangle area)
        {
            _graphics = graphics;
            _threadSafety = new ThreadSafetyEnforcer(name);
            _invoker = new Invoker(_threadSafety);
            _area = area;
            _anchor = ControlAnchor.Default;
            Name = name;
        }
        #endregion

        #region Virtual Methods


        public virtual void Load()
        {

            _loaded = true;
            //_changed = true;
            //_chancedLocation = true;
            //_chancedSize = true;
            //_chancedBackgroundColor = true;

            _preRenderdSurface = Surface.CreateBlankSurface(_graphics, Width, Height);

            _redrawPreRenderd = true;
        }

        public virtual void Unload()
        {
            _loaded = false;
            if (_preRenderdSurface != null)
            {
                _preRenderdSurface.Dispose();
                _preRenderdSurface = null;
            }
        }

        public virtual void Dispose()
        {
            if (Loaded)
                Unload();
        }

        public virtual void Tick(TickEventArgs e)
        {
            if (!Loaded) throw new LogicException("Control not loaded!");

            _threadSafety.EnforceThreadSafety();

            _invoker.PollInvokes();

            if (_redrawPreRenderd)
            {
                Redraw_PreRenderd();
                _redrawPreRenderd = false;
            }

            TickEvent?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Renders the control to the screen. You should 
        /// </summary>
        public virtual void Render()
        {
            _threadSafety.EnforceThreadSafety();

            if (!Loaded) throw new LogicException("Control not loaded!");

            if (!_visible)
                return;
            //If its not loaded, Throw a freaking error!


            _preRenderdSurface.Render(RealLocation);
        }

        /// <summary>
        /// Redraws the PreRenderd Surface.
        /// </summary>
        [ThreadSafeAttribute(ThreadSafeFlags.ThreadSafeEnforced)]
        public virtual void Redraw_PreRenderd()
        {

            _threadSafety.EnforceThreadSafety();

            if (!Loaded) throw new LogicException("Control not loaded!");

            /*
            if (_preRenderdSurface != null)
            {
                _preRenderdSurface.Dispose();
                _preRenderdSurface = null;
            } */
/*
            if (FillBackground)
            {
                _preRenderdSurface.BeginChanges();
                _preRenderdSurface.Fill(_backgroundColor);
                _preRenderdSurface.EndChanges();
            }*/

        }


        /// <summary>
        /// A MouseButton Event, returns true if the event is used, and false if it isn't.
        /// </summary>
        public virtual bool OnMouseDown(MouseEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            if (!IsPointOver(e.MouseData.Position)) return false;
            return DoMouseDown(e);
        }
        /// <summary>
        /// A MouseButton Event, returns true if the event is used, and false if it isn't.
        /// </summary>
        public virtual bool OnMouseUp(MouseEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            if (!IsPointOver(e.MouseData.Position)) return false;
            return DoMouseUp(e);
        }
        /// <summary>
        /// A MouseMove Event, returns true if the event is used, and false if it isn't.
        /// </summary>
        public virtual bool OnMouseMove(MouseEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            if (!IsPointOver(e.MouseData.Position)) return false;
            return DoMouseMove(e);
        }

        /// <summary>
        /// A Keyboard Event, returns true if the event is used, and false if it isn't.
        /// </summary>
        public virtual bool OnKeyboardPress(KeyEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            //Keyboard events should never be triggered in the base control event.
            return false; //doKeyboardDown(e);
        }

        /// <summary>
        /// A Keyboard Event, returns true if the event is used, and false if it isn't.
        /// </summary>
        public virtual bool OnKeyboardDown(KeyEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            //Keyboard events should never be triggered in the base control event.
            return false; //doKeyboardDown(e);
        }
        /// <summary>
        /// A Keyboard Event, returns true if the event is used, and false if it isn't.
        /// </summary>
        public virtual bool OnKeyboardUp(KeyEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            //Keyboard events should never be triggered in the base control event.
            return false; //doKeyboardUp(e);
        }
        #endregion

        #region protected Methods
        protected bool IsPointOver(System.Drawing.Point pos)
        {
            return Area.Contains(Point.FromPoint(pos));
        }

        protected bool IsPointOver(Point pos)
        {
            return Area.Contains(pos);
        }

        [Obsolete]
        protected bool IsPointOver(System.Drawing.PointF pos)
        {
            return Area.Contains((int)pos.X, (int)pos.Y);
        }


        #endregion

        #region Methods
        public void ContainerResized(ResizeEventArgs e)
        {
            if ((_anchor & (ControlAnchor.Left | ControlAnchor.Right)) == 0)
            {
                X = X * e.NewSize.Width / e.OldSize.Width;
            } else
            {
                //Nothing needs to be done if the left ancher is set
                //if ((_anchor & ControlAnchor.Left) != 0)
                //{
                //
                //}
                if ((_anchor & ControlAnchor.Right) != 0)
                {
                    Right += e.NewSize.Width - e.OldSize.Width;
                }
            }


            if ((_anchor & (ControlAnchor.Top | ControlAnchor.Bottom)) == 0)
            {
                Y = Y * e.NewSize.Height / e.OldSize.Height;
            } else
            {
                //Nothing needs to be done if the top ancher is set

                if ((_anchor & ControlAnchor.Bottom) != 0)
                {
                    Bottom += e.NewSize.Height - e.OldSize.Height;
                }
            }
        }

        #endregion
    }

}
