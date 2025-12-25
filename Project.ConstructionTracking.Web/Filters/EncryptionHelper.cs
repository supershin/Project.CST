using Project.ConstructionTracking.Web.Commons;
using System.Security.Cryptography;
using System.Text;

namespace Project.ConstructionTracking.Web.Filters
{
    public static class EncryptionHelper
    {
        static EncryptionHelper()
        {
        }

        public static string EncryptParam(string key, string iv, string param)
        {
            byte[] Key = ConvertHexStringToBytes(key);
            byte[] IV = ConvertHexStringToBytes(iv);
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                {
                    var plainBytes = Encoding.UTF8.GetBytes(param);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return ToHexString(encryptedBytes);
                }
            }
        }

        public static string DecryptParam(string key,string iv,string encryptedHex)
        {
            byte[] Key = ConvertHexStringToBytes(key);
            byte[] IV = ConvertHexStringToBytes(iv);

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                {
                    var encryptedBytes = FromHexString(encryptedHex);
                    var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }

        }
        public static byte[] FromHexString(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        public static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
        private static byte[] ConvertHexStringToBytes(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
