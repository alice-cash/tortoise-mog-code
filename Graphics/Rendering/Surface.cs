using System;
using GorgonLibrary.Graphics;
using GorgonLibrary.Renderers;
//using System.Drawing;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;

using Tortoise.Shared.Drawing;
using Tortoise.Shared.Threading;

namespace Tortoise.Graphics.Rendering
{
    /// <summary>
    /// A surface which holds texture data.
    /// </summary>
    [ThreadSafe(ThreadSafeFlags.ThreadUnsafe)]
    public class Surface : IDisposable
    {
        public int Width, Height;
        //GorgonLibrary.Graphics.GorgonTexture2D _texture;
        GorgonLibrary.Graphics.GorgonRenderTarget2D _target;
        private bool _can_Update;
        TGraphics _graphics;

       /* public GorgonLibrary.Graphics.GorgonRenderTarget2D Target
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

        private static GorgonLibrary.Graphics.GorgonTexture2D CreateEmptyTexture(TGraphics graphics, int width, int height)
        {
            GorgonLibrary.Graphics.GorgonTexture2D newtexture;

            newtexture = graphics.Graphics.Textures.CreateTexture(string.Format("Nondescript_Texture:_{0}x{1}", width, height),
                width, height, BufferFormat.R8G8B8A8_UIntNormal, BufferUsage.Default);

            return newtexture;
        }

        private static GorgonLibrary.Graphics.GorgonRenderTarget2D CreateEmptyTarget(TGraphics graphics, int width, int height)
        {
            GorgonLibrary.Graphics.GorgonRenderTarget2D newtexture;

            newtexture = graphics.Graphics.Output.CreateRenderTarget(string.Format("Nondescript_Texture:_{0}x{1}", width, height), 
                            new GorgonRenderTarget2DSettings  {
                                /*DepthStencilFormat = BufferFormat.Unknown,*/
                                Width = width,
                                Height = height,
                                Format = BufferFormat.R8G8B8A8_UIntNormal,
                                /*  Multisampling = GorgonMultisampling.NoMultiSampling*/
                            });

            return newtexture;
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

            GorgonLibrary.IO.GorgonImageCodec Codec = null;

            if (Filename.EndsWith(".png", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecPNG();
            if (Filename.EndsWith(".bmp", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecBMP();
            if (Filename.EndsWith(".jpg", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecJPEG();
            if (Filename.EndsWith(".jpeg", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecJPEG();
            if (Filename.EndsWith(".tga", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecTGA();
            if (Filename.EndsWith(".dds", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecDDS();
            if (Filename.EndsWith(".gif", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecGIF();
            if (Filename.EndsWith(".tif", true, System.Globalization.CultureInfo.CurrentCulture))
                Codec = new GorgonLibrary.IO.GorgonCodecTIFF();

            if (Codec == null)
                throw new System.IO.FileLoadException("The file contains an invalid extension!", Filename);

            _target = _graphics.Graphics.Textures.FromFile<GorgonRenderTarget2D>("Nondescript_Texture", Filename, Codec);


            Width = _target.Settings.Width;
            Height = _target.Settings.Height;
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

            _graphics.Renderer2D.Drawing.Blit(Source._target, rec.ToSystemF());
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

            _graphics.Renderer2D.Drawing.Blit(_target, rec.ToSystemF());
        }

        public void BeginChanges()
        {
            _graphics.Renderer2D.Target = _target;
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
            _graphics.Renderer2D.Target = null;
        }

        public void Fill(Color Color)
        {
            if (!_can_Update)
                throw new Tortoise.Shared.Exceptions.LogicException("Surface cannot be updated at this time!");
            _graphics.Renderer2D.Clear(Color);
            /*


            Program.GameLogic._2D.Target=target;

            Texture.Settings.
            Gorgon2D _2D = Texture.Graphics.Output.Create2DRenderer(GorgonRenderTarget2D.
           Program.GameLogic._2D.Drawing.FilledRectangle(new RectangleF(0, 0, Width, Height),Color);

                        Program.GameLogic._2D.Target=null;*/

        }

        /// <summary>
        /// Saves a PNG file of the surface.
        /// </summary>
        public void Save(string Path)
        {
            _target.Save(Path, new GorgonLibrary.IO.GorgonCodecPNG());
        }

        public void Dispose()
        {

        }

    }

}
