using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ServerAPIs.Authentication.Common
{
    internal class AesUtilities
    {
        private static string _privateKey;
        private static string _vector;

        private static ICryptoTransform _cryptoTransform;

        public AesUtilities(IConfiguration configuration)
        {
            _privateKey = configuration["AES:PrivateKey"];
            _vector = configuration["AES:Vector"];

            var bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(_privateKey.PadRight(bKey.Length)), bKey, bKey.Length);

            var bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(_vector.PadRight(bVector.Length)), bVector, bVector.Length);

            var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            _cryptoTransform = aes.CreateDecryptor(bKey, bVector);
        }

        public static string Decrypt(string ciphertext)
        {
            var encryptedBytes = Convert.FromBase64String(ciphertext);

            using var memory = new MemoryStream(encryptedBytes);

            using var Decryptor = new CryptoStream(memory, _cryptoTransform, CryptoStreamMode.Read);

            using var originalMemory = new MemoryStream();

            var buffer = new byte[1024];
            int readBytes = 0;

            while ((readBytes = Decryptor.Read(buffer, 0, buffer.Length)) > 0)
            {
                originalMemory.Write(buffer, 0, readBytes);
            }

            var original = originalMemory.ToArray();

            return Encoding.UTF8.GetString(original);
        }
    }
}
