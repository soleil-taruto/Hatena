using System;

namespace SimpleWebServer
{
	class Program
	{
		static void Main(string[] args)
		{
#if true
			SimpleWebServer.Run(@"C:\temp", 80);
#elif false
			SimpleWebServer.Run(args); // old
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
