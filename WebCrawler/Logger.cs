namespace WebCrawler
{
	public static class Logger
	{
		public static void Log(string logMessage, string filePath)
		{
			// using var streamWriter = File.AppendText(filePath);


			// string filePath = @"C:\Folder\file.txt";

			// Sprawdź, czy folder istnieje
			var folderPath = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(folderPath))
			{
				if (folderPath != null) Directory.CreateDirectory(folderPath);
			}

			// Otwórz plik do zapisu
			using var streamWriter = File.AppendText(filePath);


			try
			{
				var stringToLog = $"{DateTime.Now:dd.MM.yyyy_HH:mm:ss:fff} - {logMessage}";
				streamWriter.WriteLine(stringToLog);
				Console.WriteLine(stringToLog);
			}
			catch (Exception ex)
			{
				var stringToLog = $"{DateTime.Now:dd.MM.yyyy_HH:mm:ss:fff} {"- BŁĄD LOGGERA:"} {ex.Message}";
				streamWriter.WriteLine(stringToLog);
				Console.WriteLine(stringToLog);
			}
		}
	}
}
