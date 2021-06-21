using System;

namespace Server.Exceptions
{
	public class GeneralException: Exception
	{
		public GeneralException():base("A generic Exception has occured.")
		{
		}
		public GeneralException(string message):base(message)
		{
		}
		public GeneralException(string message, Exception innerException):base(message,innerException)
		{
		}
		
		
	}
}
