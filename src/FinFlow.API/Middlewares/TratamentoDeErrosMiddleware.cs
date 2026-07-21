using System.Net;
using System.Text.Json;
using FinFlow.Domain.Excecoes;

namespace FinFlow.API.Middlewares;

/// <summary>
/// Middleware global de tratamento de erros.
/// Intercepta exceções não tratadas e retorna respostas JSON padronizadas,
/// evitando a exposição de stack traces ao cliente.
/// </summary>
public class TratamentoDeErrosMiddleware
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<TratamentoDeErrosMiddleware> _logger;

    public TratamentoDeErrosMiddleware(RequestDelegate proximo, ILogger<TratamentoDeErrosMiddleware> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (RegraDeNegocioException ex)
        {
            // ── Regras de negócio retornam 400 com a mensagem da exceção ──
            await EscreverRespostaAsync(contexto, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            // ── Erros inesperados são logados e retornam 500 genérico ──
            _logger.LogError(ex, "Erro inesperado: {Mensagem}", ex.Message);
            await EscreverRespostaAsync(contexto, HttpStatusCode.InternalServerError,
                "Ocorreu um erro interno. Tente novamente mais tarde.");
        }
    }

    // ── Serializa a resposta de erro como JSON e define o status code ──
    private static async Task EscreverRespostaAsync(HttpContext contexto, HttpStatusCode statusCode, string mensagem)
    {
        contexto.Response.ContentType = "application/json";
        contexto.Response.StatusCode = (int)statusCode;

        var resposta = JsonSerializer.Serialize(new { mensagem });
        await contexto.Response.WriteAsync(resposta);
    }
}
