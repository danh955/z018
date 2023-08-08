namespace z018.StockDb.Updater.ClassMap;

using System.Globalization;
using CsvHelper.Configuration;

internal sealed class EodBulkLastDayToStockPriceMap : ClassMap<BulkLastDayStockPrice>
{
    public EodBulkLastDayToStockPriceMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.Id).Ignore();
        Map(m => m.StockSymbolId).Ignore();
        Map(m => m.AdjustedClose).Name("Adjusted_close");
        Map(m => m.Exchange).Name("Ex");
        Map(m => m.Symbol).Name("Code");
        ReferenceMaps.Remove(ReferenceMaps.Find<StockPrice>(m => m.StockSymbol));
    }
}

internal sealed class BulkLastDayStockPrice : StockPrice
{
    public required string Exchange { get; set; }

    public required string Symbol { get; set; }
}