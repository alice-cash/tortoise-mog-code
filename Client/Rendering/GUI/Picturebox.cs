/*
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
using System.Runtime.CompilerServices;
using AgateLib.DisplayLib;
using AgateLib.DisplayLib.ImplementationBase;
using AgateLib.Geometry;
using AgateLib.Resources;

namespace Tortoise.Client.Rendering.GUI
{
    class Picturebox : Control
    {
        Surface _image;
        public Surface Image
        {
            get { return _image; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _image = value;
                _redrawPreRenderd = true;
            }

        }


        ImageDrawMode _drawMode;
        public ImageDrawMode DrawMode
        {
            get { return _drawMode; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                _drawMode = value;
                _redrawPreRenderd = true;
            }

        }

        public Picturebox(string name, Surface image, int x, int y, int width, int height)
            : this(name, image, new Rectangle(x, y, width, height))
        {

        }
        public Picturebox(string name, Surface image, Point location, Size size)
            : this(name, image, new Rectangle(location, size))
        {

        }
        public Picturebox(string name, Surface image, Rectangle area)
            : base(name, area)
        {
            _image = image;
        }

        protected override void Redraw_PreRenderd()
        {
            if (_preRenderd != null)
            {
                _preRenderd.Dispose();
                _preRenderd = null;
            }


            FrameBuffer previousBuffer = Display.RenderTarget;
            _preRenderd = new FrameBuffer(Size);
            Display.RenderTarget = _preRenderd;
            Display.BeginFrame();

            if (_backgroundColor != Color.Transparent)
                Display.Clear(_backgroundColor);
            if (_backgroundImage != null)
                _backgroundImage.Draw();

            switch (_drawMode)
            {
                    //TopLeft is the default, and its what we use
                    //if an invalid setting is used(the default:)
                case ImageDrawMode.Default:
                case ImageDrawMode.TopLeft:
                default:
                    _image.Draw();
                    break;
                case ImageDrawMode.Center:
                    Point ptmp = new Point();
                    ptmp.X = (this.Width / 2) - (_image.SurfaceWidth / 2);
                    ptmp.Y = (this.Height / 2) - (_image.SurfaceHeight / 2);
                    _image.Draw(ptmp);
                    break;
                case ImageDrawMode.Strech:
                    //The (double) is so that i am sure it is doing double division, not int division.
                    _image.ScaleWidth = this.Width / (double)_image.SurfaceWidth;
                    _image.ScaleHeight = this.Height / (double)_image.SurfaceHeight;
                    _image.Draw();
                    _image.SetScale(1, 1);
                    break;
            }

            Display.EndFrame();
            Display.FlushDrawBuffer();
            Display.RenderTarget = previousBuffer;
        }

    }
}
