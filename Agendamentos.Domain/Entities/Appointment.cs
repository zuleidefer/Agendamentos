using Agendamentos.Domain.Enums;
using Agendamentos.Domain.Exceptions;

namespace Agendamentos.Domain.Entities;

public class Appointment
{
    public static readonly TimeSpan MinCancellationNotice = TimeSpan.FromHours(2);

    private Appointment() { }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ProfessionalId { get; private set; }
    public Guid ServiceId { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public static Appointment Create(
        Guid customerId,
        Guid professionalId,
        Service service,
        DateTime startsAtUtc,
        DateTime nowUtc)
    {
        if (startsAtUtc.Kind != DateTimeKind.Utc)
            throw new DomainException("A data deve estar em UTC.");
        if (startsAtUtc <= nowUtc)
            throw new DomainException("Não é possível agendar no passado.");

        return new Appointment
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ProfessionalId = professionalId,
            ServiceId = service.Id,
            StartsAt = startsAtUtc,
            EndsAt = startsAtUtc.AddMinutes(service.DurationMinutes),
            Status = AppointmentStatus.Scheduled
        };
    }

    public void Cancel(DateTime nowUtc, bool isAdmin = false)
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new DomainException("Apenas agendamentos ativos podem ser cancelados.");

        if (!isAdmin && StartsAt - nowUtc < MinCancellationNotice)
            throw new DomainException("O cancelamento exige pelo menos 2 horas de antecedência.");

        Status = AppointmentStatus.Cancelled;
        CancelledAt = nowUtc;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new DomainException("Apenas agendamentos ativos podem ser concluídos.");
        Status = AppointmentStatus.Completed;
    }

    public bool OverlapsWith(DateTime startUtc, DateTime endUtc) =>
        Status == AppointmentStatus.Scheduled && StartsAt < endUtc && EndsAt > startUtc;
}