using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tortoise.Graphics.Rendering;

using GorgonLibrary.Graphics;
using GorgonLibrary.Diagnostics;
using GorgonLibrary.IO;
using GorgonLibrary.Math;
using GorgonLibrary.Renderers;
using GorgonLibrary.UI;

using Tortoise.Shared.Drawing;

namespace Tortoise.Graphics
{
    public class TGraphics
    {

        /// <summary>
        /// The main Form
        /// </summary>
        private System.Windows.Forms.Control _control; 

        /// <summary>
        /// The Graphics interface.
        /// </summary>
        private GorgonGraphics _graphics;

        /// <summary>
        /// The 2D Interface.
        /// </summary>
        private Gorgon2D _renderer;

        /// <summary>
        /// The swap chain for our primary display.
        /// </summary>
        private GorgonSwapChain _mainScreen;

        public GorgonGraphics Graphics { get { return _graphics; } }
        public Gorgon2D Renderer2D { get { return _renderer; } }

        public System.Windows.Forms.Control Control { get { return _control; } }

        public Size ScreenSize { get { return Size.FromSystem(_control.Size); } }

        private Window _window;
        public Window Window { get { return _window; } }

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

            this._initGraphics();


            this._window = new Window(this);


            this._renderer = this._graphics.Output.Create2DRenderer(this._mainScreen);

            this._renderer.IsLogoVisible = false;


            this._mainScreen.AfterSwapChainResized += this._afterSwapChainResized;

            this._window.Initialize();
        }

        //TODO: Convert to using an external source for graphics settings and resolution.
        private void _initGraphics()
        {
            this._graphics = new GorgonGraphics();

            this._mainScreen = _graphics.Output.CreateSwapChain("MainScreen", new GorgonSwapChainSettings
            {
                Width = 800,
                Height = 600,
                Format = BufferFormat.R8G8B8A8_UIntNormal,
                Window = _control,
                IsWindowed = true
            });
        }

        public bool DoRenderLoop()
        {
            this._window.Render();
            this.Renderer2D.Render();
            return true;
        }


        private void _afterSwapChainResized(object sender, GorgonAfterSwapChainResizedEventArgs args)
        {

        }
    }
}
