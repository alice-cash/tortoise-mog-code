/*
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *          conditions and the following disclaimer.
 *          
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *          of conditions and the following disclaimer in the documentation and/or other materials
 *                provided with the distribution.
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

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using System.Text;
using Tortoise.Shared.Drawing;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Tortoise.Graphics.Rendering
{
    public enum FontTypes
    {
        Sans,
        Serif,
        Sans_Mono,
        Serif_Mono,
    }

    /// <summary>
    /// Stores and returns Font data. Based around the Vera font set.
    /// </summary>
    public class FontManager
    {
        static List<FontManager> _data = new List<FontManager>();
        const int FontFileSize = 36; //Font file is in 36pt and is downscaled for 
        private TGraphics _graphics;

        public static FontManager GetInstance(TGraphics graphics, float size, FontTypes name)
        {
            foreach (FontManager obj in _data)
            {
                if (obj.FontName == name && obj.FontSize == size && obj._graphics == graphics)
                    return obj;
            }
            FontManager font = new FontManager(graphics, name, size);
            _data.Add(font);
            return font;
        }

        public static void DrawString(FontManager font, string text, Point position, Color color)
        {
            float scale = font.FontSize / FontFileSize;
            font._graphics.SpriteBatch.DrawString(font.Bitmap, text, position.ToVector2, ColorTools.ToXNAColor(color), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }


        public static void DrawString(TGraphics graphics, string text, float size, FontTypes name, Point position, Color color)
        {
            FontManager font = GetInstance(graphics, size, name);

            DrawString(font, text, position, color);
        }

        public static PointF MeasureString(FontManager font, string text)
        {
            return PointF.FromVector2(font.Bitmap.MeasureString(text)) * (font.FontSize / FontFileSize);
        }

        private static string GetName(FontTypes type)
        {
            switch (type)
            {
                default: return @"Arial";
                case FontTypes.Sans: return @"Arial";
                case FontTypes.Serif_Mono: return @"Courier New";
                case FontTypes.Serif: return @"Times New Roman";
                case FontTypes.Sans_Mono: return @"Lucida Sans Typewriter";
            }
        }

        public float FontSize { get; private set; }
        public FontTypes FontName { get; private set; }
        public System.Drawing.Font Font { get; private set; }

        private SpriteFont Bitmap { get; set; }

        /// <summary>
        /// Placeholder to eat it!
        /// </summary>
        private FontManager() { }

        private FontManager(TGraphics graphics, FontTypes type, float size)
        {
            _graphics = graphics;
            FontSize = size;
            FontName = type;
            Font = new System.Drawing.Font(GetName(type), size);
            Bitmap = graphics.Content.Load<SpriteFont>(string.Format("{0}-{1}", GetName(type), FontFileSize));
        }
/*
        private int getFontSize(float size)
        {
            if (size < 6 || size > 38) throw new ArgumentOutOfRangeException("size");

            int isize = (int)Math.Ceiling(size);

            foreach (int i in new int [] {12,18,24,30,38})
                if (isize <= i) return i;

            throw new InvalidProgramException("You should not see this. Bug in getFontSize()?");
        }


        private static int GetFontScaler(int i)
        {
            i--;
            i |= i >> 1;
            i |= i >> 2;
            i |= i >> 4;
            i |= i >> 8;
            i++;
            return i;
        }
        */

    }

}
