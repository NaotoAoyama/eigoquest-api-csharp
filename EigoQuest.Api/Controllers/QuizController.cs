using EigoQuest.Api.Data;
using EigoQuest.Api.Dtos;
using EigoQuest.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EigoQuest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // -> /api/quiz
    // Django: permission_classes = [IsAuthenticated] の換装
    [Authorize] 
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ログイン中のユーザーIDを取得するヘルパー
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Django: GET /api/quiz/ (QuizAPIView.get)
        [HttpGet] // -> GET /api/quiz
        public async Task<IActionResult> GetQuizQuestions()
        {
            var questions = await _context.Questions
                                    // Django: order_by('?')[:10]
                                    .OrderBy(q => EF.Functions.Random()) 
                                    .Take(10)
                                    // Django: QuestionSerializer (正解を含まない)
                                    .Select(q => new QuestionQuizDto(
                                        q.Id,
                                        q.QuestionText,
                                        q.OptionA,
                                        q.OptionB,
                                        q.OptionC,
                                        q.OptionD
                                    ))
                                    .ToListAsync();
            return Ok(questions);
        }

        // Django: POST /api/quiz/submit/ (QuizSubmitAPIView.post)
        [HttpPost("submit")] // -> POST /api/quiz/submit
        public async Task<IActionResult> SubmitQuiz([FromBody] List<AnswerSubmitDto> answers)
        {
            var userId = GetUserId();
            var questionIds = answers.Select(a => a.QuestionId);
            
            // 必要なQuestionをDBから一括取得
            var questions = await _context.Questions
                                    .Where(q => questionIds.Contains(q.Id))
                                    .ToDictionaryAsync(q => q.Id);

            var resultIds = new List<long>();
            var resultsToSave = new List<Result>();

            foreach (var answer in answers)
            {
                if (!questions.TryGetValue(answer.QuestionId, out var question))
                {
                    continue; // 存在しない問題はスキップ
                }
                
                var isCorrect = question.CorrectAnswer == answer.SelectedAnswer;

                // Django: Result.objects.update_or_create
                var existingResult = await _context.Results
                    .FirstOrDefaultAsync(r => r.ApplicationUserId == userId && r.QuestionId == question.Id);

                if (existingResult != null)
                {
                    existingResult.SelectedAnswer = answer.SelectedAnswer;
                    existingResult.IsCorrect = isCorrect;
                    existingResult.AnsweredAt = DateTime.UtcNow;
                }
                else
                {
                    existingResult = new Result
                    {
                        ApplicationUserId = userId,
                        QuestionId = question.Id,
                        SelectedAnswer = answer.SelectedAnswer,
                        IsCorrect = isCorrect
                    };
                    _context.Results.Add(existingResult);
                }
                // DB保存前でもIDが振られるように、一度保存する (IDリストを返すため)
                // (より高度なUnit of Workパターンもありますが、これが一番確実です)
                await _context.SaveChangesAsync();
                resultIds.Add(existingResult.Id);
            }
            
            return Ok(new QuizSubmitResponseDto(resultIds));
        }

        // Django: GET /api/results/ (QuizResultAPIView.get)
        [HttpGet("/api/results")] // -> GET /api/results (QuizControllerのルートを上書き)
        public async Task<IActionResult> GetResults([FromQuery] string ids)
        {
            var userId = GetUserId();
            var resultIds = ids.Split(',').Select(long.Parse).ToList();

            // Django: ResultSerializer (QuestionResultSerializerをネスト)
            var results = await _context.Results
                .Include(r => r.Question) // 関連するQuestionをJOIN
                .Where(r => r.ApplicationUserId == userId && resultIds.Contains(r.Id))
                .Select(r => new ResultResponseDto(
                    r.Id,
                    new QuestionResultDto( // ネストされたDTOを生成
                        r.Question.Id,
                        r.Question.QuestionText,
                        r.Question.OptionA,
                        r.Question.OptionB,
                        r.Question.OptionC,
                        r.Question.OptionD,
                        r.Question.CorrectAnswer,
                        r.Question.Explanation
                    ),
                    r.SelectedAnswer,
                    r.IsCorrect,
                    r.AnsweredAt
                ))
                .ToListAsync();
            
            // Django: Case/When での順序保持 を換装
            var orderedResults = results.OrderBy(r => resultIds.IndexOf(r.Id));
            
            return Ok(orderedResults);
        }
    }
}