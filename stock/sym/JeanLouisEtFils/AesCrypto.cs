using System.Security.Cryptography;
using System.Text;

namespace JeanLouisEtFils;

public class AesCrypto
{
    private static string key = "12341234123412341234123412341234";

    public string Encrypt(string source)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV();                   // générer un IV pour chaque chiffrement
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(source);
                    cs.Write(inputBytes, 0, inputBytes.Length);
                    cs.FlushFinalBlock();
                }
                byte[] encrypted = ms.ToArray();
                byte[] result = new byte[aes.IV.Length + encrypted.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);                     // on concatène l'IV au début du tableau
                Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);   // puis les données chiffrées
                return Convert.ToBase64String(result);
            }
        }
    }
}
