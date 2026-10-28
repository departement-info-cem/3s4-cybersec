namespace JeanLouisEtFilsAsym;

using System;
using System.Security.Cryptography;
using System.Text;

public class RSACrypto
{
    // Seule la clé publique est dans l'application: elle sert à chiffrer, pas à déchiffrer
    private string _publicKeyXml =
        "<RSAKeyValue><Modulus>s/uKU8FHBXQjkoZeuQzseEwGHUM2zKCkH+tGTJVZN534Rr3KKl5j4WMfTTamH5lsfBGJkoSKmODpOkNgQ/HD+h4HDfW0VB0HMzSIV3uanqnJ9W7moEF9K2xTQlu+cMlY75uUjf+OcWCnRvOtH/cjLybvz5A8hfz1zgjqifTix4ZEW+4oI+3vKd3Y47LLpBP9DVFwR3qfmtggwTL2GA7OcF0F1gOsDx/0Lj6xYgG0AI0NLI4LyuiKVShLlookZC9VC4dxrctYyoFwo+Qm0JtbuRRT6lcp/0CwprB8uI2/9nIFm+XUU8FD6N2tq+8RpdrVO11ZPOV5tlAP+h7XHDWR2w==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    public string Encrypt(string data)
    {
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
        byte[] encryptedData;
        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(_publicKeyXml);
            encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
        }
        return Convert.ToBase64String(encryptedData);
    }

}
