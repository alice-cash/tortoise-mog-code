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
using Tortoise.Graphics.Input;
using Tortoise.Graphics.Rendering.GUI;

namespace Tortoise.Graphics.Rendering
{
    /// <summary>
    /// Description of Screen.
    /// </summary>
    public abstract class Screen : Container, IRender
    {
        public Screen(TGraphics graphics)
            : base(graphics, "_Screen", 0, 0, graphics.ScreenSize.Width, graphics.ScreenSize.Height)
        {
        }

        internal new void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); }

        internal new void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); }

        internal new void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); }

        internal new void OnKeyboardDown(KeyEventArgs e) { base.OnKeyboardDown(e); }

        internal new void OnKeyboardUp(KeyEventArgs e) { base.OnKeyboardUp(e); }

        internal new void OnKeyboardPress(KeyEventArgs e) { base.OnKeyboardPress(e); }

        internal virtual void OnResize() { this.Size = _graphics.ScreenSize; }

        public new void Tick(TickEventArgs e) { base.Tick(e); }

        public new void Render() { base.Render(); }

        
        public abstract void Initialize();

    }
}
