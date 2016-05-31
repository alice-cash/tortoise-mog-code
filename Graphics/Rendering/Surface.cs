using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System.Drawing;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;
using XColor = Microsoft.Xna.Framework.Color;
using Point = Tortoise.Shared.Drawing.Point;
using Rectangle = Tortoise.Shared.Drawing.Rectangle;

using Tortoise.Shared.Drawing;
using Tortoise.Shared.Threading;
using System.IO;

namespace Tortoise.Graphics.Rendering
{
    /// <summary>
    /// A surface which holds texture data.
    /// </summary>
    [ThreadSafe(ThreadSafeFlags.ThreadUnsafe)]
    public class Surface : IDisposable
    {
        public int Width, Height;
        //Texture2D _texture;
        RenderTarget2D _target;
        private bool _can_Update;
        TGraphics _graphics;

       /* public RenderTarget2D Target
        {
            get { return _target; }
        }*/

        public static Surface CreateBlankSurface(TGraphics graphics, int Width, int Height)
        {
            Surface result;
            result = new Surface(graphics, Width, Height);
            result.Initialize();
            return result;
        }

        public static Surface CreateFromFile(TGraphics graphics, string Filename)
        {
            Surface result;
            result = new Surface(graphics, Filename);
            result.Initialize();
            return result;
        }

        private static Texture2D CreateEmptyTexture(TGraphics graphics, int width, int height)
        {
            Texture2D newtexture;
            
            newtexture = new Texture2D(graphics.GraphicsDevice, width,height, false, SurfaceFormat.Color);

            return newtexture;
        }

        private static RenderTarget2D CreateEmptyTarget(TGraphics graphics, int width, int height)
        {
            RenderTarget2D newtarget;

            newtarget = new RenderTarget2D(graphics.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            
            return newtarget;
        }

        private Surface(TGraphics graphics, int width, int height)
        {
            _graphics = graphics;


            _target = CreateEmptyTarget(_graphics, width, height);

            this.Width = width;
            this.Height = height;
        }

        private Surface(TGraphics graphics, string Filename)
        {
            _graphics = graphics;

            if (!System.IO.File.Exists(Filename))
                throw new System.IO.FileNotFoundException("The file could not be found!", Filename);

            
            _target = (RenderTarget2D)RenderTarget2D.FromStream(graphics.GraphicsDevice, new FileStream(Filename, FileMode.Open));

            Width = _target.Width;
            Height = _target.Height;
        }



        private void Initialize()
        {



        }

        public void Blit(Surface Source)
        {
            Blit(Source, new Point(0, 0));
        }

        public void Blit(Surface Source, Rectangle rec)
        {
            if (!_can_Update)
                throw new Tortoise.Shared.Exceptions.LogicException("Surface cannot be updated at this time!");

            _graphics.SpriteBatch.Draw(Source._target, rec.ToRender(), XColor.White);
        }
        public void Blit(Surface Source, Point pos)
        {

            Blit(Source, new Rectangle(pos, new Size(Source.Width, Source.Height)));
        }

        public void Render(Point pos)
        {
            Render(new Rectangle(pos, new Size(Width, Height)));
        }

        public void Render(Rectangle rec)
        {
            if (_can_Update)
                throw new Tortoise.Shared.Exceptions.LogicException("Surface changes must be disabled!");


            _graphics.SpriteBatch.Draw(_target, rec.ToRender(),XColor.White);
        }

        public void BeginChanges()
        {
            _graphics.GraphicsDevice.SetRenderTarget(_target);
            _graphics.SpriteBatch.Begin();
            _can_Update = true;
        }

        /*   public void FlushChanges()
           {
               if (!_can_Update)
                   throw new Tortoise.Shared.Exceptions.LogicException("Surface cannot be updated at this time!");
               Program.GameLogic.Renderer2D.Render();
           }*/

        public void EndChanges()
        {
            if (!_can_Update)
                throw new Tortoise.Shared.Exceptions.LogicException("Surface cannot be updated at this time!");
            //FlushChanges();
            _can_Update = false;
            _graphics.SpriteBatch.End();
            _graphics.GraphicsDevice.SetRenderTarget(null);
        }

        public void Fill(Color color)
        {
            if (!_can_Update)
                throw new Tortoise.Shared.Exceptions.LogicException("Surface cannot be updated at this time!");
            _graphics.GraphicsDevice.Clear(ToXNA(color));
            /*


            Program.GameLogic._2D.Target=target;

            Texture.Settings.
            Gorgon2D _2D = Texture.Graphics.Output.Create2DRenderer(GorgonRenderTarget2D.
           Program.GameLogic._2D.Drawing.FilledRectangle(new RectangleF(0, 0, Width, Height),Color);

                        Program.GameLogic._2D.Target=null;*/

        }

        private XColor ToXNA(Color color)
        {
            return new XColor(color.R, color.G, color.B);
        }

        /// <summary>
        /// Saves a PNG file of the surface.
        /// </summary>
        public void Save(string Path)
        {
            _target.SaveAsPng(new FileStream(Path, FileMode.Create), Width,Height);
        }

        public void Dispose()
        {

        }

    }

}
