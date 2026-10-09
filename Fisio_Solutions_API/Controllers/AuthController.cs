using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Fisio_Solutions_API.Data;
using Fisio_Solutions_API.DTOs;
using Fisio_Solutions_API.Models;
using Fisio_Solutions_API.Services;

namespace Fisio_Solutions_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AppDbContext context,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// US02: Registro de Conta de Usuário (Autenticação)
        /// Validação de formato de e-mail e conferência obrigatória de confirmação de senha.
        /// Validação para impedir criação de contas com e-mails já existentes.
        /// Persistência dos dados através da API C# .NET criptografando a senha em hash seguro.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Verifica se o e-mail já existe
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (existingUser != null)
            {
                return Conflict(new { message = "Este e-mail já está cadastrado no sistema." });
            }

            // Criptografa a senha com hash seguro (PBKDF2 SHA-256 + Salt)
            var (hash, salt) = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName.Trim(),
                BirthDate = request.BirthDate.Trim(),
                Email = normalizedEmail,
                Profession = request.Profession.Trim(),
                PasswordHash = hash,
                PasswordSalt = salt,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Gera o token JWT com validade temporária
            var (token, expiration) = _tokenService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Expiration = expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Profession = user.Profession
                }
            };

            return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, response);
        }

        /// <summary>
        /// US03: Autenticação de Usuário e Gestão de Sessão (Login / Autenticação)
        /// Retorno de token JWT criptografado com validade temporária e canal HTTPS.
        /// Notificação clara de erro em caso de credenciais inválidas.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas. Verifique seu e-mail e senha." });
            }

            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Credenciais inválidas. Verifique seu e-mail e senha." });
            }

            var (token, expiration) = _tokenService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Expiration = expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Profession = user.Profession
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// US03: Login com conta Google
        /// </summary>
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail || u.GoogleId == request.GoogleId);

            if (user == null)
            {
                // Cria conta automática para primeiro login Google
                var (hash, salt) = _passwordHasher.HashPassword(Guid.NewGuid().ToString("N"));
                user = new User
                {
                    FullName = string.IsNullOrWhiteSpace(request.FullName) ? "Usuário Google" : request.FullName.Trim(),
                    Email = normalizedEmail,
                    GoogleId = request.GoogleId,
                    BirthDate = "",
                    Profession = "Paciente",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                user.GoogleId = request.GoogleId;
                await _context.SaveChangesAsync();
            }

            var (token, expiration) = _tokenService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Expiration = expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Profession = user.Profession
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// US03: Disparo de link temporário de redefinição de senha ("Esqueceu sua senha?")
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user != null)
            {
                // Gera token temporário de redefinição (validade de 1 hora)
                user.ResetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Token de redefinição gerado para {Email}: {Token}", user.Email, user.ResetToken);
            }

            // Sempre retorna resposta neutra/sucesso por segurança (não enumeração de usuários)
            return Ok(new MessageResponse
            {
                Success = true,
                Message = "Se o e-mail estiver cadastrado em nosso sistema, um link temporário para redefinição de senha foi enviado com sucesso."
            });
        }

        /// <summary>
        /// Retorna perfil do usuário logado via Bearer Token
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Profession = user.Profession
            });
        }
    }
}

