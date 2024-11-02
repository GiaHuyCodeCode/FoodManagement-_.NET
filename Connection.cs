using System;
using System.Collections.Generic;
using System.Congfiguration;
using System.Linq;
using System.Web;

namespace FoodShop
{ 
	public class Connection
	{
		public static string GetConnection 
		{
		return ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
		}
	}
	public class Utils
	{
		public static bool IsValidExtension(string fileName)
		{
			bool isValid = false;
			srting[] fileExtension = { ".jpg", ".png", ".jpeg" };
			for (int i = 0; i <= fileExtension.Length - 1; i++)
			{
				if (fileName.Contains(fileExtension[i])
					{
					isValid = true;
					break;
				}
			}
			return isValid;
		}
	}
	public static string GetImageUrl(Object url)
	{
		string url1 = "";
		if (string.IsNullOrEmpty(url.ToString()) || url == DBNull.Value) {
			url1 = "../Images/No_image.png";
		}
		else
		{
			url1 = string.Format("../{0}", url);
		}
		return url1;	
	}
}