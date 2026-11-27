using Agendamentos.Application.Abstractions;
using Agendamentos.Application.Common;
using Agendamentos.Application.Exceptions;
using Agendamentos.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Application.Features.Services;

public interface IServiceAppService
{
    Task<ServiceResponse> CreateAsync(CreateServiceRequest request, CancellationToken ct);
    Task<PagedResult<ServiceResponse>> ListAsync(int page, int pageSize, CancellationToken ct);
    Task<ServiceResponse> GetByIdAsync(Guid id, CancellationToken ct);
}

public class ServiceAppService(IAppDbContext db, IValidator<CreateServiceRequest> validator) : IServiceAppService
{
    public async Task<ServiceResponse> CreateAsync(CreateServiceRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var service = Service.Create(request.Name, request.DurationMinutes, request.Price);
        db.Services.Add(service);
        await db.SaveChangesAsync(ct);

        return ToResponse(service);
    }

    public async Task<PagedResult<ServiceResponse>> ListAsync(int page, int pageSize, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Services.AsNoTracking().OrderBy(s => s.Name);
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new ServiceResponse(s.Id, s.Name, s.DurationMinutes, s.Price))
            .ToListAsync(ct);

        return new PagedResult<ServiceResponse>(items, page, pageSize, total);
    }

    public async Task<ServiceResponse> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var service = await db.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException("Serviço não encontrado.");
        return ToResponse(service);
    }

    private static ServiceResponse ToResponse(Service s) =>
        new(s.Id, s.Name, s.DurationMinutes, s.Price);
}