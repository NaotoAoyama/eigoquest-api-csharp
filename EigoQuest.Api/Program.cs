using System.Text;
using EigoQuest.Api.Data; // DbContext を使うため
using EigoQuest.Api.Models; // ApplicationUser を使うため
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT認証
using Microsoft.AspNetCore.Identity; // ユーザー認証
using Microsoft.EntityFrameworkCore; // EF Core (ORM)
using Microsoft.IdentityModel.Tokens; // JWTトークン検証

var builder = WebApplication.CreateBuilder(args);

// --- 1. サービスの登録 (Django: INSTALLED_APPS に相当) ---

// 1a. CORS (クロスオリジン通信) の設定
// settings.py の CORS_ALLOWED_ORIGINS の換装
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173", // Vue.js 開発サーバー
            "https://eigoquest-frontend.vercel.app" // Vercel本番環境
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// 1b. DBコンテキスト (ORM) の登録
// 'appsettings.json' から "DefaultConnection" の文字列を読み込み、
// PostgreSQL (Npgsql) を使う設定で ApplicationDbContext を登録します。
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 1c. ユーザー認証 (Identity) の登録
// Djangoの 'django.contrib.auth' の換装
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 1d. JWT認証 (SimpleJWT) の登録
// 'rest_framework_simplejwt' の換装
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // APIに送られてきたJWTが本物か検証するルールを設定
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

// 1e. コントローラー (Django: Views) の登録
builder.Services.AddControllers();

// (Swagger/OpenAPI の登録 - APIの動作テストに便利なので残します)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 作成した JwtService を、API全体（コントローラー）で使えるように Program.cs に登録
builder.Services.AddScoped<EigoQuest.Api.Services.JwtService>();


// --- 2. ミドルウェアパイプラインの構築 (Django: MIDDLEWARE に相当) ---

var app = builder.Build();

// 開発環境でのみSwagger (API仕様書) を有効化
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2a. CORSミドルウェアを有効化
// 'corsheaders.middleware.CorsMiddleware' の換装
app.UseCors("AllowVueApp");

// 2b. 認証ミドルウェアを有効化
// 'django.contrib.auth.middleware.AuthenticationMiddleware' の換装
app.UseAuthentication();
app.UseAuthorization();


// 2c. URLルーティングの有効化
// 'config.urls' の換装
// これが 'Controllers' フォルダ内のAPIを自動的にURLにマッピングします
app.MapControllers();

// サーバー起動
app.Run();