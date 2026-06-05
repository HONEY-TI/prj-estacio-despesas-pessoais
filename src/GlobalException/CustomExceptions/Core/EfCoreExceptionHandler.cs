using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GlobalException.CustomExceptions.Core;

public static class EfCoreExceptionHandler
{
    public static async Task<bool> HandleAsync(HttpContext context, Exception ex, ILogger logger)
    {
        switch (ex)
        {
            case DbUpdateConcurrencyException concurrencyEx:
                logger.LogError(concurrencyEx, "Conflito de concorrência no banco de dados");
                await WriteResponseAsync(context, StatusCodes.Status409Conflict,
                    "Erro ao atualizar o registro. Pode ter sido alterado ou removido por outro usuário.");
                return true;

            case DbUpdateException dbEx:
                logger.LogError(dbEx, "Erro ao salvar alterações no banco de dados");

                // 🔹 Se for violação de chave estrangeira (FK constraint)
                if (dbEx.InnerException?.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) == true)
                {
                    await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                        "Não é possível excluir este registro, pois existem dados vinculados.");
                    return true;
                }

                await WriteResponseAsync(context, StatusCodes.Status500InternalServerError,
                    "Erro ao acessar os dados. Tente novamente mais tarde.");
                return true;

            case InvalidOperationException invOpEx:
                logger.LogError(invOpEx, "Registro não encontrado ou operação inválida no repositório");
                await WriteResponseAsync(context, StatusCodes.Status404NotFound,
                    "Registro não encontrado ou operação inválida.");
                return true;

            case NullReferenceException nullEx:
                logger.LogError(nullEx, "Referência nula detectada");
                await WriteResponseAsync(context, StatusCodes.Status500InternalServerError,
                    "Erro interno inesperado.");
                return true;

            case ArgumentException argEx:
                logger.LogError(argEx, "Erro de argumento inválido");
                await WriteResponseAsync(context, StatusCodes.Status400BadRequest,
                    argEx.Message);
                return true;

            default:
                // Qualquer outra exceção -> deixa o middleware global tratar
                return false;
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
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
