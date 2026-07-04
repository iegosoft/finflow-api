using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Controladores e serialização ──
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger com suporte a autenticação Bearer ──
builder.Services.AddSwaggerGen(opcoes =>
{
    opcoes.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FinFlow API",
        Version = "v1",
        Description = "API RESTful de controle financeiro pessoal",
        Contact = new OpenApiContact
        {
            Name = "Iego Costa",
            Url = new Uri("https://github.com/iegosoft")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // ── Define o esquema de segurança JWT no Swagger ──
    opcoes.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {seu_token}"
    });

    opcoes.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── Autenticação JWT ──
var chaveJwt = builder.Configuration["Jwt:Chave"]
    ?? throw new InvalidOperationException("Chave JWT não configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emissor"],
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt))
        };
    });

builder.Services.AddAuthorization();

// ── CORS — permite qualquer origem em desenvolvimento ──
builder.Services.AddCors(opcoes =>
{
    opcoes.AddPolicy("PermitirTudo", politica =>
        politica.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});

var app = builder.Build();

// ── Swagger disponível apenas em desenvolvimento ──
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/swagger/v1/swagger.json", "FinFlow API v1");
        opcoes.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("PermitirTudo");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
