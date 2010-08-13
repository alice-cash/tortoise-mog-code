/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/13/2010
 * Time: 1:17 AM
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
using System.Threading;

namespace Tortoise.Shared.Threading
{
	/// <summary>
	/// Takes the thread ID used to create it and uses it when CheckThreadSafety and _threadSafety.EnforceThreadSafety() are called.
	/// </summary>
	public class ThreadSafetyEnforcer
	{
		//When this is false, this class is absoutly useless... Other than IsSameThread()
		internal bool _enforceThreadSafeCalls;
		internal int _safeThreadID;
		internal string _identifier;
		
		public bool EnforcingThreadSafety
		{
			get{return _enforceThreadSafeCalls;}
		}

		public ThreadSafetyEnforcer(string identifier) : this(identifier, true)
		{
		}
		
		public ThreadSafetyEnforcer(string identifier, bool _threadSafety)
		{
			_enforceThreadSafeCalls = _threadSafety;
			_safeThreadID = GetManagedThreadId();
			_identifier = identifier;

		}
		
		/// <summary>
		/// Throws an exception when the Thread Check fails.
		/// </summary>
		public void EnforceThreadSafety()
		{
			if(!CheckThreadSafety()) throw new InvalidOperationException(string.Format("Crossthread Access to '{0}' is not permited.", _identifier));
		}
		
		/// <summary>
		/// Returns true if it is safe, or we are not enforcing Safety.
		/// </summary>
		/// <returns></returns>
		public bool CheckThreadSafety()
		{
			//If enforce threadSafety is on, we return based on if the thread is the same as the parent,
			//otherwise we just return true because we want whatevers using this to suceed.
			return _enforceThreadSafeCalls ? _safeThreadID == GetManagedThreadId(): true;
		}
		
		/// <summary>
		/// Returns if the current thread is the same as the Orignral thread.
		/// </summary>
		/// <returns></returns>
		public bool IsSameThread()
		{
			return  _safeThreadID == GetManagedThreadId();
		}
		
		internal int GetManagedThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}
	}
}
