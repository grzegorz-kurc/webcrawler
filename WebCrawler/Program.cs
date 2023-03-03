using HtmlAgilityPack;
using System.Diagnostics;
using WebCrawler.Models;

namespace WebCrawler
{
	internal class Program
	{
		public static void Main()
		{
			var appStartTime = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var config = Infrastructure.Configuration.LoadAppSettings();

			if (config == null)
			{
				Logger.Log("An error occurred while processing the configuration file.", Directory.GetCurrentDirectory());
				throw new ArgumentNullException("config", "An error occurred while processing the configuration file.");
			}
			var logPath = $@"{config["App:Path"]}\Logs\log_" + appStartTime + "_.txt";

			try
			{
				var httpClient = new HttpClient();

				Logger.Log("--- App started ---", logPath);

				// Start parsing
				var htmlPagesList = Helpers.GetHtmlPagesList(httpClient, config["App:StartingUrl"], logPath, config);
				var offerList = new List<Offer>();

				foreach (var page in htmlPagesList)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(page);

					var singlePageOfferList = htmlDoc.DocumentNode.SelectNodes(".//div[@class='tile-product']");

					foreach (var node in singlePageOfferList)
					{
						var offer = new Offer
						{
							Title = node.SelectSingleNode(".//a[contains(@class, 'tile-product__name')]").SelectSingleNode(".//strong").InnerText.Trim(),
							Subtitle = node.SelectSingleNode(".//a[contains(@class, 'tile-product__name')]").SelectSingleNode(".//span").InnerText.Trim(),
							Url = $"https://www.rossmann.pl/{node.SelectSingleNode(".//a[contains(@class, 'tile-product__name')]").Attributes["href"].Value}",
							Price = node.SelectSingleNode(".//span[@class='tile-product__current-price']")?.InnerText.Trim(),
							OldPrice = node.SelectSingleNode(".//span[@class='tile-product__old-price']")?.InnerText.Trim(),
							PromoPrice = node.SelectSingleNode(".//span[@class='tile-product__promo-price']")?.InnerText.Trim(),
						};

						offerList.Add(offer);
					}
				}

				foreach (var offer in offerList)
				{
					var task = Task.Run(() => Helpers.DownloadPageAsync(httpClient, offer.Url, logPath));
					var downloadedOfferPage = task.Result;

					var downloadedOfferPageHtmlDoc = new HtmlDocument();
					downloadedOfferPageHtmlDoc.LoadHtml(downloadedOfferPage);

					var button = downloadedOfferPageHtmlDoc.DocumentNode
						.SelectSingleNode(".//div[@class='product__desctiption']").Descendants()
						.FirstOrDefault(node => node.InnerText == config["App:TextToFind"]);

					offer.Description = button?.NextSibling.InnerText;
				}

				var offersToSaveList = offerList;

				Logger.Log(
					ExcelCreator.CreateExcel(offersToSaveList, config, appStartTime, logPath)
						? "The application completed without errors."
						: $"The application completed the task with errors. Check the log file {logPath}", logPath);

				Logger.Log($"--- App stopped, elapsed time: {stopwatch.Elapsed.Minutes}min {stopwatch.Elapsed.Seconds}s {stopwatch.Elapsed.Milliseconds}ms ---", logPath);
			}
			catch (Exception e)
			{
				Logger.Log($"General error {e.Message} {e.InnerException}", logPath);
				throw;
			}
		}
	}
}