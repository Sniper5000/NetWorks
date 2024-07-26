namespace NetWorks.Security;

using System.Security.Cryptography;
using System.Text;

// /!/ DO NOT SHARE THE PRIVATE KEY WITH CLIENTS /!/
/// <summary>
/// Class holding Public & Private <see cref="RSA"/> keys
/// </summary>
public class SecurityKeypair
{
    public SecurityKey PublicKey { get; private set; }
    public SecurityKey PrivateKey { get; private set; }

    public SecurityKeypair(int keySize = 2048)
    {
        RSACryptoServiceProvider rsa = new(keySize);
        PublicKey = new(rsa.ToXmlString(false));
        PrivateKey = new(rsa.ToXmlString(true));
    }
}

public class SecurityKey
{
    private readonly string xmlString;

    public SecurityKey(string xmlString)
    {
        this.xmlString = xmlString;
    }

    public string XmlString => xmlString;
    public byte[] Bytes => Encoding.ASCII.GetBytes(xmlString);

    public static SecurityKey FromXmlString(string xmlString)
    {
        return new SecurityKey(xmlString);
    }

    public static SecurityKey FromBytes(byte[] bytes)
    {
        string xmlString = Encoding.ASCII.GetString(bytes);
        return new SecurityKey(xmlString);
    }
}