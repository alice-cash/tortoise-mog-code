/*
 * Copyright 2011 Matthew Cash. All rights reserved.
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
using System.Runtime.Serialization;
using C5;

namespace Tortoise.Server.Collection
{
    //TODO: Totally screwed up, fix
 
    //class CachedDictionary<K, X> : DictionaryBase<K, CachedData<X>>
    //{
    //    int _position = 0;
    //    Type _default;

    //    public CachedDictionary(Type defaultType)
    //        : base(C5.EqualityComparer<K>.Default)
    //    {
    //        _position = 0;

    //    }


    //    public void Poll(int count = 100)
    //    {
    //        //If the count is greater than number of items in the que, we are just going to check them all.
    //        if (count >= Count)
    //        {
    //            foreach (var cd in this)
    //            {
    //                cd.Value.Poll();
    //            }
    //        }
    //        else
    //        {

    //        }
    //    }

    //    public override object Clone()
    //    {
    //        CachedDictionary<K, X> clone = new CachedDictionary<K, X>(_default);
    //        clone.AddAll(this);
    //        return clone;
    //    }
    //}
}
