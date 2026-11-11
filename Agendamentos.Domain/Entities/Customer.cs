using Agendamentos.Domain.Enums;
using Agendamentos.Domain.Exceptions;

namespace Agendamentos.Domain.Entities;

public class Customer
{
    private Customer() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }

    public static Customer Create(string name, string email, string passwordHash, UserRole role = UserRole.Customer)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("E-mail inválido.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("A senha é obrigatória.");

        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role
        };
    }
}