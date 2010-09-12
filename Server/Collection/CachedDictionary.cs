/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 9/11/2010
 * Time: 10:04 PM
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
using System.Collections.Generic;
using System.Runtime.Serialization;
using C5;

namespace Tortoise.Server.Collection
{
    //TODO: Tottaly screwwed up, fix
    class CachedDictionary<K, X, T> : DictionaryBase<K, T> where T: CachedData<X>
    {
        int _position;

        public CachedDictionary()
            : base(C5.EqualityComparer<K>.Default)
        {
            _position = 0;
            
        }

        public new void Add(K key, X value)
        {
            CachedData<X> newValue = Activator.CreateInstance(typeof(T)) as CachedData<X>;
            newValue.Value = value;
            base.Add(key, newValue as T);
        }

        public override void AddAll<L, W>(IEnumerable<C5.KeyValuePair<L, W>> entries)
        {
            base.AddAll<L, W>(entries);
        }

        /*public override void CopyTo(C5.KeyValuePair<K, CachedData<X>>[] array, int index)
        {
            base.CopyTo(array, index);
        }

        public override bool Find(K key, out CachedData<X> value)
        {
            return base.Find(key, out value);
        }

        public override bool Find(ref K key, out CachedData<X> value)
        {
            return base.Find(ref key, out value);
        }

        public override bool FindOrAdd(K key, ref CachedData<X> value)
        {
            return base.FindOrAdd(key, ref value);
        }
        public override bool Remove(K key, out CachedData<X> value)
        {
            return base.Remove(key, out value);
        }

        public override CachedData<X> this[K key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public override bool Update(K key, CachedData<X> value)
        {
            return base.Update(key, value);
        }

        public override bool Update(K key, CachedData<X> value, out CachedData<X> oldvalue)
        {
            return base.Update(key, value, out oldvalue);
        }

        public override bool UpdateOrAdd(K key, CachedData<X> value)
        {
            return base.UpdateOrAdd(key, value);
        }

        public override bool UpdateOrAdd(K key, CachedData<X> value, out CachedData<X> oldvalue)
        {
            return base.UpdateOrAdd(key, value, out oldvalue);
        }

         */

        public void Poll(int count = 100)
        {
            //If the count is greater than number of items in the que, we are just going to check them all.
            if (count >= Count)
            {
                foreach (var cd in this)
                {
                    cd.Value.Poll();
                }
            }
            else
            {
                
            }
        }
    }
}
