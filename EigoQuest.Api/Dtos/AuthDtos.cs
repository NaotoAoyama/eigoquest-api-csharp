using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EigoQuest.Api.Dtos
{
    public record RegisterRequestDto(
        [Required] string Username,
        [Required] string Password,
        // ▼▼▼ 修正 ▼▼▼
        [property: JsonPropertyName("password_confirm")] [Required] string PasswordConfirm
    );

    public record LoginRequestDto(
        [Required] string Username,
        [Required] string Password
    );

    public record AuthResponseDto(
        string AccessToken,
        string RefreshToken
    );
}