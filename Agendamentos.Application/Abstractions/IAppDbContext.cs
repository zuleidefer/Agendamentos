using Agendamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agendamentos.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Professional> Professionals { get; }
    DbSet<Service> Services { get; }
    DbSet<Appointment> Appointments { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}