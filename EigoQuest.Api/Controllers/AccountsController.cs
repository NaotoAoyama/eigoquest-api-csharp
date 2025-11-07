using EigoQuest.Api.Dtos;
using EigoQuest.Api.Models;
using EigoQuest.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EigoQuest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // -> /api/accounts
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;

        public AccountsController(UserManager<ApplicationUser> userManager, JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        // Django: POST /accounts/api/signup/ の換装
        [HttpPost("register")] // -> /api/accounts/register
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            // Django: SignUpSerializer.validate
            if (request.Password != request.PasswordConfirm)
            {
                return BadRequest(new { password_confirm = "パスワードが一致しません。" });
            }

            var user = new ApplicationUser { UserName = request.Username, Email = request.Username };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "登録が完了しました。" });
            }

            // Django: "このユーザー名は既に使用されています。"
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { username = errors });
        }

        // Django: POST /api/token/ (TokenObtainPairView) の換装
        [HttpPost("login")] // -> /api/accounts/login
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                // 401 Unauthorized
                return Unauthorized(new { detail = "ユーザー名またはパスワードが正しくありません。" });
            }

            var accessToken = _jwtService.CreateAccessToken(user);
            var refreshToken = _jwtService.CreateRefreshToken(user);

            // VueのauthStore が期待するレスポンス
            return Ok(new AuthResponseDto(
                AccessToken: accessToken,
                RefreshToken: refreshToken
            ));
        }
    }
}