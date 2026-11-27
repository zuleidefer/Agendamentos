using Agendamentos.Application.Features.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Agendamentos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateServiceRequestValidator>();
        services.AddScoped<IServiceAppService, ServiceAppService>();
        return services;
    }
}