/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/29/2010
 * Time: 11:25 PM
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
using AgateLib;
using AgateLib.Geometry;
using AgateLib.DisplayLib;
using AgateLib.Resources;

using Tortoise.Client.Rendering.GUI;


namespace Tortoise.Client.Rendering
{
	/// <summary>
	/// Description of MainMenuScreen.
	/// </summary>
	public class MainMenuScreen : Control, IScreen
	{
		AgateResourceCollection _resourceCollection;
		Container _renderItems;
		
		public MainMenuScreen():base("_Screen",Point.Empty,Size.Empty)
		{
			_resourceCollection = new AgateResourceCollection();
			_renderItems  = new Container("_parent", 0,0,Display.CurrentWindow.Width, Display.CurrentWindow.Height);
			_renderItems._backgroundColor = Color.Wheat;
		}
		
		public  void Dispose()
		{
			_renderItems.Dispose();
		}
		
		public  void Init()
		{
			_renderItems.Init();
			
			Label b1 = new Label("_b1", "HELLO WORLD", new Point(0,0), new Size(200,40));
			TextBox b2 = new TextBox("_b2", new Point(100,40), new Size(200,40));
			b1.TextAlignement = TextAlignement.Center;
			b2.Text = "HELLO WORLD";
			_renderItems.Controls.Add(10, b1);
			_renderItems.Controls.Add(10, b2);

			_renderItems.Controls["_b1"].BackgroundColor = Color.Red;
			_renderItems.Controls["_b2"].BackgroundColor = Color.Transparent;
		}
		
		public override void Load()
		{
			_renderItems.Load();
		}
		
		public override void Unload()
		{
			_renderItems.Unload();
		}
		
		public new void Render()
		{
			_renderItems.Render();
		}
		
		public new void Tick(TickEventArgs e)
		{
			
			_renderItems.Tick(e);
		}
		
		public  void OnMouseUp(MouseEventArgs e)
		{
			_renderItems.OnMouseUp(e);
		}
		
		public  void OnMouseMove(MouseEventArgs e)
		{
			_renderItems.OnMouseMove(e);
		}
		
		public  void OnMouseDown(MouseEventArgs e)
		{
			_renderItems.OnMouseDown(e);
		}
		
		public  void OnKeyboardUp(MouseEventArgs e)
		{
			_renderItems.OnKeyboardUp(e);
		}
		
		public  void OnKeyboardDown(MouseEventArgs e)
		{
			_renderItems.OnKeyboardDown(e);
		}
	}
}
