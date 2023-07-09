using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZeiJakuSei.Modules
{
    internal static class ModCrypto
    {
        /// <summary>
        /// 用给定的RSA公钥加密纯文本
        /// </summary>
        /// <param name="plainText">要加密的文本</param>
        /// <param name="publicXmlKey">用于加密的公钥.</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        public static string EncryptRsa(string plainText, string publicXmlKey)
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            try
            {
                //加载公钥
                rsa.FromXmlString(publicXmlKey);

                var bytesToEncrypt = Encoding.Unicode.GetBytes(plainText);

                var bytesEncrypted = rsa.Encrypt(bytesToEncrypt, false);

                return Convert.ToBase64String(bytesEncrypted);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        /// <summary>
        /// 用给定的RSA公钥加密bytes
        /// </summary>
        /// <param name="bytes">要加密的bytes</param>
        /// <param name="publicXmlKey">用于加密的公钥.</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        public static byte[] EncryptRsaToBytes(byte[] bytes, string publicXmlKey)
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            try
            {
                //加载公钥
                rsa.FromXmlString(publicXmlKey);

                return rsa.Encrypt(bytes, false);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        /// <summary>
        /// 给定的RSA私钥解密纯文本
        /// </summary>
        /// <param name="encryptedText">加密的密文</param>
        /// <param name="privateXmlKey">用于加密的私钥.</param>
        /// <returns>未加密数据的字符串</returns>
        public static string DecryptRsa(string encryptedText, string privateXmlKey)
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            try
            {
                rsa.FromXmlString(privateXmlKey);

                var bytesEncrypted = Convert.FromBase64String(encryptedText);

                var bytesPlainText = rsa.Decrypt(bytesEncrypted, false);

                return Encoding.Unicode.GetString(bytesPlainText);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        /// <summary>
        /// 给定的RSA私钥解密bytes
        /// </summary>
        /// <param name="bytes">加密的bytes</param>
        /// <param name="privateXmlKey">用于加密的私钥.</param>
        /// <returns>未加密数据的字符串</returns>
        public static byte[] DecryptRsaFromBytes(byte[] bytes, string privateXmlKey)
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            try
            {
                rsa.FromXmlString(privateXmlKey);

                var bytesPlainText = rsa.Decrypt(bytes, false);

                return bytesPlainText;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        /// <summary>
        /// 使用给定的 AES 密钥加密纯文本
        /// </summary>
        /// <param name="plainText">要加密的文本</param>
        /// <param name="key">AES 密钥</param>
        /// <returns>加密后的 Base64 编码字符串</returns>
        public static string EncryptAES(string plainText, string key)
        {
            byte[] encryptedBytes;
            byte[] iv;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encryptedBytes = msEncrypt.ToArray();
            }

            byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
            Array.Copy(iv, 0, combinedBytes, 0, iv.Length);
            Array.Copy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }

        /// <summary>
        /// 使用给定的 AES 密钥解密文本
        /// </summary>
        /// <param name="encryptedText">加密的密文</param>
        /// <param name="key">AES 密钥</param>
        /// <returns>解密后的文本</returns>
        public static string DecryptAES(string encryptedText, string key)
        {
            byte[] combinedBytes = Convert.FromBase64String(encryptedText);

            byte[] iv = new byte[16]; // Assuming 128-bit IV size
            byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];
            Array.Copy(combinedBytes, 0, iv, 0, iv.Length);
            Array.Copy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            string decryptedText;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new(encryptedBytes);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                decryptedText = srDecrypt.ReadToEnd();
            }

            return decryptedText;
        }

        public static string EncryptAES(string plainText, byte[] key)
        {
            byte[] encryptedBytes;
            byte[] iv;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encryptedBytes = msEncrypt.ToArray();
            }

            byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
            Array.Copy(iv, 0, combinedBytes, 0, iv.Length);
            Array.Copy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }

        public static string DecryptAES(string encryptedText, byte[] key)
        {
            byte[] combinedBytes = Convert.FromBase64String(encryptedText);

            byte[] iv = new byte[16]; // Assuming 128-bit IV size
            byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];
            Array.Copy(combinedBytes, 0, iv, 0, iv.Length);
            Array.Copy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            string decryptedText;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new(encryptedBytes);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                decryptedText = srDecrypt.ReadToEnd();
            }

            return decryptedText;
        }


        /// <summary>
        /// 生成用作 AES 加密的随机密钥
        /// </summary>
        /// <returns>随机密钥</returns>
        public static byte[] GenerateAesKey()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            return aes.Key;
        }

        /// <summary>
        /// 将字节数组转换为十六进制字符串表示形式
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
