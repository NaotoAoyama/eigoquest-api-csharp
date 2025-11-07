using System.ComponentModel.DataAnnotations;

namespace EigoQuest.Api.Models
{
    public class Question
    {
        // models.BigAutoField -> long Id
        // C#の規約で 'Id' という名前のプロパティは自動的に主キー(PK)として認識されます。
        public long Id { get; set; }

        // models.TextField -> string
        [Required] // NOT NULL制約
        public string QuestionText { get; set; }

        // models.CharField -> string
        [Required]
        public string OptionA { get; set; }
        [Required]
        public string OptionB { get; set; }
        [Required]
        public string OptionC { get; set; }
        [Required]
        public string OptionD { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        // models.TextField(blank=True, null=True) -> string?
        // 'string?' と '?' を付けることで、nullを許容する型になります。
        public string? Explanation { get; set; }

        // default='PART5' -> プロパティの初期値として設定
        public string Part { get; set; } = "PART5";

        // default=600 -> プロパティの初期値として設定
        public int DifficultyLevel { get; set; } = 600;

        // auto_now_add=True -> DateTime型にし、デフォルト値を設定
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // auto_now=True -> DateTime型にし、デフォルト値を設定
        // (auto_nowはDB保存時に自動更新するロジックが別途必要ですが、
        //  モデル定義としてはこれでOKです)
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}