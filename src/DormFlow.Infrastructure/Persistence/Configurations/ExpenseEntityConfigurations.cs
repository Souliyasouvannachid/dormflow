using DormFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DormFlow.Infrastructure.Persistence.Configurations;

/// <summary>ตั้งค่าตารางค่าใช้จ่าย</summary>
public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("expenses");

        builder.Property(expense => expense.DormitoryId).IsRequired();
        builder.Property(expense => expense.Category).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(expense => expense.ExpenseDate).IsRequired();
        builder.Property(expense => expense.Description).HasMaxLength(500).IsRequired();
        builder.Property(expense => expense.PaidTo).HasMaxLength(200);
        builder.Property(expense => expense.Notes).HasMaxLength(1000);

        builder.HasIndex(expense => expense.Category);
        builder.HasIndex(expense => expense.ExpenseDate);
    }
}
