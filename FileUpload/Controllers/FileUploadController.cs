using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections;
using System.Data;
using System.Xml.Linq;

namespace FileUpload.Controllers
{
	public class FileUploadController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public ActionResult UploadFile(IFormFile file)
		{
			try
			{
				if (file != null && file.Length > 0)
				{
					if (Path.GetExtension(file.FileName) != ".csv")
					{
						TempData["Message"] = "Please upload a valid CSV file";
					}
					else
					{
						var list = ConvertCSVDataToList(file);

						if (IsSorted(list))
						{
							int threshold = list.Count / 2;

							var result = list.GroupBy(x => x)
												.Where(g => g.Count() > threshold)
												.Select(g => g.Key)
												.FirstOrDefault();
							if (result == 0)
							{
								TempData["Message"] = "-1";
							}
							else
							{
								TempData["Message"] = $"{result}";
							}
						}
						else
						{
							TempData["Message"] = "Data is not sorted in non-decreasing order";
						}
					}
				}
				else
				{
					TempData["Message"] = "File can not be empty";
				}
				return RedirectToAction("Index");
			}
			catch
			{
				TempData["Message"] = "File upload failed!!";
				return RedirectToAction("Index");
			}
		}

		public static List<int> ConvertCSVDataToList(IFormFile file)
		{
			var list = new List<int>();
			using (var stream = file.OpenReadStream())
			using (var reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');

					if (int.TryParse(values[0], out int value))
					{
						list.Add(value);
					}
				}
			}
			return list;
		}

		private bool IsSorted(List<int> number)
		{
			for (int i = 0; i < number.Count - 1; i++)
			{
				if (number[i] > number[i + 1])
				{
					return false;
				}
			}
			return true;
		}
	}
}
