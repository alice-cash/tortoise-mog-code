/*
 * Copyright 2016 Matthew Cash. All rights reserved.
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

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise.Shared.Drawing;

namespace Tortoise.Graphics.Input
{
    public class TMouseState : InputState
    {

        private MouseButtons[] _mouseStateArray;
        private Point _mouseStatePoint;
        private int _mouseStateWheel;

        public event Action<MouseEventArgs> MouseEvent;
        public event Action<MouseEventArgs> MouseDownEvent;
        public event Action<MouseEventArgs> MouseUpEvent;
        public event Action<MouseEventArgs> MouseMoveEvent;
        public event Action<MouseEventArgs> MouseWheelEvent;

        public TMouseState()
        {
            _mouseStateArray = new MouseButtons[0];
        }

        public override bool Poll()
        {
            bool returnStatus = false;
            MouseState ms = Mouse.GetState();
            MouseButtons[] currentState = _getButtonState(ms);
            Point currentPoint = (Point)ms.Position; //Explicit conversion from XNA point to Tortus Point
            int currentWheel = ms.ScrollWheelValue;

            MouseButtons[] pressedButtons = GetNewItems(_mouseStateArray, currentState);
            MouseButtons[] releasedButtons = GetMissingItems(_mouseStateArray, currentState);

            //Subtract current values so positive values result in a positive delta.
            Point relativePoint = currentPoint - _mouseStatePoint;
            int wheelDelta = currentWheel - _mouseStateWheel;

            MouseEventData eventData = new MouseEventData(pressedButtons, releasedButtons, currentState, currentPoint, relativePoint, currentWheel, wheelDelta);

            if (pressedButtons.Length != 0 || releasedButtons.Length != 0 || relativePoint != Point.Empty || wheelDelta != 0)
            {
                if (MouseEvent != null)
                {
                    MouseEvent(new MouseEventArgs(eventData));
                }
                returnStatus = true;
            }

            if (pressedButtons.Length != 0)
            {
                if (MouseDownEvent != null)
                {
                    MouseDownEvent(new MouseEventArgs(eventData));
                }
            }

            if (releasedButtons.Length != 0)
            {
                if (MouseUpEvent != null)
                {
                    MouseUpEvent(new MouseEventArgs(eventData));
                }
            }


            if (relativePoint != Point.Empty)
            {
                if (MouseMoveEvent != null)
                {
                    MouseMoveEvent(new MouseEventArgs(eventData));
                }
            }

            if (wheelDelta != 0)
            {
                if (MouseWheelEvent != null)
                {
                    MouseWheelEvent(new MouseEventArgs(eventData));
                }
            }

            _mouseStateArray = currentState;
            return returnStatus;
        }

        private MouseButtons[] _getButtonState(MouseState ms)
        {
  
            List<MouseButtons> ButtonList = new List<MouseButtons>();
            if (ms.LeftButton == ButtonState.Pressed)
                ButtonList.Add(MouseButtons.Left);
            if (ms.RightButton == ButtonState.Pressed)
                ButtonList.Add(MouseButtons.Right);
            if (ms.MiddleButton == ButtonState.Pressed)
                ButtonList.Add(MouseButtons.Middle);
            if (ms.XButton1 == ButtonState.Pressed)
                ButtonList.Add(MouseButtons.X1);
            if (ms.XButton2 == ButtonState.Pressed)
                ButtonList.Add(MouseButtons.X2);

            return ButtonList.ToArray();
        }
    }
}
