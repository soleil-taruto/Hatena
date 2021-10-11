using System;

namespace SimpleWebServer
{
	class Program
	{
		static void Main(string[] args)
		{
#if !true
			SimpleWebServer.Run(args);
#else
			new SimpleWebServer()
			{
				DocRoot = @"C:\temp",
			}
			.Perform();
#endif
		}
	}
}
