
using System;

namespace Server.Exceptions
{
	public class ConfigFormatException : GeneralException
	{
		public ConfigFormatException(string message, Object Value, string RequiredFormat):base(message)
		{
			Data.Add("Value", Value);
			Data.Add("RequiredFormat", RequiredFormat);
		}
	}
}
