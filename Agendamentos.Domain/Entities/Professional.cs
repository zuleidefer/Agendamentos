using Agendamentos.Domain.Exceptions;

namespace Agendamentos.Domain.Entities;

public class Professional
{
    private Professional() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    public static Professional Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do profissional é obrigatório.");

        return new Professional
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
}