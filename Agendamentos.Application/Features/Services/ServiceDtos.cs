namespace Agendamentos.Application.Features.Services;

public record CreateServiceRequest(string Name, int DurationMinutes, decimal Price);
public record ServiceResponse(Guid Id, string Name, int DurationMinutes, decimal Price);