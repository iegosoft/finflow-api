using System.Text;
using FinFlow.API.Middlewares;
using FinFlow.Application.DTOs.Autenticacao;
using FinFlow.Application.DTOs.Categorias;
using FinFlow.Application.DTOs.Transacoes;
using FinFlow.Application.Interfaces;
using FinFlow.Application.Servicos;
using FinFlow.Application.Validadores;
using FinFlow.Domain.Interfaces;
using FinFlow.Infra.Contexto;
using FinFlow.Infra.Repositorios;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de dados com Entity Framework Core e PostgreSQL ──
builder.Services.AddDbContext<FinFlowDbContext>(opcoes =>
    opcoes.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// ── Repositórios via injeção de dependência ──
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<ITransacaoRepositorio, TransacaoRepositorio>();

// ── Serviços da camada Application via injeção de dependência ──
builder.Services.AddScoped<IAutenticacaoServico, AutenticacaoServico>();
builder.Services.AddScoped<ICategoriaServico, CategoriaServico>();
builder.Services.AddScoped<ITransacaoServico, TransacaoServico>();

// ── Validadores FluentValidation ──
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<RegistrarDto>, RegistrarValidador>();
builder.Services.AddScoped<IValidator<CriarCategoriaDto>, CriarCategoriaValidador>();
builder.Services.AddScoped<IValidator<AtualizarCategoriaDto>, AtualizarCategoriaValidador>();
builder.Services.AddScoped<IValidator<CriarTransacaoDto>, CriarTransacaoValidador>();
builder.Services.AddScoped<IValidator<AtualizarTransacaoDto>, AtualizarTransacaoValidador>();

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

// ── Middleware global de tratamento de erros — deve ser o primeiro da pipeline ──
app.UseMiddleware<TratamentoDeErrosMiddleware>();

app.UseHttpsRedirection();
app.UseCors("PermitirTudo");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
