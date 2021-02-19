using ApiCore.Api.ViewModel.User;
using ApiCore.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiCore.Api.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(INotificador notificador,
                                SignInManager<IdentityUser> signInManager,
                                UserManager<IdentityUser> userManager) : base(notificador)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult> RegistrarAsync(RegisterUserViewModel registrerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registrerUser.Email,
                Email = registrerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registrerUser.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return CustomResponse(registrerUser);
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(registrerUser);
        }

        [HttpPost("logar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                return CustomResponse(loginUser);
            }

            if (result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas invalidas.");
                return CustomResponse(loginUser);
            }

            NotificarErro("Usuário ou senha incorretos.");
            return CustomResponse(loginUser);
        }
    }
}