using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EigoQuest.Api.Models
{
    public class Result
    {
        public long Id { get; set; }

        [Required]
        public string SelectedAnswer { get; set; }
        
        // models.BooleanField(default=False) -> bool
        // C#のboolはデフォルトで false のため、初期値設定は不要です。
        public bool IsCorrect { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        
        // --- Djangoの ForeignKey の換装 ---
        
        // 1. user = models.ForeignKey(User, ...)
        //
        // C#では、リレーションを「外部キー(ID)」と「ナビゲーションプロパティ」の
        // 2つで表現するのが一般的です。
        
        // 1a. 外部キー (ID)
        // ApplicationUser (IdentityUser) の主キーは string (GUID) です。
        public string ApplicationUserId { get; set; }
        
        // 1b. ナビゲーションプロパティ (関連するUserオブジェクト)
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser User { get; set; }

        
        // 2. question = models.ForeignKey(Question, ...)
        
        // 2a. 外部キー (ID)
        // Questionの主キーは long です。
        public long QuestionId { get; set; }
        
        // 2b. ナビゲーションプロパティ (関連するQuestionオブジェクト)
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
}