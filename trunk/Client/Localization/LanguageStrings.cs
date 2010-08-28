/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/3/2010
 * Time: 6:04 PM
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
using System.IO;
using C5;

namespace Tortoise.Client.localization
{
	/// <summary>
	/// Description of LanguageStrings.
	/// </summary>
	public class LanguageStrings
	{
		private static List<LanguageStrings> _languageStrings;
		public static LanguageStrings[] GetAvalableLanguages()
		{
			if(_languageStrings == null)
			{
				_languageStrings = new List<LanguageStrings>();
				foreach(string locFile in Directory.GetFiles("./localization/"))
				{
					if(locFile.EndsWith(".lang"))
						_languageStrings.Add(new LanguageStrings(locFile));
				}
				
			}
			return _languageStrings.ToArray();
		}
		
		private string _errorNoLocal = "No 'Error_No_Local' in language file!";
		private string _languageName = "No 'Language_Name' in language file!";
		
		public string ErrorNoLocal{get{return _errorNoLocal;}}
		public string LanguageName{get{return _languageName;}}
		
		public HashDictionary<string, string> Language{get; protected set;}
		
		public string GetFormatedString(string name, params object[] args)
		{
			if(Language.Contains(name))
				return string.Format(Language[name], args);
			return string.Format(ErrorNoLocal, name);
		}
		

		public LanguageStrings(string fileName)
		{
			Language = new C5.HashDictionary<string, string>();
			string type, text, line;
			foreach(string l in File.ReadAllLines(fileName))
			{
				line = l.Trim();
				if(line.StartsWith("#")) continue;
				if(!line.Contains(" ")) continue;
				
				type = line.Substring(0, line.IndexOf(' '));
				if(Language.Contains(type)) continue;
				text = line.Substring(line.IndexOf(' '));
				text = text.Trim();
				text = text.Replace("\\n","\n");
				Language.Add(type, text);
			}
			
			if(Language.Contains("Error_No_Local")) _errorNoLocal = Language["Error_No_Local"];
			if(Language.Contains("Language_Name")) _errorNoLocal = Language["Language_Name"];
			
		}
		
	}
}
