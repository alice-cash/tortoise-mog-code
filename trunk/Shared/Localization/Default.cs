﻿/*
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

using System.Globalization;



namespace Tortoise.Shared.Localization
{
	/// <summary>
    /// Attempts to load a language file based off of the current system culture, en-us, or the first one avalaible.
	/// </summary>
	public class DefaultLanguage
	{
        private static  LanguageStrings _strings;
		public static LanguageStrings Strings{
            get
            {
                if (_strings == null)
                {
                    InitDefault();
                }
                return _strings; }
        }

        public static void InitDefault()
        {
            var languages = LanguageStrings.GetAvalableLanguages();
            if (languages.Length == 0)
                throw new Exception("No lanuage files found!");
            if (languages.Length == 1)
            {
                _strings = languages[0];
            }
            InitAs(CultureInfo.InstalledUICulture, languages);
        }

        /// <summary>
        /// Will attempt to load a language file based off of the provided culture, otherwise it will default to either the system culture, en-us, or the first one avalaible.
        /// </summary>
        public static void InitDefaultAs(CultureInfo ci)
        {
            var languages = LanguageStrings.GetAvalableLanguages();
            if (languages.Length == 0)
                throw new Exception("No lanuage files found!");
            if (languages.Length == 1)
            {
                _strings = languages[0];
            }
            InitAs(ci, languages);
            InitAs(CultureInfo.InstalledUICulture, languages);
        }

        private static void InitAs(CultureInfo ci, LanguageStrings[] languages)
        {
            foreach (LanguageStrings strings in languages)
                if (strings.LanguageName == ci.Name)
                {
                    _strings = strings;
                    return;
                }
        }
	}
}
