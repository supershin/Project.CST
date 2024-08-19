using System;
namespace Project.ConstructionTracking.Web.Commons
{
	public static class HashExtension
	{
        public static string DecodeFrom64(this string encryptData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encryptData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}

