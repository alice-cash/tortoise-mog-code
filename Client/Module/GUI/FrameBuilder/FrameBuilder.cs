/*
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *	1. Redistributions of source code must retain the above copyright notice, this list of
 *	   conditions and the following disclaimer.
 * 
 *	2. Redistributions in binary form must reproduce the above copyright notice, this list
 *	   of conditions and the following disclaimer in the documentation and/or other materials
 *	   provided with the distribution.
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
using Tortoise.Client.Exceptions;
using System.Drawing;
using Tortoise.Client.Rendering;

namespace Tortoise.Client.Module.GUI.FrameBuilder
{
    class FrameBuilder
    {
        public static FrameBuilder Frame1 = new FrameBuilder(@"./Module/GUI/FrameBuilder/Frame1.png");
        public static FrameBuilder Frame2 = new FrameBuilder(@"./Module/GUI/FrameBuilder/Frame2.png");
        public static FrameBuilder Frame3 = new FrameBuilder(@"./Module/GUI/FrameBuilder/Frame3.png");
        public static FrameBuilder Frame4 = new FrameBuilder(@"./Module/GUI/FrameBuilder/Frame4.png");
        public static FrameBuilder Frame5 = new FrameBuilder(@"./Module/GUI/FrameBuilder/Frame5.png");

        //Simple naming structure:
        //t = Top, m = Middle, b = Bottom
        //l = Left, c = Center, r = Right
        private Surface _tl, _tc, _tr,
                        _ml, _mc, _mr,
                        _bl, _bc, _br;

        Size _subSize;

        public FrameBuilder(string filename)
            : this(new Surface(filename))
        {
        }
        public FrameBuilder(Surface filesurface)
        {
            if (filesurface.Width % 3 != 0 || filesurface.Height % 3 != 0)
                throw new ResourceLoadingException("Image size must be divisible by 3");
            _subSize = new Size(filesurface.Width / 3, filesurface.Height / 3);
            int x, y;
            x = 0;
            y = 0;
            _tl = CreateSurfaceFromClipRectangle(filesurface, new Point(x, y), _subSize);
            x += _subSize.Width;
            _tc = CreateSurfaceFromClipRectangle(filesurface, new Point(x, y), _subSize);
            x += _subSize.Width;
            _tr = CreateSurfaceFromClipRectangle(filesurface, new Point(x, y), _subSize);
            y += _subSize.Height;
            x = 0;
            _ml = CreateSurfaceFromClipRectangle(filesurface, new Point(x, y), _subSize);
            x += _subSize.Width;
            _mc = CreateSurfaceFromClipRectangle( filesurface, new Point(x, y), _subSize);
            x += _subSize.Width;
            _mr = CreateSurfaceFromClipRectangle( filesurface, new Point(x, y), _subSize);
            y += _subSize.Height;
            x = 0;
            _bl = CreateSurfaceFromClipRectangle( filesurface, new Point(x, y), _subSize);
            x += _subSize.Width;
            _bc = CreateSurfaceFromClipRectangle( filesurface, new Point(x, y), _subSize);
            x += _subSize.Width;
            _br = CreateSurfaceFromClipRectangle(filesurface, new Point(x, y), _subSize);
        }

        private Surface CreateSurfaceFromClipRectangle(Surface source, Point pos, Size size)
        {
            Surface Sf = Rendering.Window.Instance.GenerateSurface(size);
            Sf.Blit(source, new Rectangle(pos, size));
            return Sf;
        }

        public Surface CreateFrame(Size frameSize)
        {
            Surface frame = Rendering.Window.Instance.GenerateSurface(frameSize);

            //If the requested frame is small enough that a full frame won't fit(2x the width and height), we just
            //draw the center area.
            if (frameSize.Width < _subSize.Width * 2 || frameSize.Height < _subSize.Height * 2)
            {
                for (int x = 0; x < frameSize.Width; x += _subSize.Width)
                    for (int y = 0; y < frameSize.Width; y += _subSize.Height)
                        frame.Blit(_mc, new Point(x, y));
            }
            else
            {

                //For sake of easiness, we draw repeating textures to a sub surface, which is drawn to the main.
                Surface tmpframe = Rendering.Window.Instance.GenerateSurface(frameSize.Width - (_subSize.Width * 2), frameSize.Height - (_subSize.Height * 2));
                //draw the center texture first
                for (int x = 0; x < frameSize.Width - _subSize.Width; x += _subSize.Width)
                    for (int y = 0; y < frameSize.Height - _subSize.Height; y += _subSize.Height)
                        frame.Blit(_mc, new Point(x, y));

                frame.Blit(tmpframe, new Point(_subSize));

                tmpframe = Rendering.Window.Instance.GenerateSurface(frameSize.Width - (_subSize.Width * 2), _subSize.Height * 2);

                //Top Center
                for (int x = 0; x < frameSize.Width - _subSize.Width - 1; x += _subSize.Width)
                    frame.Blit(_tc, new Point(x, 0));


                frame.Blit(tmpframe, new Point(_subSize.Width, 0));


                tmpframe = Rendering.Window.Instance.GenerateSurface(frameSize.Width - (_subSize.Width * 2), _subSize.Height * 2);

                //Bottom Center
                for (int x = 0; x < frameSize.Width - _subSize.Width - 1; x += _subSize.Width)
                    frame.Blit(_bc, new Point(x, 0));

                frame.Blit(tmpframe, new Point(_subSize.Width, frameSize.Height - _subSize.Height));

                tmpframe = Rendering.Window.Instance.GenerateSurface(_subSize.Width, frameSize.Height - (_subSize.Height * 2));

                //Middle Right
                for (int y = 0; y < frameSize.Height - _subSize.Height - 1; y += _subSize.Height)
                    frame.Blit(_mr, new Point(0, y));

                frame.Blit(tmpframe, new Point(frameSize.Width - _subSize.Width, _subSize.Height));

                tmpframe = Rendering.Window.Instance.GenerateSurface(_subSize.Width, frameSize.Height - (_subSize.Height * 2));

                //Middle Left
                for (int y = 0; y < frameSize.Height - _subSize.Height - 1; y += _subSize.Height)
                    frame.Blit(_ml, new Point(0, y));

                frame.Blit(tmpframe, new Point(0, _subSize.Height));

                //Top left
                frame.Blit(_tl);
                //Top Right
                frame.Blit(_tr, new Point(frameSize.Width - _subSize.Width, 0));
                //Bottom Left
                frame.Blit(_bl, new Point(0, frameSize.Height - _subSize.Height));
                //Bottom Right
                frame.Blit(_br, new Point(frameSize.Width - _subSize.Width, frameSize.Height - _subSize.Height));
            }
            return frame;
        }
    }
}
