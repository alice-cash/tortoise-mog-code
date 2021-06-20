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
using System.Drawing;
using System.Runtime.CompilerServices;


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

        protected new void Redraw_PreRenderd()
        {
            _preRenderdSurface.Fill(BackgroundColor);
            if (_backgroundImage != null)
                _preRenderdSurface.Blit(_backgroundImage);

            switch (_drawMode)
            {
                    //TopLeft is the default, and its what we use
                    //if an invalid setting is used(the default:)
                case ImageDrawMode.Default:
                case ImageDrawMode.TopLeft:
                default:
                    _preRenderdSurface.Blit(_image);
                    break;
                case ImageDrawMode.Center:
                    Point ptmp = new Point();
                    ptmp.X = (this.Width / 2) - (_image.Width / 2);
                    ptmp.Y = (this.Height / 2) - (_image.Height / 2);
                    _preRenderdSurface.Blit(_image, ptmp);
                    break;
                case ImageDrawMode.Strech:
                    //The (double) is so that i am sure it is doing double division, not int division.

                    _preRenderdSurface.Blit(_image, new Rectangle(new Point(0,0), new Size(this.Width, this.Height)));
                    break;
            }
        }

    }
}
