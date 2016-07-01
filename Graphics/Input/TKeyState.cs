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
 
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace Tortoise.Graphics.Input
{
    public class TKeyState: InputState
    {
        private Keys[] _keyStateArray;

        public event Action<KeyEventArgs> KeyboardEvent;
        public event Action<KeyEventArgs> KeyboardKeyPressEvent;
        public event Action<KeyEventArgs> KeyboardKeyReleaseEvent;

        public TKeyState()
        {
            _keyStateArray = new Keys[0];
        }

        public override bool Poll()
        {
            bool returnStatus = false;
            Keys[] currentState = Keyboard.GetState().GetPressedKeys();

            Keys[] pressedKeys = GetNewItems(_keyStateArray, currentState);
            Keys[] releasedKeys = GetMissingItems(_keyStateArray, currentState);

            KeyEventData eventData = new KeyEventData(pressedKeys, releasedKeys, currentState);

            if (pressedKeys.Length != 0 || releasedKeys.Length !=0)
            {
                if(KeyboardEvent != null)
                {
                    KeyboardEvent(new KeyEventArgs(eventData));
                }
                returnStatus = true;
            }

            if (pressedKeys.Length != 0)
            {
                if (KeyboardKeyPressEvent != null)
                {
                    KeyboardKeyPressEvent(new KeyEventArgs(eventData));
                }
            }

            if (releasedKeys.Length != 0)
            {
                if (KeyboardKeyReleaseEvent != null)
                {
                    KeyboardKeyReleaseEvent(new KeyEventArgs(eventData));
                }
            }

            _keyStateArray = currentState;
            return returnStatus;
        }
    }
}
