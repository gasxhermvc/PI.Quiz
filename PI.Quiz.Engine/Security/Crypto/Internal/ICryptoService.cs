namespace PI.Quiz.Engine.Security.Crypto
{
    public interface ICryptoService
    {
        //=>ถอดรหัส
        string Decrypt(string cipherText);

        //=>เข้ารหัส
        string Encrypt(string plainText);
    }
}
