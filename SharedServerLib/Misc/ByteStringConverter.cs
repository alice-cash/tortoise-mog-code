/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/2/2010
 * Time: 2:47 AM
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
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using SharedServerLib.Exceptions;

namespace SharedServerLib.Misc
{
	/// <summary>
	/// This class simply converts data between an array of bytes and a human redable integer string
	/// </summary>
	public static class ByteStringConverter
	{
		public static byte[] StringToBytes(string Input)
		{
			string[] Splited = Input.Split(new char[] {','});
			
			byte[] Output = new byte[Splited.Length];
			try
			{
				for(int i = 0; i < Splited.Length; i++)
				{
					Output[i] = byte.Parse(Splited[i]);
				}
			}
			catch(System.FormatException ex)
			{
				throw new TortusFormatException("Invalid entity in input.", ex);
			}catch(System.OverflowException ex)
			{
				throw new TortusFormatException("Invalid entity in input.", ex);				
			}
			return Output;
		}
		
			
		public static string BytesToString(byte[] Input)
		{
			List<string> NewArray = new List<string>();
			foreach(var b in Input)
				NewArray.Add(b.ToString());
			return NewArray.Aggregate((a,b) => a + ", " + b);
		}
	}
}
