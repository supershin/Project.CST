using System;
using System.Security.Cryptography;
using System.Text;

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

        public static string DecryptMD5(string input, string hash)
        {
            byte[] data = Convert.FromBase64String(input);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();

            tripleDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            tripleDES.Mode = CipherMode.ECB;

            ICryptoTransform transform = tripleDES.CreateDecryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

            return UTF8Encoding.UTF8.GetString(result);
        }

        public static string EncryptMD5(string input, string hash)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(input);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();

            tripleDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            tripleDES.Mode = CipherMode.ECB;

            ICryptoTransform transform = tripleDES.CreateEncryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

            return Convert.ToBase64String(result);
        }

        public static string GenerateApproveNumber(int count, string mainString)
        {
            int newCount = count + 1;

            string approveString = newCount.ToString("D5");

            string year = DateTime.Now.ToString("yy");
            string month = DateTime.Now.ToString("MM");

            string result = mainString + '-' + month + year + '-' + approveString;
            return result;
        }
    }
}

