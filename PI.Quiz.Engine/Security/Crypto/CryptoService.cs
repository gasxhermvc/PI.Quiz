using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace PI.Quiz.Engine.Security.Crypto
{
    public class CryptoService : ICryptoService
    {
        private readonly string _secretKey;

        private readonly IConfiguration _configuration;

        public CryptoService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _secretKey = _configuration["WebServiceSettings:Crypt:SecretKey"] ?? throw new ArgumentNullException(nameof(_secretKey));
        }

        public string Decrypt(string cipherText)
        {
            dynamic payload = this.GetJsonPayload(cipherText);

            RijndaelManaged aes = new RijndaelManaged();

            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            aes.Key = Encoding.UTF8.GetBytes(this._secretKey);
            aes.IV = Convert.FromBase64String(payload["iv"]);

            ICryptoTransform AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] buffer = Convert.FromBase64String(payload["value"]);

            return (Encoding.UTF8.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length))).ToString();
        }

        public string Encrypt(string plainText)
        {
            RijndaelManaged aes = new RijndaelManaged();

            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            aes.Key = Encoding.UTF8.GetBytes(_secretKey);
            aes.GenerateIV();

            ICryptoTransform AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] buffer = Encoding.UTF8.GetBytes(plainText);

            string encryptedText = Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));

            string mac = string.Empty;

            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
            {
                hmacsha256.ComputeHash(Encoding.Default.GetBytes(Convert.ToBase64String(aes.IV) + encryptedText));
                mac = this.ByteToString(hmacsha256.Hash);
            }

            return JsonPayload(aes.IV, encryptedText, mac);
        }

        private Dictionary<string, string> GetJsonPayload(string encryptedText)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(encryptedText)));
        }

        private string JsonPayload(byte[] iv, string encryptedText, string mac)
        {
            var keyValues = new Dictionary<string, object>
                {
                    { "iv", Convert.ToBase64String(iv) },
                    { "value", encryptedText },
                    { "mac", mac },
                };

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(keyValues)));
        }

        private string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("x2"); /* hex format */
            return sbinary;

        }
    }
}
