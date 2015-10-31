using System;

namespace MySoundLib.Exceptions
{
	public class DatabaseAccessDeniedExcpetion : Exception
	{
		 public DatabaseAccessDeniedExcpetion() { }

		 public DatabaseAccessDeniedExcpetion(string message) : base(message) { }
	}
}