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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
namespace Tortoise.Graphics.Rendering.GUI
{
	public class Container : Control
	{
		protected ControlContainerManager _Items;
		private bool _inFocusChange = false;

		public ControlContainerManager Controls
		{
			get { return _Items; }
			set { _Items = value; }
		}
		
		/*
		public Control this[string name]
		{
			get{
				return _Items[name];
			}
		}
		
		public Control this[int index]
		{
			get{
				return _Items[index];
			}
		}
		 */

		public Container(string name, int x, int y, int width, int height)
			: this(name, new Rectangle(x, y, width, height))
		{

		}
		public Container(string name, Point location, Size size)
			: this(name, new Rectangle(location, size))
		{

		}
		public Container(string name, Rectangle area)
			: base(name, area)
		{
			Controls = new ControlContainerManager();
		}


		public override void Initialize()
		{
            if (_inited) return;
			base.Initialize();
			_Items = new ControlContainerManager();

		}

		public override void Load()
		{
			base.Load();
			foreach (Control Item in _Items.Values)
				if (!Item.Loaded)
					Item.Load();
		}

		public override void Unload()
		{
			base.Unload();
			_loaded = false;
			foreach (Control Item in _Items.Values)
				if (Item.Loaded)
					Item.Load();
		}

        internal override void Tick(TickEventArgs e)
		{
			foreach (var Item in Controls)
			{
				if (Item.Value.Parent != this)
				{
					Item.Value.Parent = this;

					Item.Value.FocusChanged += new EventHandler(Item_Value_FocusChanged);
				}
				Item.Value.Tick(e);
			}
			base.Tick(e);
		}

		void Item_Value_FocusChanged(object sender, EventArgs e)
		{
			//We need to make sure all other controls within this one
			//notify us when they gain focus, so we can tell them and our
			//parent control to remove it from anything else.
			Control item = sender as Control;
			if(item == null) return;
			
			if(!_inFocusChange && item.HasFocus)
			{
				_inFocusChange = true;
				foreach(var c in _Items)
				{
					if(c.Value == item) continue;
					c.Value.HasFocus = false;
				}
				_inFocusChange = false;
			}
			doFocusChange();
		}
		


		/// <summary>
		/// Renders the control to the screen.
		/// </summary>
        internal override void Render()
		{
            base.Render();

			foreach (Control Item in _Items.Values)
			{
                Item.Render();
			}

		}

		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
        internal override bool OnMouseDown(MouseEventArgs e)
		{
			bool go = false;
			foreach (Control Item in _Items.Values.Reverse())
			{
				go = Item.OnMouseDown(e);
				if (go)
					break;
			}
			return go ? true : doMouseDown(e);
		}
		/// <summary>
		/// A MouseButton Event, returns true if the event is used, and false if it isn't.
		/// </summary>
        internal override bool OnMouseUp(MouseEventArgs e)
		{

			bool go = false;
			foreach (Control Item in _Items.Values.Reverse())
			{
				go = Item.OnMouseUp(e);
				if (go)
					break;
			}
			return go ? true : doMouseUp(e);
		}
		/// <summary>
		/// A MouseMove Event, returns true if the event is used, and false if it isn't.
		/// </summary>
        internal override bool OnMouseMove(MouseEventArgs e)
		{
			bool go = false;
			foreach (Control Item in _Items.Values.Reverse())
			{
				go = Item.OnMouseMove(e);
				if (go)
					break;
			}
			return go ? true : doMouseMove(e);
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
        internal override bool OnKeyboardDown(KeyEventArgs e)
		{
			bool go = false;
			foreach (Control Item in _Items.Values.Reverse())
			{
				go = Item.OnKeyboardDown(e);
				if (go)
					break;
			}
			//Keyboard events should never be triggered in the base container event.
			return go;
		}
		/// <summary>
		/// A Keyboard Event, returns true if the event is used, and false if it isn't.
		/// </summary>
        internal override bool OnKeyboardUp(KeyEventArgs e)
		{
			bool go = false;
			foreach (Control Item in _Items.Values.Reverse())
			{
				go = Item.OnKeyboardUp(e);
				if (go)
					break;
			}
			//Keyboard events should never be triggered in the base container event.
			return go;
		}

        internal override bool OnKeyboardPress(KeyEventArgs e)
        {
			bool go = false;
			foreach (Control Item in _Items.Values.Reverse())
			{
				go = Item.OnKeyboardDown(e);
				if (go)
					break;
			}
			//Keyboard events should never be triggered in the base container event.
			return go; 
        }
    }

}
