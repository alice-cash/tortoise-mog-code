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

namespace Tortoise.Shared.Threading
{
	/// <summary>
	/// The invoker clas is used to Invoke methods across threads.
	/// </summary>
	public class Invoker
	{
		ThreadSafetyEnforcer _threadSafety;
		Queue<InvokeItem> _invokeList;

		/// <summary>
		/// Creates a new instance of the Invoker Class, which is used to Invoke methods across threads.
		/// </summary>
		/// <param name="threadSafety"></param>
		public Invoker(ThreadSafetyEnforcer threadSafety)
		{
			_threadSafety = threadSafety;
			_invokeList  = new Queue<InvokeItem>();
		}
		
		/// <summary>
		/// Either adds the specified thread to the invoke list, or calls it now if its in the parent thread.
		/// </summary>
		/// <param name="methodToInvoke">A method or deligate to call.</param>
		/// <param name="userData">An object with information sent to the method.</param>
		public void InvokeMethod(System.Action<object> methodToInvoke, object userData)
		{
			if(!InvokeRequired())
			{
				//if its true we just run it.
				//no reason to run it later.
				methodToInvoke(userData);
				return;
			}
			//Othherwise we lock it and add it.
			lock(_invokeList)
			{
				_invokeList.Enqueue(new InvokeItem(methodToInvoke, userData));
			}
		}
		
		/// <summary>
		/// Returns true if we need to invoke a method.
		/// </summary>
		public bool InvokeRequired()
		{
			return !_threadSafety.CheckThreadSafety();
		}
		
				
		/// <summary>
		/// To be called by the parent thread, This checks the invoke list and runs any methods in it.
		/// </summary>
		public void PollInvokes()
		{
		 	//We want to throw an exception if we are calling this on a different thread.
			_threadSafety.EnforceThreadSafety();
			
			lock(_invokeList)
			{
				InvokeItem item;
				while(_invokeList.Count > 0)
				{
					item = _invokeList.Dequeue();
					item.Action(item.UserData);
				}
				
			}

		}
	}
}
