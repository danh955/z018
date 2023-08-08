namespace z018.StockDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class StockPriceTypeConfiguration : IEntityTypeConfiguration<StockPrice>
{
    public void Configure(EntityTypeBuilder<StockPrice> builder)
    {
        //// builder.ToTable("Stocks", StockSchemaNames.Stocks);
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StockSymbolId).HasColumnName("StockSymbolId");
        builder.Property(s => s.Date).HasColumnName("Date");
        builder.Property(s => s.Open).HasColumnName("Open");
        builder.Property(s => s.High).HasColumnName("High");
        builder.Property(s => s.Low).HasColumnName("Low");
        builder.Property(s => s.Close).HasColumnName("Close");
        builder.Property(s => s.AdjustedClose).HasColumnName("AdjustedClose");
        builder.Property(s => s.Volume).HasColumnName("Volume");

        builder.HasIndex(s => new { s.StockSymbolId, s.Date })
            .IsUnique();

        // Stock prices has one stock.
        builder.HasOne<StockSymbol>(p => p.StockSymbol)
            .WithMany(s => s.StockPrices)
            .HasForeignKey(p => p.StockSymbolId);
    }
}