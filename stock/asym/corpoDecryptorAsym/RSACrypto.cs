namespace corpoDecryptoAsym;

using System;
using System.Security.Cryptography;
using System.Text;

public class RSACrypto
{
    // La clé privée n'est PAS dans le code: elle est lue dans un fichier présent
    // uniquement sur le poste sécurisé de l'entreprise
    public const string FichierClePrivee = "cle-privee.xml";
    public const string FichierClePublique = "cle-publique.xml";

    private readonly string _privateKeyXml;

    public RSACrypto(string privateKeyXml)
    {
        _privateKeyXml = privateKeyXml;
    }

    // Génère une nouvelle paire de clés RSA 2048 bits dans le dossier donné
    public static void GenerateKeys(string dossier)
    {
        using (var rsa = RSA.Create(2048))
        {
            File.WriteAllText(Path.Combine(dossier, FichierClePublique), rsa.ToXmlString(false));
            File.WriteAllText(Path.Combine(dossier, FichierClePrivee), rsa.ToXmlString(true));
        }
    }

    public string Decrypt(string encryptedData)
    {
        byte[] dataToDecrypt = Convert.FromBase64String(encryptedData);
        byte[] decryptedData;
        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(_privateKeyXml);
            decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256);
        }
        return Encoding.UTF8.GetString(decryptedData);
    }
}
