using Agendamentos.Domain.Exceptions;

namespace Agendamentos.Domain.Entities;

public class Service
{
    private Service() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }

    public static Service Create(string name, int durationMinutes, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do serviço é obrigatório.");
        if (durationMinutes is < 5 or > 480)
            throw new DomainException("A duração deve estar entre 5 e 480 minutos.");
        if (price < 0)
            throw new DomainException("O preço não pode ser negativo.");

        return new Service
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            DurationMinutes = durationMinutes,
            Price = price
        };
    }
}