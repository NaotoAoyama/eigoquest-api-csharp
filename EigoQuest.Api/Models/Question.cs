using System.ComponentModel.DataAnnotations;

namespace EigoQuest.Api.Models
{
    public class Question
    {
        public long Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty; // 修正

        [Required]
        public string OptionA { get; set; } = string.Empty; // 修正

        [Required]
        public string OptionB { get; set; } = string.Empty; // 修正

        [Required]
        public string OptionC { get; set; } = string.Empty; // 修正 (警告の箇所)

        [Required]
        public string OptionD { get; set; } = string.Empty; // 修正

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty; // 修正

        // Explanationは 'string?' なので null 許容。修正不要。
        public string? Explanation { get; set; }

        // Part は初期値があるので修正不要
        public string Part { get; set; } = "PART5";

        // その他（int, DateTime）も初期値があるので修正不要
        public int DifficultyLevel { get; set; } = 600;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}