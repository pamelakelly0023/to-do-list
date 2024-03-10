using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ListaTarefas.Models;
using ListaTarefas.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;

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
        private readonly IValidator<UsuarioModel> _validator;


        public AuthController( 
                              SignInManager<IdentityUser> signInManager, 
                              UserManager<IdentityUser> userManager, 
                              IOptions<AppSettings> appSettings,
                              ILogger<AuthController> logger,
                              IValidator<UsuarioModel> validator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _appSettings = appSettings.Value;
            _validator = validator;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult> Registrar (UsuarioModel registrarUser)
        {
            var validation = await _validator.ValidateAsync(registrarUser);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors);
            }

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

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(UsuarioModel loginUser)
        {
            var validation = await _validator.ValidateAsync(loginUser);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors);
            }

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if(!result.Succeeded)
                return StatusCode (
                    StatusCodes.Status500InternalServerError,
                    new ResponseModel { Success = false, Message = "Email ou senha incorretos" }
                );
            
            //  return Ok(new ResponseModel { Message = "Usuário logado!"});
            return Ok( new ResponseModel { Data = GerarJwt()});
        }

        // Criar token model
        [HttpGet("token")]
        private TokenModel GerarJwt()
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
            
            return new()
            {
                // _logger.LogInformation("Token criado: {token} ", token ),
                Token = tokenHandler.WriteToken(token),
                ValidTo = token.ValidTo
            };
            
        }
    }

}