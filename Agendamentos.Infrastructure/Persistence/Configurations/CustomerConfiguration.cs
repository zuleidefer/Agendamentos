using Agendamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agendamentos.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("customers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Email).HasMaxLength(200).IsRequired();
        b.Property(x => x.PasswordHash).IsRequired();
        b.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
        b.HasIndex(x => x.Email).IsUnique();
    }
}