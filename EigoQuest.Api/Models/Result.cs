using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EigoQuest.Api.Models
{
    public class Result
    {
        public long Id { get; set; }

        [Required]
        public string SelectedAnswer { get; set; } = string.Empty; // 修正

        public bool IsCorrect { get; set; }
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
        
        // --- 外部キー ---
        
        // 1. User 関連
        public string ApplicationUserId { get; set; } = string.Empty; // 修正
        
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser User { get; set; } = null!; // 修正

        // 2. Question 関連
        public long QuestionId { get; set; }
        
        [ForeignKey("QuestionId")]
        public Question Question { get; set; } = null!; // 修正
    }
}