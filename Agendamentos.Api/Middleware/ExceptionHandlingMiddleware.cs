using Agendamentos.Application.Exceptions;
using Agendamentos.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Agendamentos.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        ProblemDetails problem = ex switch
        {
            ValidationException v => new ValidationProblemDetails(
                v.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            { Status = 400, Title = "Dados inválidos" },

            DomainException d => new ProblemDetails
            { Status = 400, Title = "Regra de negócio violada", Detail = d.Message },

            NotFoundException n => new ProblemDetails
            { Status = 404, Title = "Recurso não encontrado", Detail = n.Message },

            _ => new ProblemDetails { Status = 500, Title = "Erro interno do servidor" }
        };

        if (problem.Status == 500)
            logger.LogError(ex, "Erro não tratado");

        context.Response.StatusCode = problem.Status!.Value;
        await context.Response.WriteAsJsonAsync(problem, problem.GetType());
    }
}