using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class BadRequestException : Exception
	{
		public BadRequestException(string message) : base(message)
		{

		}
		public BadRequestException(string message, Exception innerException) : base(message, innerException)
		{

		}
		public BadRequestException() : base("The request was invalid or cannot be served. The exact error should be explained in the message.")
		{

		}
	}
}
