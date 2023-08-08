namespace z018.StockDb.Updater.ClassMap
{
    using System.Globalization;
    using CsvHelper.Configuration;

    internal class EodToStockPriceMap : ClassMap<StockPrice>
    {
        public EodToStockPriceMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Id).Ignore();
            Map(m => m.StockSymbolId).Ignore();
            Map(m => m.AdjustedClose).Name("Adjusted_close");
            ReferenceMaps.Remove(ReferenceMaps.Find<StockPrice>(m => m.StockSymbol));
        }
    }
}