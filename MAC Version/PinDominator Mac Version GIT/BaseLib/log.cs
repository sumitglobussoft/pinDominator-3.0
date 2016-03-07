using System;

namespace BaseLib
{
	public delegate void AddToLoggerDelegate(string Text);
	public static class GlobusLogHelper
	{
		public static AddToLoggerDelegate objAddToLoggerDelegate ;
		public static class log
		{
			public static void Info (string Text)
			{
				try
				{
					objAddToLoggerDelegate (Text);
				}
				catch(Exception ex) 
				{
					Console.Write (ex.Message);
				}
			}

			public static void Error (string Text)
			{
				try
				{
					//objAddToLoggerDelegate (Text);
				}
				catch(Exception ex) 
				{
					Console.Write (ex.Message);
				}
			}

			public static void Debug (string Text)
			{
				try
				{
					//objAddToLoggerDelegate (Text);
				}
				catch(Exception ex) 
				{
					Console.Write (ex.Message);
				}
			}
	
		}
	}
}

