using Agendamentos.Domain.Entities;
using Agendamentos.Domain.Enums;
using Agendamentos.Domain.Exceptions;

namespace Agendamentos.Tests.Domain;

public class AppointmentTests
{
    private static readonly DateTime Now = new(2026, 11, 20, 12, 0, 0, DateTimeKind.Utc);
    private static readonly Service Corte = Service.Create("Corte", 30, 50m);

    private static Appointment CreateAt(DateTime start) =>
        Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), Corte, start, Now);

    [Fact]
    public void Create_InThePast_Throws()
    {
        var act = () => CreateAt(Now.AddMinutes(-1));
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_CalculatesEndsAtFromServiceDuration()
    {
        var appt = CreateAt(Now.AddDays(1));
        Assert.Equal(appt.StartsAt.AddMinutes(30), appt.EndsAt);
        Assert.Equal(AppointmentStatus.Scheduled, appt.Status);
    }

    [Fact]
    public void Cancel_LessThanTwoHoursBefore_ThrowsForCustomer()
    {
        var appt = CreateAt(Now.AddMinutes(90));
        Assert.Throws<DomainException>(() => appt.Cancel(Now));
    }

    [Fact]
    public void Cancel_LessThanTwoHoursBefore_AllowedForAdmin()
    {
        var appt = CreateAt(Now.AddMinutes(90));
        appt.Cancel(Now, isAdmin: true);
        Assert.Equal(AppointmentStatus.Cancelled, appt.Status);
    }

    [Fact]
    public void Cancel_TwiceThrows()
    {
        var appt = CreateAt(Now.AddDays(1));
        appt.Cancel(Now);
        Assert.Throws<DomainException>(() => appt.Cancel(Now));
    }

    [Theory]
    [InlineData(-10, 10, true)]   // começa antes, termina dentro
    [InlineData(10, 20, true)]    // totalmente dentro
    [InlineData(20, 40, true)]    // começa dentro, termina depois
    [InlineData(-10, 40, true)]   // engloba
    [InlineData(30, 60, false)]   // começa exatamente quando termina
    [InlineData(-30, 0, false)]   // termina exatamente quando começa
    public void OverlapsWith_DetectsConflictsCorrectly(int startOffset, int endOffset, bool expected)
    {
        var appt = CreateAt(Now.AddDays(1)); // dura 30 min
        var start = appt.StartsAt.AddMinutes(startOffset);
        var end = appt.StartsAt.AddMinutes(endOffset);

        Assert.Equal(expected, appt.OverlapsWith(start, end));
    }

    [Fact]
    public void OverlapsWith_IgnoresCancelledAppointments()
    {
        var appt = CreateAt(Now.AddDays(1));
        appt.Cancel(Now);
        Assert.False(appt.OverlapsWith(appt.StartsAt, appt.EndsAt));
    }
}