/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/3/2010
 * Time: 5:22 PM
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
using System.IO;
using System.Linq;
using C5;
using Tortoise.Client.Exceptions;
using Tortoise.Client.Collection;

namespace Tortoise.Client
{
	public enum ConsoleCommandSucess
	{
		Sucess,
		Failure
	}
	public struct ConsoleResponce
	{
		public string Value;
		public ConsoleCommandSucess Sucess;
	}
	public struct ConsoleVarable
	{
		public string Value;
		public Func<string, bool> ValidCheck;
	}

	/// <summary>
	/// This is the backend for a Console.
	/// </summary>
	public class Console
	{
		private HashDictionary<string, ConsoleVarable> _varables;
		private HashDictionary<string, System.Func<string[],ConsoleResponce>> _functions;
		
		private LimitedList<string> _consoleBacklog;
		
		public Console()
		{
			_varables = new HashDictionary<string, ConsoleVarable>();
			_functions = new HashDictionary<string, System.Func<string[],ConsoleResponce>>();
			_consoleBacklog = new LimitedList<string>(500, string.Empty);
		}
		
		public ConsoleVarable GetValue(string name)
		{
			if(_varables.Contains(name))
			{
				return _varables[name];
			}
			//this is called by normal code, so throw a normal exception!
			throw new InvalidVarableNameExceptions(string.Format("{0} does not exist",name));
		}
		
		public void SetValue(string name, ConsoleVarable value)
		{
			if(_varables.Contains(name))
			{
				_varables[name] = value;
			}
			else
			{
				_varables.Add(name, value);
			}
		}
		
		public bool ValueContains(string name)
		{
			return _varables.Contains(name);
		}
		
		public ConsoleResponce ExecuteFunc(string name, params string[] args)
		{
			ConsoleResponce cr = new ConsoleResponce();
			cr.Sucess = ConsoleCommandSucess.Failure;
			cr.Value = "";
			try
			{		
				if(_functions.Contains(name))
				{
					return _functions[name](args);
				}
			} catch(ConsoleException ex)
			{
				cr.Value = localization.Default.Strings.GetFormatedString("Console_Function_Exception", ex);
			}
			return cr;
		}
		
		public void SetFunc(string name, System.Func<string[],ConsoleResponce> func)
		{
			if(_varables.Contains(name))
			{
				_functions[name] = func;
			}
			else
			{
				_functions.Add(name, func);
			}
		}
		
		public bool FuncContains(string name)
		{
			return _functions.Contains(name);
		}
		
		public ConsoleResponce ProcessLine(string line)
		{
			
			ConsoleResponce cr = new ConsoleResponce();
			cr.Sucess=ConsoleCommandSucess.Sucess;
			if(line == "")
			{
				cr.Value="";
				return cr;
			}
			
			string[] split = line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			string name = split[0], value = "";
			Array.Copy(split,1,split,0,split.Length - 1);
			
			if(_varables.Contains(name) && split.Length == 0)
			{
				cr.Value = string.Format("{0} = {1}\n", name, _varables[name]);
				return cr;
			}
			value =  String.Join(" ", split);
			if(_varables.Contains(name))
			{
				if(value.StartsWith("=") || value.StartsWith("\\"))
					value.Remove(0,1);
				if(_varables[name].ValidCheck(value))
				{
					ConsoleVarable cv = _varables[name];
					cv.Value = value;
					_varables[name] = cv;
				}
				else
				{
					cr.Sucess = ConsoleCommandSucess.Failure;
					cr.Value = localization.Default.Strings.GetFormatedString("Console_Validation_Failure", name, value);
					return cr;
				}
			}
			if(_functions.Contains(name))
			{
				return ExecuteFunc(name, value);
			}
			
			return cr;
		}
	}
}
