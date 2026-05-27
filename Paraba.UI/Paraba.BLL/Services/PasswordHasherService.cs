using System.Security.Cryptography;

namespace Paraba.BLL.Services
{
    public class PasswordHasherService
    {
        private const int Iterations = 100000;

        public (string Hash, string Salt, int Iterations) CrearHash(string password)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);

            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256,
                32);

            return (
                Convert.ToBase64String(hashBytes),
                Convert.ToBase64String(saltBytes),
                Iterations);
        }

        public bool VerificarPassword(string password, string passwordHash, string passwordSalt, int iterations)
        {
            byte[] saltBytes = Convert.FromBase64String(passwordSalt);
            byte[] expectedHash = Convert.FromBase64String(passwordHash);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
