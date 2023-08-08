namespace z018.StockDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class StockSymbolTypeConfiguration : IEntityTypeConfiguration<StockSymbol>
{
    public void Configure(EntityTypeBuilder<StockSymbol> builder)
    {
        //// builder.ToTable("Stocks", StockSchemaNames.Stocks);
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Exchange).HasColumnName("Exchange").IsRequired();
        builder.Property(s => s.Symbol).HasColumnName("Symbol").IsRequired();
        builder.Property(s => s.Name).HasColumnName("Name");
        builder.Property(s => s.Country).HasColumnName("Country");
        builder.Property(s => s.Currency).HasColumnName("Currency");
        builder.Property(s => s.Type).HasColumnName("Type");
        builder.Property(s => s.Isin).HasColumnName("Isin");

        builder.HasIndex(s => new { s.Exchange, s.Symbol }).IsUnique();
    }
}