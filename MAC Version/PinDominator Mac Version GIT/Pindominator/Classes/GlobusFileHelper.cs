using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Gtk;

namespace PinDominator
{
	public class GlobusFileHelper
	{
		public GlobusFileHelper ()
		{
		}

		private static int _bufferSize = 16384;


		public static List<string> ReadTweetFiletoStringList(string filepath)
		{
			List<string> list = new List<string>();
			StreamReader reader = new StreamReader(filepath);
			string text = "";
			while ((text = reader.ReadLine()) != null)
			{
				if (!string.IsNullOrEmpty(text.Trim()))
				{
					list.Add(text.Replace("�", " ").Replace("\0", ""));
				}
			}
			reader.Close();
			return list;

		}

		public static void AppendStringToTextfileNewLine(String content, string filepath)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(filepath, true))
				{
					using (StringReader reader = new StringReader(content))
					{
						string temptext = "";

						while ((temptext = reader.ReadLine()) != null)
						{
							writer.WriteLine(temptext);
						}
					}
				}
			}
			catch(Exception ex) { Console.WriteLine(ex.Message); }
		}


		public static List<string> ReadFile(string filename)
		{
			List<string> listFileContent = new List<string>();
			try
			{

				StreamReader reader = new StreamReader(filename, System.Text.Encoding.UTF8, true);

				StringBuilder stringBuilder = new StringBuilder();
				using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
				{
					try
					{                     
						using (StreamReader streamReader = new StreamReader(fileStream))
						{
							char[] fileContents = new char[_bufferSize];
							int charsRead = streamReader.Read(fileContents, 0, _bufferSize);

							// Can't do much with 0 bytes
							if (charsRead == 0)
								throw new Exception("File is 0 bytes");

							while (charsRead > 0)
							{
								stringBuilder.Append(fileContents);
								charsRead = streamReader.Read(fileContents, 0, _bufferSize);
							}

							string[] contentArray = stringBuilder.ToString().Split(new char[] { '\r', '\n' });
							foreach (string line in contentArray)
							{
								if (line.EndsWith("\0"))
								{
									listFileContent.Add(line.Replace("\0", "").Replace("�","\""));

								}
								else
								{
									listFileContent.Add(line);
								}
								//listFileContent.Add(line.Replace("#", ""));
							}
							listFileContent.RemoveAll(s => string.IsNullOrEmpty(s));
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
			return listFileContent;
		}
	

	public static void WriteListtoTextfile(List<string> list,string filepath)
	{
		try
		{
			using (StreamWriter writer = new StreamWriter(filepath))
			{
				foreach (string listitem in list)
				{
					writer.WriteLine(listitem);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.StackTrace);
		}
	}







	}

}

