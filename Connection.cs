using System;
using System.Collections.Generic;
using System.Configuration; // Fixed typo: 'Congfiguration'
using System.Linq;
using System.Web;

namespace FoodShop
{
    public class Connection
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        }
    }

    public class Utils
    {
        public static bool IsValidExtension(string fileName)
        {
            bool isValid = false;
            string[] fileExtension = { ".jpg", ".png", ".jpeg" }; // Fixed typo: 'srting'
            for (int i = 0; i <= fileExtension.Length - 1; i++)
            {
                if (fileName.Contains(fileExtension[i])) // Fixed missing parenthesis
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }
    }

    //public static string GetImageUrl(object url)
    //{
    //    string url1 = "";
    //    if (string.IsNullOrEmpty(url.ToString()) || url == DBNull.Value)
    //    {
    //        url1 = "../Images/No_image.png";
    //    }
    //    else
    //    {
    //        url1 = string.Format("../{0}", url);
    //    }
    //    return url1;
    //}
}
