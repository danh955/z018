namespace z018.StockDb.Updater;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using z018.StockDb.Updater.ClassMap;

public partial class StockDbUpdater
{
    /// <summary>
    /// Update the database with all the prices to the current date.
    /// </summary>
    /// <param name="exchangeCode">Exchange code to process.  Default is "US".</param>
    /// <param name="daysToBackfill">Number of day to go back fill from the last date in the database.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>Task.</returns>
    public async Task UpdateLastStockPrices(string? exchangeCode = null, int daysToBackfill = 0, CancellationToken cancellationToken = default)
    {
        var today = TodaysWeekDate();
        var selectedDate = await db.StockPrices.MaxAsync(p => (DateOnly?)p.Date, cancellationToken);
        selectedDate ??= today;

        logger?.LogInformation("{exchangeCode}, {daysToBackfill}, {selectedDate}", exchangeCode, daysToBackfill, selectedDate);

        for (DateOnly date = selectedDate.Value.AddDays(0 - daysToBackfill); date <= today; date = date.AddDays(1))
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;
            await UpdateLastStockPricesByDate(exchangeCode, date, cancellationToken);
        }
    }

    /// <summary>
    /// Update all the stock prices for one date.
    /// </summary>
    /// <param name="exchangeCode">Exchange code to process.  Default is "US".</param>
    /// <param name="updateDate">Date to update.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>Task.</returns>
    public async Task UpdateLastStockPricesByDate(string? exchangeCode = null, DateOnly? updateDate = null, CancellationToken cancellationToken = default)
    {
        exchangeCode ??= DefaultExchangeCode;
        updateDate ??= TodaysWeekDate();

        logger?.LogInformation("{exchangeCode}, {updateDate}", exchangeCode, updateDate);

        // Get database stock symbol and the selected price record.
        var currentPrices = await db.StockSymbols
            .GroupJoin(db.StockPrices,
                s => s.Id,
                p => p.StockSymbolId,
                (s, p) => new { s.Id, s.Symbol, StockPrice = p.Where(p => p.Date == updateDate).SingleOrDefault() })
            .ToDictionaryAsync(s => s.Symbol, StringComparer.OrdinalIgnoreCase, cancellationToken);

        List<BulkLastDayStockPrice> sourcePrices = await client.GetEodBulkLastDayAsync(exchangeCode, null, updateDate, new EodBulkLastDayToStockPriceMap(), cancellationToken);

        List<StockPrice> updatePrices = new();
        List<StockPrice> addPrices = new();
        foreach (BulkLastDayStockPrice item in sourcePrices)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentFound = currentPrices.GetValueOrDefault(item.Symbol);
            if (currentFound == null)
            {
                // Stock symbol not found.
                continue;
            }

            if (currentFound.StockPrice == null)
            {
                // Add
                StockPrice addPrice = (StockPrice)item;
                addPrice.StockSymbolId = currentFound.Id;
                addPrices.Add(addPrice);
            }
            else
            {
                // Update
                StockPrice foundPrice = currentFound.StockPrice;
                if (foundPrice.Open != item.Open
                    | foundPrice.High != item.High
                    | foundPrice.Low != item.Low
                    | foundPrice.Close != item.Close
                    | foundPrice.AdjustedClose != item.AdjustedClose
                    | foundPrice.Volume != item.Volume)
                {
                    //// Not sure: If it has changed, we should update all the prices.   maybe.
                    foundPrice.Open = item.Open;
                    foundPrice.High = item.High;
                    foundPrice.Low = item.Low;
                    foundPrice.Close = item.Close;
                    foundPrice.AdjustedClose = item.AdjustedClose;
                    foundPrice.Volume = item.Volume;
                    updatePrices.Add(foundPrice);
                }
            }
        }

        if (updatePrices.Count > 0)
        {
            db.StockPrices.UpdateRange(updatePrices);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (addPrices.Count > 0)
        {
            db.StockPrices.AddRange(addPrices);
            await db.SaveChangesAsync(cancellationToken);
        }

        this.logger?.LogInformation("Date = {date},  Update count = {updateCount}, Add count = {addCount}", updateDate, updatePrices.Count, addPrices.Count);
    }
}