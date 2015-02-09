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
    public class Graphics
    {

        /// <summary>
        /// The main Form
        /// </summary>
        private MainForm _form; 

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

        public Size ScreenSize { get { return _form.Size; } }

        private Window _window;
        public Window Window { get { return _window; } }
    }
}
