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
using Tortoise.Shared.Drawing;

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Tortoise.Graphics.Input
{
	/// <summary>
	/// Description of MouseEventArgs.
	/// </summary>
    public class MouseEventArgs : EventArgs
	{


        public MouseEventData MouseData { get; private set; }
        public MouseEventArgs(MouseEventData args)
        {
            MouseData = args;
        }

	}

    public struct MouseEventData
    {
        public IEnumerable<MouseButtons> ButtonsPressed { get; private set; }
        public IEnumerable<MouseButtons> ButtonsReleased { get; private set; }
        public IEnumerable<MouseButtons> ButtonsDown { get; private set; }
        public Shared.Drawing.Point Position { get; private set; }
        public Shared.Drawing.Point RelativePosition { get; private set; }
        public int WheelPosition { get; private set; }
        public int WheelDelta { get; private set; }

        public bool LeftButtonPressed { get; private set; }
        public bool RightButtonPressed { get; private set; }
        public bool MiddleButtonPressed { get; private set; }
        public bool X1ButtonPressed { get; private set; }
        public bool X2ButtonPressed { get; private set; }

        public MouseEventData(IEnumerable<MouseButtons> bP, IEnumerable<MouseButtons> bR, IEnumerable<MouseButtons> bD, Point pos, Point rPos, int wheel, int wD)
        {
            ButtonsPressed = bP;
            ButtonsReleased = bR;
            ButtonsDown = bD;
            Position = pos;
            RelativePosition = rPos;
            WheelPosition = wheel;
            WheelDelta = wD;

            LeftButtonPressed = false;
            RightButtonPressed = false;
            MiddleButtonPressed = false;
            X1ButtonPressed = false;
            X2ButtonPressed = false;

            foreach(MouseButtons button in bD)
            {
                switch (button)
                {
                    case MouseButtons.Left:
                        LeftButtonPressed = true;
                        continue;
                    case MouseButtons.Right:
                        RightButtonPressed = true;
                        continue;
                    case MouseButtons.Middle:
                        MiddleButtonPressed = true;
                        continue;
                    case MouseButtons.X1:
                        X1ButtonPressed = true;
                        continue;
                    case MouseButtons.X2:
                        X2ButtonPressed = true;
                        continue;

                }
            }
        }
    }

    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
        X1,
        X2
    }
}
