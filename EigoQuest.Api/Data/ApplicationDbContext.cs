using EigoQuest.Api.Models; // Question, Result, ApplicationUser を使うため
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // IdentityDbContext を使うため
using Microsoft.EntityFrameworkCore; // DbContext を使うため

namespace EigoQuest.Api.Data
{
    // C#のユーザー認証(Identity)とDBを連携させるため、
    // ただの DbContext ではなく IdentityDbContext<T> を継承します。
    // <ApplicationUser> は、フェーズ3で定義したカスタムUserクラスです。
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Djangoの models.py にクラスを定義する作業に相当 ---
        // "Questions" という名前のテーブルを、Questionモデルと連携させます
        public DbSet<Question> Questions { get; set; }
        // "Results" という名前のテーブルを、Resultモデルと連携させます
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Djangoの Meta.constraints の換装 ---
            // Resultモデル (テーブル) に対して、
            // ApplicationUserId と QuestionId の組み合わせを
            // ユニーク(重複禁止)制約に設定します。
            builder.Entity<Result>()
                .HasIndex(r => new { r.ApplicationUserId, r.QuestionId })
                .IsUnique();
        }
    }
}