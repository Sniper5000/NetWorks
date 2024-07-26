using System.Text;
using NetWorks.Security;

public class SecurityTests
{
    private readonly SecurityKeypair keypair = new();

    private static void Log(string text)
    {
        Console.WriteLine($"[MAIN][SECURITY]: {text}");
    }

    public void Run()
    {
        Log("Initializing Server Security Module...");
        Log("Server Security Module Initialized!");
        Log("Public Key: " + keypair.PublicKey);
        Log("Performing tests...");

        TestSecurity();
    }

    public void TestSecurity()
    {
        Log("Testing security encryption and decrpytion");

        string testMessage = "Testing Security Modules.";
        Log("Initial message: " + testMessage);
        
        TestRsa(testMessage);
        TestAedm(testMessage);
    }

    private void TestRsa(string testMessage)
    {
        Log("Testing RSA encryption");

        byte[] testBytes = Encoding.ASCII.GetBytes(testMessage);
        byte[] encrypted = RsaEncryption.Encrypt(keypair.PublicKey, testBytes);
        byte[] decrypted = RsaEncryption.Decrypt(keypair.PrivateKey, encrypted);

        Log("Encrypted message: " + Convert.ToHexString(encrypted));
        Log("Decrypted message: " + Encoding.ASCII.GetString(decrypted));
    }

    private void TestAedm(string testMessage)
    {
        Log("Testing AEDM encryption");

        byte[] testBytes = Encoding.ASCII.GetBytes(testMessage);
        byte[] securedData = AedmEncryption.Encrypt(keypair.PublicKey, testBytes);
        byte[] decrypted = AedmEncryption.Decrypt(keypair.PrivateKey, securedData);

        Log("Secured data: " + Convert.ToHexString(securedData));
        Log("Decrypted message: " + Encoding.ASCII.GetString(decrypted));
    }
}