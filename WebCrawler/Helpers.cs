using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;

namespace WebCrawler
{
	public class Helpers
	{
		public static async Task<string> DownloadPageAsync(HttpClient httpClient, string pageUrl, string logPath)
		{
			Logger.Log($"Parsing: {pageUrl} ", logPath);
			var response = await httpClient.GetAsync(pageUrl);

			return await response.Content.ReadAsStringAsync();
		}

		public static int GetPagesCount(string html)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(html);

			var pagesCount = htmlDoc.DocumentNode.SelectSingleNode(".//a[contains(@class, 'pages__last')]").InnerText;

			if (int.TryParse(pagesCount, out var num))
			{
				return num;
			}

			return -1;
		}

		public static IEnumerable<string> GetHtmlPagesList(HttpClient httpClient, string? startingUrl, string logPath, IConfiguration config)
		{
			if (string.IsNullOrEmpty(startingUrl))
			{
				Logger.Log("Error: startingUrl is empty.", logPath);
				return new List<string>();
			}

			var htmlPagesList = new List<string>();
			var task = Task.Run(() => DownloadPageAsync(httpClient, startingUrl, logPath));

			var downloadedFirstPage = task.Result;

			if (downloadedFirstPage != string.Empty)
			{
				htmlPagesList.Add(downloadedFirstPage);
				var pagesCount = GetPagesCount(downloadedFirstPage);

				if (pagesCount > 1)
				{
					Logger.Log($"Pages to parse: {pagesCount}", logPath);

					for (var i = 2; i <= pagesCount; i++)
					{
						var url = $"{config["App:NextUrl"]}{i}";
						var taskDownloadPage = Task.Run(() => DownloadPageAsync(httpClient, url, logPath));
						var resultDownloadPage = taskDownloadPage.Result;

						htmlPagesList.Add(resultDownloadPage);
					}
				}
			}

			return htmlPagesList;
		}
	}
}
