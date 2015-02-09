﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using Tortoise.Client;
using Tortoise.Client.Module;
using Tortoise.Client.Net;
using Tortoise.Shared.Module;
using Tortoise.Shared.Localization;
using Tortoise.Shared.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

using GorgonLibrary;

using GorgonLibrary.Graphics;
using GorgonLibrary.Diagnostics;
using GorgonLibrary.IO;
using GorgonLibrary.Math;
using GorgonLibrary.Renderers;
using GorgonLibrary.UI;

using Tortoise.Client.Screen;

using Tortoise.Graphics.Rendering;
using Tortoise.Graphics;

using Tortoise.Shared;

namespace Tortoise.Client
{
    class Game
    {


        public Game(MainForm Target)
        {
            _form = Target;


            // Create the graphics interface.
            _graphics = new GorgonGraphics();

            _window = new Window("Game Window");

            // Create the primary swap chain.
            _mainScreen = _graphics.Output.CreateSwapChain("MainScreen", new GorgonSwapChainSettings
            {
                Width = 800,
                Height = 600,
                Format = BufferFormat.R8G8B8A8_UIntNormal,
                Window = _form,
                IsWindowed = true
            });

            _renderer = _graphics.Output.Create2DRenderer(_mainScreen);
            _renderer.IsLogoVisible = false;


            _mainScreen.AfterSwapChainResized += (sender, args) =>
            {

            };

            _window.Initialize();

        }


        public bool GameLoop()
        {

            GorgonRectangle tt = Program.GameLogic.Renderer2D.Renderables.CreateRectangle("TEST",
                new Rectangle(1,1,100,100),Color.Gray, true);

            tt.Color = Color.Gray;

            tt.Draw();


            /*

            GorgonFont f = FontInfo.GetInstance(20, FontTypes.Serif).GFont;
            GorgonRenderTarget2D target = _graphics.Output.CreateRenderTarget("Target",
                                                                      new GorgonRenderTarget2DSettings
                                                                      {
                                                                          Width = 640,
                                                                          Height = 480,
                                                                          Format = BufferFormat.R8G8B8A8_UIntNormal
                                                                      });
            GorgonText text = _renderer.Renderables.CreateText("MyText", f, "Test text", Color.Black);

            _renderer.Target = target;
            _renderer.Clear(Color.White);

            text.Draw();

            _renderer.Target = null;

            target.Save(@"text.png", new GorgonLibrary.IO.GorgonCodecPNG());*/


            _renderer.Drawing.DrawString(FontInfo.GetInstance(12.5f, FontTypes.Sans).GFont, "Arial", new SlimMath.Vector2(10, 10), Color.White);
            _renderer.Drawing.DrawString(FontInfo.GetInstance(12.5f, FontTypes.Serif).GFont, "Times New Roman", new SlimMath.Vector2(10, 30), Color.White);
            _renderer.Drawing.DrawString(FontInfo.GetInstance(12.5f, FontTypes.Sans_Mono).GFont, "Lucida Sans", new SlimMath.Vector2(10, 50), Color.White);
            _renderer.Drawing.DrawString(FontInfo.GetInstance(12.5f, FontTypes.Serif_Mono).GFont, "Courier New", new SlimMath.Vector2(10, 70), Color.White);
            

            _renderer.Render();


            return true;
        }

        internal void CleanUp()
        {

        }
    }
}
