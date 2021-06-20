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
//using System.Drawing;

using Color = System.Drawing.Color;
using Tortoise.Shared.Drawing;
using Tortoise.Graphics.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
//using MonoGame.Extended.BitmapFonts;

namespace Tortoise.Graphics.Rendering.GUI
{
    /// <summary>
    /// Description of Button.
    /// </summary>
    public class Button : Label
    {
        public Surface MouseOverTexture { get; set; }
        public Surface MouseDownTexture { get; set; }

        private bool _useDownTexture;
        private bool _useOverTexture;



        public override bool OnMouseMove(MouseEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            if (!IsPointOver(e.MouseData.Position))
            {
                _redrawPreRenderd = _useOverTexture == true || _useDownTexture == true;
                _useOverTexture = false;
                _useDownTexture = false;

                return false;
            }
            _redrawPreRenderd = _useOverTexture == false;
            _useOverTexture = true;
            return DoMouseMove(e);
        }

        public override bool OnMouseDown(MouseEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            if (!IsPointOver(e.MouseData.Position)) return false;

            _redrawPreRenderd = _useDownTexture == false || _useOverTexture == true;
            _useOverTexture = false;
            _useDownTexture = true;
            return DoMouseDown(e);
        }

        public override bool OnMouseUp(MouseEventArgs e)
        {
            _threadSafety.EnforceThreadSafety();
            if (!IsPointOver(e.MouseData.Position)) return false;
            _redrawPreRenderd = _useOverTexture == true || _useDownTexture == true;
            _useOverTexture = false;
            _useDownTexture = false;
            return DoMouseUp(e);
        }

        public Button(TGraphics graphics, string name, string text, Point location, Size size, FontManager fontinfo)
            : this(graphics, name, text, new Rectangle(location, size), fontinfo)
        {

        }

        public Button(TGraphics graphics, string name, string text, int x, int y, int width, int height, FontManager fontinfo)
            : this(graphics, name, text, new Rectangle(x, y, width, height), fontinfo)
        {

        }

        public Button(TGraphics graphics, string name, string text, Rectangle area, FontManager fontinfo)
            : base(graphics, name, text, area, fontinfo)
        {

        }

        public override void Load()
        {
            base.Load();
            _useDownTexture = false;
            _useOverTexture = false;
        }

        protected new void Redraw_PreRenderd()
        {
            _preRenderdSurface.BeginChanges();

            _preRenderdSurface.Fill(_backgroundColor);


            if (_backgroundImage != null &&
                !(_useOverTexture == true && MouseOverTexture != null) &&
                !(_useDownTexture == true && MouseDownTexture != null))
                _preRenderdSurface.Blit(_backgroundImage);

            if (_useOverTexture == true && MouseOverTexture != null &&
                !(_useDownTexture == true && MouseDownTexture != null))
                _preRenderdSurface.Blit(MouseOverTexture);

            if (_useDownTexture == true && MouseDownTexture != null)
                _preRenderdSurface.Blit(MouseDownTexture);


            if (_backgroundImage != null)
                _preRenderdSurface.Blit(_backgroundImage);

            FontManager.DrawString(_fontInfo, _text, new Point(0, 0), TextColor);

            //_graphics.SpriteBatch.DrawString(_fontInfo.Bitmap, _text, new Vector2(0, 0), ColorTools.ToXNAColor(TextColor));


            _preRenderdSurface.EndChanges();

        }
    }
}