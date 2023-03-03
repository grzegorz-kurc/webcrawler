using Microsoft.Extensions.Configuration;

namespace WebCrawler.Infrastructure
{
	public class Configuration
	{
		public static IConfigurationRoot LoadAppSettings()
		{
			try
			{
				// Requires:
				// Microsoft.Extensions.Configuration
				// Microsoft.Extensions.Configuration.FileExtensions
				// Microsoft.Extensions.Configuration.Json

				var configurationRoot = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", false, true)
					.Build();

				return !string.IsNullOrEmpty(configurationRoot["App:StartingUrl"])
				    && !string.IsNullOrEmpty(configurationRoot["App:NextUrl"])
				    && !string.IsNullOrEmpty(configurationRoot["App:Path"])
				    && !string.IsNullOrEmpty(configurationRoot["App:ColumnNames"])
					&& !string.IsNullOrEmpty(configurationRoot["App:TextToFind"]) ? configurationRoot :
					throw new ArgumentException("Error occurred: one of the required parameters is missing from the main configuration file.");
			}
			catch (FileNotFoundException)
			{
				throw new FileNotFoundException("Error: main configuration file missing");
			}
			catch (Exception ex)
			{
				throw new Exception($"Error: {ex.Message}\n {ex.InnerException}");
			}
		}
	}
}
