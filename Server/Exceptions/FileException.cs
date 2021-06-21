using System;

namespace Server.Exceptions
{
	public class FileException : Exception
	{
		public FileException(string message, string FileName, Exception InnerException):base(message, InnerException)
		{
			Data.Add("Filename", FileName);
		}
		
		
	}
}
