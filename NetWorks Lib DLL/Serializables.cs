

using MemoryPack;

namespace NetWorks.Serializables
{

    //Secured with RSA Encryption

    [Serializable, MemoryPackable]
    public partial class SecuredData
    {
        public byte[] AesKey; //RSA Encrypted
        public byte[] AesIv; //RSA Encrypted
        public byte[] Encrypted;

        public SecuredData(byte[] aesKey, byte[] aesIv, byte[] encrypted)
        {
            AesKey = aesKey;
            AesIv = aesIv;
            Encrypted = encrypted;
        }
    }




}