using Microsoft.Extensions.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using WebCrawler.Models;

namespace WebCrawler
{
	public class ExcelCreator
	{
		public static bool CreateExcel(List<Offer> offersToSaveList, IConfiguration config, string appStartTime, string logPath)
		{
			try
			{
				var hsWorkbook = new HSSFWorkbook();
				var font = (HSSFFont)hsWorkbook.CreateFont();
				font.FontHeightInPoints = 11.0;
				font.FontName = "Calibri";
				var cellStyle1 = (HSSFCellStyle)hsWorkbook.CreateCellStyle();
				cellStyle1.SetFont(font);

				var cellStyle2 = (HSSFCellStyle)hsWorkbook.CreateCellStyle();
				cellStyle2.SetFont(font);
				cellStyle2.FillForegroundColor = IndexedColors.PaleBlue.Index;
				cellStyle2.FillPattern = FillPattern.SolidForeground;

				var sheet = hsWorkbook.CreateSheet("Pobrane_dane");
				var row1 = sheet.CreateRow(0);

				var listOfCols = config["App:ColumnNames"]?.Split(',').Select(p => p.Trim()).ToList();

				if (listOfCols != null)
				{
					for (var i = 0; i < listOfCols.Count; ++i)
					{
						CreateCell(row1, i, listOfCols[i], cellStyle2);
					}
				}

				var rownum = 1;

				foreach (var offerToSave in offersToSaveList)
				{
					var row2 = sheet.CreateRow(rownum);
					if (rownum > 0)
					{
						CreateCell(row2, 0, offerToSave.Title, cellStyle1);
						CreateCell(row2, 1, offerToSave.Subtitle, cellStyle1);
						if (offerToSave.Price != null) CreateCell(row2, 2, offerToSave.Price, cellStyle1);
						if (offerToSave.PromoPrice != null) CreateCell(row2, 3, offerToSave.PromoPrice, cellStyle1);
						if (offerToSave.OldPrice != null) CreateCell(row2, 4, offerToSave.OldPrice, cellStyle1);
						if (offerToSave.Description != null) CreateCell(row2, 5, offerToSave.Description, cellStyle1);
						CreateCell(row2, 6, offerToSave.Url, cellStyle1);
					}
					++rownum;
				}

				sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, 24));
				sheet.CreateFreezePane(0, 1);

				var lastCellNum = sheet.GetRow(0).LastCellNum;

				for (var column = 0; column <= lastCellNum; ++column)
				{
					sheet.AutoSizeColumn(column);
					GC.Collect();
				}

				using var out1 = new FileStream(@$"C:\Users\Dom\OneDrive - SI-C\_repo\WebCrawlerNew\WebCrawler\WebCrawler\Pobrane_dane_{appStartTime}_.xls", FileMode.Create);
				hsWorkbook.Write(out1);

				Logger.Log("The excel file was created successfully.", logPath);
				return true;
			}
			catch (Exception e)
			{
				Logger.Log($"Error: {e.Message}", logPath);
				return false;
			}
		} 
		
		private static void CreateCell(IRow currentRow, int cellIndex, string value, ICellStyle style)
		{
			var cell = currentRow.CreateCell(cellIndex);
			cell.SetCellValue(value);
			cell.CellStyle = style;
		}
	}
}
