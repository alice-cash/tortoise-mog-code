/*
 * Copyright 2016 Matthew Cash. All rights reserved.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tortoise.Graphics.Input;
using Tortoise.Graphics.Rendering;
using Rectangle = Tortoise.Shared.Drawing.Rectangle;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using Tortoise.Shared.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;

namespace Tortoise.Graphics
{
    public class TGraphics : IGraphicsDeviceService
    {

        /// <summary>
        /// The main Form
        /// </summary>
        private System.Windows.Forms.Control _control;

        PresentationParameters parameters;

        GameServiceContainer _services;
        public ContentManager Content { get; private set; }

        GraphicsDevice _graphicsDevice;
        SpriteBatch _spriteBatch;

        public System.Windows.Forms.Control Control { get { return _control; } }

        public Size ScreenSize { get { return Size.FromSystem(_control.ClientSize); } }

        private Window _window;

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public Window Window { get { return _window; } }

        public TInputManager InputManager { get; private set; }

        internal GraphicsDevice GraphicsDevice { get { return _graphicsDevice; } }
        internal SpriteBatch SpriteBatch { get { return _spriteBatch; } }

        GraphicsDevice IGraphicsDeviceService.GraphicsDevice
        {
            get
            {
                return _graphicsDevice;
            }
        }

        public static TGraphics CreateGraphics(System.Windows.Forms.Control control)
        {
            TGraphics graphics = new TGraphics();
            
            graphics._init(control);
            return graphics;
        }


        private TGraphics()
        {
        }


        private void _init(System.Windows.Forms.Control control)
        {

            this._control = control;

            this._window = new Window(this);

            

            _services = new GameServiceContainer();

            Content = new ContentManager(_services, "Content");
            
            InputManager = new TInputManager(this);

            Mouse.WindowHandle = control.Handle;


            parameters = new PresentationParameters();

            parameters.BackBufferWidth = Math.Max(ScreenSize.Width, 1);
            parameters.BackBufferHeight = Math.Max(ScreenSize.Height, 1);
            parameters.BackBufferFormat = SurfaceFormat.Color;
            parameters.DepthStencilFormat = DepthFormat.Depth24;
            parameters.DeviceWindowHandle = this._control.Handle;
            parameters.PresentationInterval = PresentInterval.Immediate;
            parameters.IsFullScreen = false;

            _graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
                                                GraphicsProfile.Reach,
                                                parameters);
            _spriteBatch = new SpriteBatch(_graphicsDevice);

            _services.AddService(_graphicsDevice);
            _services.AddService(_spriteBatch);
            _services.AddService(typeof(IGraphicsDeviceService), this);

            this._window.Initialize();
            this._window.Graphics.Control.Resize += _window_ScreenChanged;
            OnDeviceCreated(EventArgs.Empty);
        }

        private void _window_ScreenChanged(object sender, EventArgs e)
        {
            parameters = new PresentationParameters();

            parameters.BackBufferWidth = Math.Max(ScreenSize.Width, 1);
            parameters.BackBufferHeight = Math.Max(ScreenSize.Height, 1);
            parameters.BackBufferFormat = SurfaceFormat.Color;
            parameters.DepthStencilFormat = DepthFormat.Depth24;
            parameters.DeviceWindowHandle = this._control.Handle;
            parameters.PresentationInterval = PresentInterval.Immediate;
            parameters.IsFullScreen = false;

            _graphicsDevice.Reset(parameters);
        }

        public bool DoTick()
        {
            InputManager.DoEventPoll();
            this._window.Tick();
            return true;
        }

        
        public bool DoRender()
        {
            _spriteBatch.Begin();
            this._window.Render();
            _spriteBatch.End ();
            this._flush();
            return true;
        }

        private void _flush()
        {
            try
            {
                //Rectangle sourceRectangle = new Rectangle(0, 0, ScreenSize.Width,
                //                                                ScreenSize.Height);
                _graphicsDevice.Present();
                //_graphicsDevice.Present(sourceRectangle, null, parameters.DeviceWindowHandle);
            }
            catch
            {
                // Present might throw if the device became lost while we were
                // drawing. The lost device will be handled by the next BeginDraw,
                // so we just swallow the exception.
            }
        }

        public void Resize()
        {

        }

        internal void OnDeviceCreated(EventArgs e)
        {
            if (DeviceCreated != null)
                DeviceCreated(this, e);
        }

        internal void OnDeviceDisposing(EventArgs e)
        {
            if (DeviceDisposing != null)
                DeviceDisposing(this, e);
        }

        internal void OnDeviceResetting(EventArgs e)
        {
            if (DeviceResetting != null)
                DeviceResetting(this, e);
        }

        internal void OnDeviceReset(EventArgs e)
        {
            if (DeviceReset != null)
                DeviceReset(this, e);
        }

    }
}
