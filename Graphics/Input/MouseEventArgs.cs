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

using System.Linq;

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Text;

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

        public MouseEventData(System.Windows.Forms.MouseEventArgs e, bool IsDownEvent)
        {
            ButtonsPressed = new MouseButtons[0];
            ButtonsReleased = new MouseButtons[0];
            ButtonsDown = new MouseButtons[0];

            

            Position = Point.FromPoint(e.Location);
            RelativePosition = Position;
            WheelPosition = 0;
            WheelDelta = e.Delta;

            if (e.Button != System.Windows.Forms.MouseButtons.None)
            {
                if (IsDownEvent)
                    ButtonsPressed = new MouseButtons[] { WinFormToTortus(e.Button) };
                else
                    ButtonsReleased = new MouseButtons[] { WinFormToTortus(e.Button) };
            }

            ButtonsDown = ButtonsPressed;

            LeftButtonPressed = false;
            RightButtonPressed = false;
            MiddleButtonPressed = false;
            X1ButtonPressed = false;
            X2ButtonPressed = false;

            foreach (MouseButtons button in ButtonsDown)
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

        

        public override string ToString()
        {
            return string.Format("[(Pressed:{0}),(Released:{1}),(Down:{2}),(Position:{3}),(Wheel:{4}),(WheelDelta:{5})",
                _buttonDataToString(ButtonsPressed), _buttonDataToString(ButtonsReleased), _buttonDataToString(ButtonsDown), 
                Position.ToString(),WheelPosition.ToString(),WheelDelta.ToString());
        }

        private static string _buttonDataToString(IEnumerable<MouseButtons> data)
        {
            if(data.Count() == 0) { return "{}"; }
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (MouseButtons e in data)
            {
                sb.Append(e.ToString());
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        private static MouseButtons WinFormToTortus(System.Windows.Forms.MouseButtons button)
        {
            switch (button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    return MouseButtons.Left;
                case System.Windows.Forms.MouseButtons.Middle:
                    return MouseButtons.Middle;
                case System.Windows.Forms.MouseButtons.Right:
                    return MouseButtons.Right;
                case System.Windows.Forms.MouseButtons.XButton1:
                    return MouseButtons.X1;
                case System.Windows.Forms.MouseButtons.XButton2:
                    return MouseButtons.X2;
            }
            return MouseButtons.None;
        }
    }

    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
        X1,
        X2,
        None
    }
}
