namespace z018.StockDb;

using System;
using System.Runtime.CompilerServices;

public class StockPrice
{
    public int Id { get; set; }

    public int StockSymbolId { get; set; }

    public DateOnly Date { get; set; }

    public double Open { get; set; }

    public double High { get; set; }

    public double Low { get; set; }

    public double Close { get; set; }

    public double AdjustedClose { get; set; }

    public long Volume { get; set; }

    public required StockSymbol StockSymbol { get; set; }

    public override string ToString()
    {
        return $"{StockSymbolId},{Date},{Open},{High},{Low},{Close},{AdjustedClose},{Volume}";
    }
}