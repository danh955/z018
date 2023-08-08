namespace z018.StockDb.Updater.ClassMap;

using System.Globalization;
using CsvHelper.Configuration;

internal class ExchangeSymbolListToStockSymbolMap : ClassMap<StockSymbol>
{
    public ExchangeSymbolListToStockSymbolMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.Symbol).Name("Code");
        Map(m => m.PriceLastUpdated).Ignore();
        Map(m => m.StockPrices).Ignore();
    }
}