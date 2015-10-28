using System;

namespace MySoundLib
{
	public class DatabaseAccessDeniedExcpetion : Exception
	{
		 public DatabaseAccessDeniedExcpetion() { }

		 public DatabaseAccessDeniedExcpetion(string message) : base(message) { }
	}
}