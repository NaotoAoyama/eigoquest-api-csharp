using System.Text.Json.Serialization;

namespace EigoQuest.Api.Dtos
{
    public record QuestionQuizDto(
        long Id,
        // ▼▼▼ 修正 ▼▼▼
        [property: JsonPropertyName("question_text")] string QuestionText,
        [property: JsonPropertyName("option_a")] string OptionA,
        [property: JsonPropertyName("option_b")] string OptionB,
        [property: JsonPropertyName("option_c")] string OptionC,
        [property: JsonPropertyName("option_d")] string OptionD
    );

    public record QuestionResultDto(
        long Id,
        // ▼▼▼ 修正 ▼▼▼
        [property: JsonPropertyName("question_text")] string QuestionText,
        [property: JsonPropertyName("option_a")] string OptionA,
        [property: JsonPropertyName("option_b")] string OptionB,
        [property: JsonPropertyName("option_c")] string OptionC,
        [property: JsonPropertyName("option_d")] string OptionD,
        [property: JsonPropertyName("correct_answer")] string CorrectAnswer,
        string? Explanation
    );

    public record AnswerSubmitDto(
        // ▼▼▼ 修正 ▼▼▼
        [property: JsonPropertyName("question_id")] long QuestionId,
        [property: JsonPropertyName("selected_answer")] string SelectedAnswer
    );
    
    public record QuizSubmitResponseDto(
        // ▼▼▼ 修正 ▼▼▼
        [property: JsonPropertyName("result_ids")] List<long> ResultIds
    );

    public record ResultResponseDto(
        long Id,
        QuestionResultDto Question,
        // ▼▼▼ 修正 ▼▼▼
        [property: JsonPropertyName("selected_answer")] string SelectedAnswer,
        [property: JsonPropertyName("is_correct")] bool IsCorrect,
        [property: JsonPropertyName("answered_at")] DateTime AnsweredAt
    );
}