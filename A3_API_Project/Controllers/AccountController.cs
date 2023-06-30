using A3_API_Project.Interfaces;
using A3_API_Project.Models;
using A3_API_Project.Models.IDP;
using A3_API_Project.Models.IDP.DTO;
using A3_API_Project.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace A3_API_Project.Controllers
{
    //[Authorize]
    //[Route("api/[controller]")]
    //[ApiController]
    //public class AccountController : ControllerBase
    //{
    //    private readonly UserManager<ApplicationUser> _userManager;
    //    private readonly SignInManager<ApplicationUser> _signInManager;
    //    private readonly ILogger _logger;
    //    private readonly IEmailSender _emailSender;

    //    [TempData]
    //    public string ErrorMessage { get; set; }

    //    public AccountController(UserManager<ApplicationUser> userManager,
    //        SignInManager<ApplicationUser> signInManager,
    //        ILogger<AccountController> logger,
    //        IEmailSender emailSender)
    //    {
    //        _userManager = userManager;
    //        _signInManager = signInManager;
    //        _logger = logger;
    //        _emailSender = emailSender;
    //    }

    //    [HttpPost("Login")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Login(LoginDTO model, string returnUrl = null)
    //    {

    //        // Isso não conta falhas de login para bloqueio de conta
    //        // Para habilitar falhas de senha para acionar o bloqueio de conta, defina lockoutOnFailure: true
    //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

    //        // caso seja necessário validar se o email/link foi confirmado
    //        var user = await _userManager.FindByEmailAsync(model.Email);
    //        var isEmailConfirmado = await _userManager.IsEmailConfirmedAsync(user);

    //        if (result.Succeeded)
    //        {
    //            _logger.LogInformation("Usuário logado. Aguarde, gerando token...");

    //            IList<Claim> permissoes = await _userManager.GetClaimsAsync(user);

    //            // gera token de acordo com as permissões
    //            int tempoExpiracaoToken = 6;
    //            byte[] secretKey = Encoding.ASCII.GetBytes("MINHA_CHAVE_SECRETA_AQUI");
    //            DateTime dataAtual = DateTime.Now;
    //            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
    //            {
    //                Subject = new ClaimsIdentity(permissoes),
    //                Expires = dataAtual.AddHours(tempoExpiracaoToken),
    //                NotBefore = dataAtual,
    //                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha512Signature),
    //            };

    //            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    //            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
    //            string tokenString = tokenHandler.WriteToken(token);

    //            // cria um objeto de retorno amigavel - mostrando se o e-mail foi confirmado
    //            var retorno = isEmailConfirmado 
    //                ? new { Status = $"Usuário logado!", IsEmailConfirmado = true, Token = tokenString } 
    //                : new { Status = $"Usuário logado!", IsEmailConfirmado = false, Token = tokenString };

    //            Token tok = new Token()
    //            {
    //                AccessToken = tokenString,
    //                ExpiresIn = (int)(tokenDescriptor.Expires.Value - dataAtual).TotalSeconds,
    //                TokenType = "Bearer"
    //            };

    //            return Ok(tok);
    //        }
    //        if (result.RequiresTwoFactor)
    //        {
    //            return BadRequest("Requer 2 fatores");
    //        }
    //        if (result.IsLockedOut)
    //        {
    //            _logger.LogWarning("Conta de usuário bloqueada.");
    //            return BadRequest("Conta de usuário bloqueada");
    //        }
    //        else
    //        {
    //            return BadRequest("Tentativa de login inválida...");
    //        }

    //    }

    //    [HttpGet("Logout")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Logout(string returnUrl = null)
    //    {
    //        // Clear the existing external cookie to ensure a clean login process
    //        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

    //        return Ok();
    //    }

    //    /// <summary>
    //    /// Registra um novo usuário e gera um link de confirmação de e-mail.
    //    /// </summary>
    //    /// <returns>Exemplo returns</returns>
    //    /// <response code="200">Retorna um link para confirmação de e-mail.</response>
    //    [HttpPost("Register")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Register(RegisterDTO model, string returnUrl = null)
    //    {

    //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
    //        var result = await _userManager.CreateAsync(user, model.Password);
    //        if (result.Succeeded)
    //        {
    //            _logger.LogInformation("User created a new account with password.");

    //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

    //            // gera link para confirmação de conta
    //            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

    //            // envia e-mail de confirmação de conta
    //            await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

    //            await _signInManager.SignInAsync(user, isPersistent: false);
    //            _logger.LogInformation("Conta criada com sucesso. Verifique seu e-mail para ativar sua conta através do link...");

    //            var retorno = new
    //            {
    //                Descricao = "Conta criada com sucesso!",
    //                Id = user.Id,
    //                TokenConfirmacaoEmail = code,
    //                Instrucao = $"Caso desejar simular a confirmação de e-mail, copie o link para o navegador ou acesse o endpoint '{nameof(ConfirmEmail)}' com o ID '{user.Id}' e o token.",
    //                LinkConfirmacaoEmail = callbackUrl
    //            };

    //            return Ok(retorno);
    //        }
    //        else
    //        {
    //            string jsonErroIdentity = JsonConvert.SerializeObject(result.Errors, Formatting.Indented);
    //            return BadRequest($"Erro ao tentar registrar:\n{ jsonErroIdentity }");
    //        }

    //    }


    //    [HttpGet("ConfirmEmail")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    //    {
    //        if (userId == null || code == null)
    //        {
    //            return BadRequest($"Informações inválidas...");
    //        }

    //        var user = await _userManager.FindByIdAsync(userId);
    //        if (user == null)
    //        {
    //            throw new ApplicationException($"Não foi possível carregar o usuário com ID '{userId}'.");
    //        }

    //        var result = await _userManager.ConfirmEmailAsync(user, code);

    //        if (result.Succeeded)
    //        {

    //            return base.Content("<h1>E-mail confirmado com sucesso!</h1>", "text/html");
    //        }
    //        else
    //        {
    //            string jsonErroIdentity = JsonConvert.SerializeObject(result.Errors, Formatting.Indented);
    //            return BadRequest($"Erro ao confirmar e-mail:\n{ jsonErroIdentity }");
    //        }
    //    }

    //    [HttpPost("ForgotPassword")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
    //    {

    //        var user = await _userManager.FindByEmailAsync(model.Email);
    //        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
    //        {
    //            // Não revelar se o usuário existe ou se o email foi confirmado
    //            // Esta sendo ilustrado somentes nos testes da API. Em ambiente real, tratar de outra forma...
    //            return BadRequest("Tente novamente. (Esqueci minha senha)");
    //        }

    //        // For more information on how to enable account confirmation and password reset please
    //        // visit https://go.microsoft.com/fwlink/?LinkID=532713
    //        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
    //        var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
    //        await _emailSender.SendEmailAsync(model.Email, "Reset Password",
    //           $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

    //        string retorno = $"Usuário encontrado. Para continuar, acesse o endpoint '{nameof(ResetPassword) }', digite a nova senha e insira chave:\n{code}";

    //        return Ok(retorno);

    //    }

    //    [HttpPost("ResetPassword")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
    //    {

    //        var user = await _userManager.FindByEmailAsync(model.Email);

    //        // Não revelar que o usuário não existe retornando um erro, direcionar direto para a tela de confirmar reset password
    //        // retornar OK e redireciona-lo para tela de ResetPassword novamente
    //        if (user == null) return BadRequest("Tente novamente. (Resetar senha)");

    //        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
    //        if (result.Succeeded) return Ok("Sucesso ao resetar password... faça login com a nova senha!");

    //        // caso chegue aqui, algo deu errado
    //        string jsonErroIdentity = JsonConvert.SerializeObject(result.Errors, Formatting.Indented);
    //        return BadRequest($"Erro ao resetar a senha:\n{ jsonErroIdentity }");
    //    }

    //}
}
