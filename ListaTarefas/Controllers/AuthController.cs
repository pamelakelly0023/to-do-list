using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ListaTarefas.Controllers;
using ListaTarefas.Models;
using ListaTarefas.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ListaTarefas.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public AuthController( 
                              SignInManager<IdentityUser> signInManager, 
                              UserManager<IdentityUser> userManager, 
                              IOptions<AppSettings> appSettings,
                              ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> Registrar (RegistrarUsuarioModel registrarUser)
        {
            var userExist = await _userManager.FindByEmailAsync(registrarUser.Email);

            if (userExist is not null)
                return StatusCode(
                  StatusCodes.Status500InternalServerError,
                  new ResponseModel { Success = false, Message = "Usuário já Existe!" }  
                );

            IdentityUser user = new()
            {
                UserName = registrarUser.Email,
                Email = registrarUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registrarUser.Password);

            if(!result.Succeeded)
                return StatusCode (
                    StatusCodes.Status500InternalServerError,
                    new ResponseModel { Success = false, Message = "Erro ao criar usuário" }
                );

            return Ok(new ResponseModel { Message = "Usuário criado com Sucesso!"});
            
        }

        // public async Task<ActionResult> Entrar(LoginUsuarioModel loginUser )
        // {
        //     var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        //     if(result.Succeeded)
        //     {
        //          _logger.LogInformation("Usuario: {user} logado com sucesso:", loginUser.Email);
        //          return Ok(GerarTokenJWT());
        //     }
        //     if (result.IsLockedOut)
        //     {
        //         // Usuário temporariamente bloqueado por tentativas inválidas
        //         // TODO: Criar esse retorno de erro customizado. 
        //     }

        //     return 

        // }

        [HttpGet]
        private string GerarTokenJWT()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });
            
            _logger.LogInformation("Token criado: {token} ", token );
            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }
    }

}