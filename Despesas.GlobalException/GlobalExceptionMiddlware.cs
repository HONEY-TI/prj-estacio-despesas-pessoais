using Despesas.GlobalException.CustomExceptions.Acesso;
using Despesas.GlobalException.CustomExceptions.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Despesas.GlobalException;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AcessoException ex)
        {
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, (int)HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            // Verifica se é uma exceção do EF Core / repositório
            if (!await EfCoreExceptionHandler.HandleAsync(context, ex, _logger))
            {
                // Qualquer outra exceção inesperada
                _logger.LogError(ex, "Erro inesperado");
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro inesperado. Tente novamente mais tarde.");
            }
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            success = false,
            error = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
