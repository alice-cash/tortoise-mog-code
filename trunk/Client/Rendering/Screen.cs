/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/29/2010
 * Time: 10:45 PM
 * 
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
using Tortoise.Client.Rendering.GUI;

namespace Tortoise.Client.Rendering
{
    /// <summary>
    /// Description of Screen.
    /// </summary>
    public class Screen : Container, IRender
    {
        public Screen()
            : base("_Screen", 0, 0, Window.ScreenWidth, Window.ScreenHeight)
        {

        }

        public new void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); }

        public new void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); }

        public new void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); }

        public new void OnKeyboardDown(KeyEventArgs e) { base.OnKeyboardDown(e); }

        public new void OnKeyboardUp(KeyEventArgs e) { base.OnKeyboardUp(e); }

        public void OnResize() { this.Size = Window.MainWindow.Size; }

        public new void Tick(TickEventArgs e) { base.Tick(e); }

        public new void Render() { base.Render(); }

    }
}
