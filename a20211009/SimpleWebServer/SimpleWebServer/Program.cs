using System;

namespace SimpleWebServer
{
	class Program
	{
		static void Main(string[] args)
		{
			new SimpleWebServer()
			{
				DocRoot = @"C:\temp",
			}
			.Perform();
		}
	}
}
