/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/2/2010
 * Time: 3:58 PM
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
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;

using SharedServerLib.Exceptions;

namespace SharedServerLib.Misc
{
	/// <summary>
	/// Description of IPRange.
	/// </summary>
	[Serializable]
	public class IPRange
	{
	
		public string IPAddress
		{ get; set; }
		public string IPHostMask
		{ 
			get 
			{
				return _IPHostMask;
			}
			set
			{
				if(!Hostmasks.Contains(value))
				{
					throw new TortusFormatException("Subnet Mask is invalid");
				}
				_IPHostMask = value;
			}
		}
		
		private string _IpAddress;
		private string _IPHostMask;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="IP">A string in the format of xxx.xxx.xxx.xxx</param>
		/// <param name="subnetmask">A string that contains a properly formated subnet mask </param>
		public IPRange(string IP, string subnetmask)
		{
			if(!Hostmasks.Contains(subnetmask))
			{
				throw new TortusFormatException("Subnet Mask is invalid");
			}
		}
		
		
		#region static properties and methods
		
		/// <summary>
		/// All of the proper subnet masks
		/// </summary>
		public static readonly string[] Hostmasks = new string[]
		{ 	"0.0.0.0",
			"128.0.0.0",         "192.0.0.0",       "224.0.0.0",       "240.0.0.0",       "248.0.0.0",       "252.0.0.0",       "254.0.0.0",       "255.0.0.0",
			"255.128.0.0", 	     "255.192.0.0",     "255.224.0.0",     "255.240.0.0",     "255.248.0.0",     "255.252.0.0",     "255.254.0.0",     "255.255.0.0",
			"255.255.128.0",   	 "255.255.192.0",   "255.255.224.0",   "255.255.240.0",   "255.255.248.0",   "255.255.252.0",   "255.255.254.0",   "255.255.255.0",
			"255.255.255.128",   "255.255.255.192", "255.255.255.224", "255.255.255.240", "255.255.255.248", "255.255.255.252", "255.255.255.254", "255.255.255.255"
		};
		
		/// <summary>
		/// This takes an IP/Mask Input and returns an IP range value.
		/// </summary>
		/// <param name="input">A string in the form of xxx.xxx.xxx.xxx/yy</param>
		/// <returns></returns>
		public static IPRange RangeFromIPMask(string input)
		{
			Regex IPMaskPattern = new Regex("[0-223]\\.[0-255]\\.[0-255]\\.[0-255]\\\\[0-32]");
			if(!IPMaskPattern.IsMatch(input))
			{
				throw new TortusFormatException("IPMask Input is Invalid");
			}
			
			string[] Split = input.Split(new char[] {'\\'});
			string IP = Split[0];
			int HostIndex = int.Parse(Split[1]);
			IPRange IP = new IPRange(IP, Hostmasks[HostIndex]);
			
			return IP;
		}
		

		
		public bool IsIPInRange(string IP)
		{
			
			
		}

		#endregion

	}
}
