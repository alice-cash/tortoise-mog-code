using System;

namespace Server.Exceptions
{
	public class MissingResourceException : Exception
	{
		public MissingResourceException(string message, string ResourceName):base(message)
		{
			Data.Add("ResourceName", ResourceName);
		}
	}
}
