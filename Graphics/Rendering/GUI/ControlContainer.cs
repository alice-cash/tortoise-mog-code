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
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Tortoise.Client.Extension.System;

namespace Tortoise.Graphics.Rendering.GUI
{
    public class ControlContainerManager : SortedList<float, Control>
    {
        Random _random;
        public ControlContainerManager()
            : base(new ControlSorter())
        {
            _random = new Random();
        }

        public void Add(Control control)
        {
            this.Add(0, control);
        }

        public void Add(int depth, Control control)
        {
            float fdepth;
            // Very simple random depth.
            // Its bad if 2 controls share the same depth
            // since we cannot guarantee which will be on top
            // This should almost always only run once, though 
            // we still want to handle the 1 in 10^75 or something
            // chance of getting a duplicate number should 1 exists 
            // already in that particular depth
            while (ContainsKey(fdepth = _nextFloat(_random) + depth)) ;
            Add(fdepth, control);
        }

        private static float _nextFloat(Random _random)
        {
            return Convert.ToSingle(_random.NextDouble());
        }

        public Control this[string name]
        {
            get
            {
                foreach (Control item in this.Values)
                    if (item.Name == name) return item;
                return null;
            }
        }
    }
}
