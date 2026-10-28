using corpoDecryptor;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the path of the file to decrypt:");
        string path = Console.ReadLine();
        string[] lines = System.IO.File.ReadAllLines(path);
        foreach (string line in lines)
        {
            AesCrypto aes = new AesCrypto();
            string first = line.Split(" @ ")[0];
            string decrypted = aes.Decrypt(first);
            Console.WriteLine(decrypted + " @ " + line.Split(" @ ")[1]);
        }
        
    }
}