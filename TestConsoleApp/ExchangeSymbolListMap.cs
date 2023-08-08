namespace TestConsoleApp;

using System.Globalization;
using CsvHelper.Configuration;
using z018.StockDb;

internal class ExchangeSymbolListToStockSymbolMap : ClassMap<StockSymbol>
{
    public ExchangeSymbolListToStockSymbolMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.Symbol).Name("Code");
        Map(m => m.PriceLastUpdated).Ignore();
    }
}