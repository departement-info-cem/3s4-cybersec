using System.Security.Cryptography;
using System.Text;

namespace corpoDecryptor;

public class AesCrypto
{
    private static string key = "12341234123412341234123412341234";
    private static byte[] iv = Convert.FromHexString("12341234123412341234123412341234");

    public string Decrypt(string source)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] fullCipher = Convert.FromBase64String(source);
            int ivLen = aes.BlockSize / 8;
            byte[] ivBytes = new byte[ivLen];
            byte[] cipherBytes = new byte[fullCipher.Length - ivLen];
            Buffer.BlockCopy(fullCipher, 0, ivBytes, 0, ivLen);
            Buffer.BlockCopy(fullCipher, ivLen, cipherBytes, 0, cipherBytes.Length);
            Console.WriteLine($"Vecteur d'injection: {Convert.ToHexString(ivBytes)}");
            Console.WriteLine($"Données chiffrées: {Convert.ToHexString(cipherBytes)}");

            aes.IV = ivBytes;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
