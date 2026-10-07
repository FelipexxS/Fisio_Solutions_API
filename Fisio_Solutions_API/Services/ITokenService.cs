using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Services
{
    public interface ITokenService
    {
        (string Token, DateTime Expiration) GenerateToken(User user);
    }
}

