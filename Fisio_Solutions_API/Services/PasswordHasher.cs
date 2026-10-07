using System.Security.Cryptography;

namespace Fisio_Solutions_API.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        public (string Hash, string Salt) HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithm,
                KeySize
            );

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            try
            {
                byte[] salt = Convert.FromBase64String(storedSalt);
                byte[] expectedHash = Convert.FromBase64String(storedHash);

                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithm,
                    KeySize
                );

                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}

