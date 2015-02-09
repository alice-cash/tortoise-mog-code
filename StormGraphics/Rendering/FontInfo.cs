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
 * */

using System;
using System.Collections.Generic;
using System.Text;

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
    public class FontInfo
    {
        static List<FontInfo> _data = new List<FontInfo>();
        private System.Drawing.Text.PrivateFontCollection FontData;


        public static FontInfo GetInstance(float size, FontTypes name)
        {
            foreach(FontInfo obj in _data)
            {
                if (obj.FontName == name && obj.FontSize == size)
                    return obj;
            }
            FontInfo font = new FontInfo(name, size);
            _data.Add(font);
            return font;
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

        public GorgonLibrary.Graphics.GorgonFont GFont { get; private set; }

        /// <summary>
        /// Placeholder to eat it!
        /// </summary>
        private FontInfo()
        {
          
        }
        private FontInfo(FontTypes type, float size)
        {
           
            FontSize = size;
            FontName = type;
            Font = new System.Drawing.Font(GetName(type),size);
            GFont = Program.GameLogic.Graphics.Fonts.CreateFont(Font.Name, Font, GorgonLibrary.Graphics.FontAntiAliasMode.AntiAlias);
        }


    }

}
