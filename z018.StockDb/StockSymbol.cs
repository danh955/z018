namespace z018.StockDb;

using System;

public class StockSymbol
{
    public int Id;

    public required string Exchange { get; set; }

    public required string Symbol { get; set; }

    public required string Name { get; set; }

    public required string Country { get; set; }

    public required string Currency { get; set; }

    public required string Type { get; set; }

    public required string Isin { get; set; }

    public DateTimeOffset PriceLastUpdated { get; set; } = DateTimeOffset.MinValue;

    public required List<StockPrice> StockPrices { get; set; }
}