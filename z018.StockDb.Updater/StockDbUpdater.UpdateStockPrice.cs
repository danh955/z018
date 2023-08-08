namespace z018.StockDb.Updater;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using z018.EodHistoricalData;
using z018.StockDb.Updater.ClassMap;

public partial class StockDbUpdater
{
    /// <summary>
    /// Update the stock prices.
    /// </summary>
    /// <param name="symbol">Symbol code of stock prices to update.</param>
    /// <param name="fromDate">From date to retrieve stock prices.</param>
    /// <param name="toDate">The last date to retrieve stock prices. If null, todays date.</param>
    /// <param name="period">Day, week, or month.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>Task</returns>
    //// TODO: Add exchangeCode
    public async Task UpdateStockPriceAsync(string symbol, DateOnly? fromDate = null, DateOnly? toDate = null, DataPeriod? period = null, CancellationToken cancellationToken = default)
    {
        logger?.LogInformation("UpdateStockPrice {symbol}, {fromDate}, {toDate}, {period}", symbol, fromDate, toDate, period);

        List<StockPrice> sourcePrice;
        try
        {
            sourcePrice = await client.GetEodAsync<StockPrice>(symbol, null, fromDate, toDate, period, new EodToStockPriceMap(), cancellationToken);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "GetEodAsync<{className}> error {symbol}", nameof(StockPrice), symbol);
            return;
        }

        var StockSymbolId = await db.StockSymbols
            .Where(s => s.Symbol == symbol)
            .Select(s => s.Id).SingleOrDefaultAsync(cancellationToken);

        if (StockSymbolId == 0)
        {
            logger?.LogWarning("Symbol {symbol} not found in DB", symbol);
            return;
        }

        var currentPrices = await db.StockPrices
            .Where(p => p.StockSymbolId == StockSymbolId)
            .Where(p => fromDate == null || p.Date >= fromDate)
            .Where(p => toDate == null || p.Date <= toDate)
            .ToDictionaryAsync(p => p.Date, cancellationToken);

        List<StockPrice> updatePrices = new();
        List<StockPrice> addPrices = new();
        foreach (var item in sourcePrice)
        {
            var found = currentPrices.GetValueOrDefault(item.Date);
            if (found == null)
            {
                // Add
                item.StockSymbolId = StockSymbolId;
                addPrices.Add(item);
            }
            else
            {
                // Update
                if (found.Open != item.Open
                    | found.High != item.High
                    | found.Low != item.Low
                    | found.Close != item.Close
                    | found.AdjustedClose != item.AdjustedClose
                    | found.Volume != item.Volume)
                {
                    found.Open = item.Open;
                    found.High = item.High;
                    found.Low = item.Low;
                    found.Close = item.Close;
                    found.AdjustedClose = item.AdjustedClose;
                    found.Volume = item.Volume;
                    updatePrices.Add(found);
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

        int deleteCount = 0;
        if (currentPrices.Count > 0)
        {
            var dates = sourcePrice.Select(p => p.Date).ToHashSet();
            var deletePrices = currentPrices.Values.Where(p => dates.Contains(p.Date) == false).ToList();
            if (deletePrices.Count > 0)
            {
                deleteCount++;
                db.StockPrices.RemoveRange(deletePrices);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        logger?.LogInformation("{symbol}  Updated: {updated}  Added: {added}  Deleted: {deleted}", symbol, updatePrices.Count, addPrices.Count, deleteCount);
    }
}