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
using GorgonLibrary.Input;

namespace Tortoise.Client.Rendering
{
	/// <summary>
	/// Description of MouseEventArgs.
	/// </summary>
    public class MouseEventArgs : EventArgs
	{


        public PointingDeviceButtons Buttons { get; private set; }

        public int ClickCount { get; private set; }
        public bool DoubleClick { get; private set; }
        public Shared.Drawing.Point2D Position { get; private set; }
        public Shared.Drawing.Point2D RelativePosition { get; private set; }
        public PointingDeviceButtons ShiftButtons { get; private set; }
        public int WheelDelta { get; private set; }
        public int WheelPosition { get; private set; }
        


        //private PointingDeviceEventArgs _eventData;
        //private PointF _mousePosition;
		//private int _wheelDelta;

        //public PointingDeviceEventArgs EventData { get { return _eventData; } }
        //public Point Position { get { return Position }
        //public Point RelativePosition { get { return new Point((int)base.RelativePosition.X, (int)base.RelativePosition.Y); } }

        public MouseEventArgs(PointingDeviceEventArgs args)
        {
            ClickCount = args.ClickCount;
            DoubleClick = args.DoubleClick;
            Position = Point2D.FromPointF(args.Position);
            RelativePosition = Point2D.FromPointF(args.RelativePosition);

            ShiftButtons = args.ShiftButtons;
            WheelDelta = args.WheelDelta;
            WheelPosition = args.WheelPosition;

        }

		//public int WheelDelta {get{return _wheelDelta;}}
/*
        public MouseEventArgs(PointingDeviceEventArgs TKargs):base(
		{
            _eventData = TKargs;
            _mousePosition = TKargs.Position;
           // _wheelDelta = openTKargs.WheelDelta;
		}  */   

	}
}
