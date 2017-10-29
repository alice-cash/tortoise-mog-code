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
using Control = System.Windows.Forms.Control;
using Tortoise.Shared.Drawing;

namespace Tortoise.Graphics.Input
{
    public class TMouseState : InputState
    {

        private TGraphics _graphics;
        private MouseButtons[] _mouseStateArray;
        private Point _mouseStatePoint;
        private int _mouseStateWheel;

        public event Action<MouseEventArgs> MouseEvent;
        public event Action<MouseEventArgs> MouseDownEvent;
        public event Action<MouseEventArgs> MouseUpEvent;
        public event Action<MouseEventArgs> MouseMoveEvent;
        public event Action<MouseEventArgs> MouseWheelEvent;

        private List<MouseStateEventData> _mouseEventData;


        private struct MouseStateEventData
        {
            public MouseEventData Data;
            public MouseStateEventType Type;

            public MouseStateEventData(System.Windows.Forms.MouseEventArgs e, MouseStateEventType t)
            {
                Type = t;
                Data = new MouseEventData(e, t == MouseStateEventType.MouseDown);
            }
        }

        private enum MouseStateEventType
        {
            MouseDown,
            MouseUp,
            MouseMove,
            MouseWheel
        }


        public TMouseState(TGraphics graphics)
        {
            _mouseStateArray = new MouseButtons[0];
            _mouseEventData = new List<MouseStateEventData>();

            _graphics = graphics;

            graphics.Control.MouseDown += _control_MouseDown;
            graphics.Control.MouseUp += _control_MouseUp;
            graphics.Control.MouseMove += _control_MouseMove;
            graphics.Control.MouseWheel += _control_MouseWheel;
           
        }

        private void _control_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (_mouseEventData)
            {
                _mouseEventData.Add(new MouseStateEventData(e, MouseStateEventType.MouseWheel));
            }
        }

        private void _control_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (_mouseEventData)
            {
                _mouseEventData.Add(new MouseStateEventData(e, MouseStateEventType.MouseMove));
            }
        }

        private void _control_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (_mouseEventData)
            {
                _mouseEventData.Add(new MouseStateEventData(e, MouseStateEventType.MouseUp));
            }
        }

        private void _control_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (_mouseEventData)
            {
                _mouseEventData.Add(new MouseStateEventData(e, MouseStateEventType.MouseDown));
            }
        }

        public override bool Poll()
        {
            bool returnStatus = false;



            MouseStateEventData[] eventDataArray;

            lock (_mouseEventData)
            {
                eventDataArray = _mouseEventData.ToArray();
                _mouseEventData.Clear();
            }

            if(eventDataArray.Length == 0) { return returnStatus; }


            MouseButtons[] currentState = _getButtonState(eventDataArray);

            Point currentPoint = eventDataArray[0].Data.Position; 
  

            MouseButtons[] pressedButtons = GetNewItems(_mouseStateArray, currentState);
            MouseButtons[] releasedButtons = GetMissingItems(_mouseStateArray, currentState);

            //Subtract current values so positive values result in a positive delta.
            Point relativePoint = currentPoint - _mouseStatePoint;
            int wheelDelta = eventDataArray[0].Data.WheelDelta;
            _mouseStateWheel = _mouseStateWheel + wheelDelta;


            MouseEventData eventData = new MouseEventData(pressedButtons, releasedButtons, currentState, currentPoint, relativePoint, _mouseStateWheel, wheelDelta);

            //System.Diagnostics.Trace.WriteLine(eventData.ToString());

            if (pressedButtons.Length != 0 || releasedButtons.Length != 0 || relativePoint != Point.Empty || wheelDelta != 0)
            {
                MouseEvent?.Invoke(new MouseEventArgs(eventData));
                returnStatus = true;
            }

            if (pressedButtons.Length != 0)
            {
                MouseDownEvent?.Invoke(new MouseEventArgs(eventData));
            }

            if (releasedButtons.Length != 0)
            {
                MouseUpEvent?.Invoke(new MouseEventArgs(eventData));
            }


            if (relativePoint != Point.Empty)
            {
                MouseMoveEvent?.Invoke(new MouseEventArgs(eventData));
            }

            if (wheelDelta != 0)
            {
                MouseWheelEvent?.Invoke(new MouseEventArgs(eventData));
            }

            _mouseStateArray = currentState;
            return returnStatus;
        }

        private MouseButtons[] _getButtonState(MouseStateEventData[] data)
        {
  
             
            List<MouseButtons> ButtonList = new List<MouseButtons>();

            //This is all super fuckin redundant however the data from the Form events is spread out among a 
            // bunch of event data elements so this is a super hack to consolidate it all.
            foreach(var e in data)
            {
                if (e.Data.LeftButtonPressed)
                    ButtonList.Add(MouseButtons.Left);
                if (e.Data.MiddleButtonPressed)
                    ButtonList.Add(MouseButtons.Middle);
                if (e.Data.RightButtonPressed)
                    ButtonList.Add(MouseButtons.Right);
                if (e.Data.X1ButtonPressed)
                    ButtonList.Add(MouseButtons.X1);
                if (e.Data.X2ButtonPressed)
                    ButtonList.Add(MouseButtons.X2);

            }

            return ButtonList.ToArray();
        }
    }
}
