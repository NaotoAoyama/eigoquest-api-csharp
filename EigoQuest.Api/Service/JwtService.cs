using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EigoQuest.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace EigoQuest.Api.Services
{
    // トークン発行専門のクラス
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _jwtKey;

        public JwtService(IConfiguration config)
        {
            _config = config;
            // appsettings.json の "Jwt:Key" を読み込む
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        }

        public string CreateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id), // ユーザーID
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Name, user.UserName!)
            };

            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), // Django: ACCESS_TOKEN_LIFETIME
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken(ApplicationUser user)
        {
            // C#のIdentityにはSimpleJWTのようなリフレッシュトークンDB管理機能は
            // 標準搭載されていません。
            // ここでは簡易的に、有効期限の長いアクセストークンを
            // リフレッシュトークンとして代用します。（本番運用ではDB管理を推奨）
            
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id)
            };
            
            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1), // Django: REFRESH_TOKEN_LIFETIME
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}