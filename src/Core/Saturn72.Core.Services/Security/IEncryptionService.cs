namespace Saturn72.Core.Services.Security
{
    public interface IEncryptionService
    {
        string DecryptText(string cipherText, string encryptionPrivateKey = "");

        string EncryptText(string plainText, string encryptionPrivateKey = "");

        string CreateSaltKey(int size);

        string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1");

        string CreateHash(byte[] data, string hashAlgorithm = "SHA1");
    }
}