using System.Security.Cryptography;
using System.Text;
using Encryption.Blowfish;

namespace JeanLouisEtFils;

public class BlowFishCrypto
{
    
    private static string key = "a3bd614b27864e3f854b971f9df1a802";
    private static byte[] iv = new byte[]{23, 56, 45, 67, 78, 89, 90, 12};

    public String Encrypt(String source)
    {
        byte[] buf = Encoding.UTF8.GetBytes(source);
        buf = buf.CopyAndPadIfNotAlreadyPadded();
        var cbc = new BlowfishCtr(key);
        var ok = cbc.CryptOrDecrypt(buf, iv);
        String encrypted = Convert.ToBase64String(buf);
        return encrypted;
    }

    public String Decrypt(String source)
    {
        var cbc = new BlowfishCtr(key);
        byte[] buf = Convert.FromBase64String(source);
        var ok = cbc.CryptOrDecrypt(buf, iv);
        String decrypted = Encoding.UTF8.GetString(buf);
        return decrypted;
    }
}