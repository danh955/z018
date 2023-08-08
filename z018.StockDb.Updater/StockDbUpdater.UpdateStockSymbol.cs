namespace z018.StockDb.Updater;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using z018.StockDb.Updater.ClassMap;

public partial class StockDbUpdater
{
    /// <summary>
    /// Update the stock symbol database table.
    /// This will not do delete.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns></returns>
    public async Task UpdateStockSymbolAsync(CancellationToken cancellationToken = default)
    {
        List<StockSymbol> updateSymbols = new();
        List<StockSymbol> addSymbols = new();

        var sourceSymbols = await client.GetExchangeSymbolListAsync("US", new ExchangeSymbolListToStockSymbolMap(), cancellationToken);

        var currentSymbols = await db.StockSymbols.ToDictionaryAsync(s => s.Symbol, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var item in sourceSymbols)
        {
            var found = currentSymbols.GetValueOrDefault(item.Symbol);
            if (found == null)
            {
                // Add
                item.Symbol = item.Symbol.ToUpper();
                addSymbols.Add(item);
            }
            else
            {
                // Update
                if (found.Name != item.Name
                    || found.Country != item.Country
                    || found.Exchange != item.Exchange
                    || found.Currency != item.Currency
                    || found.Type != item.Type
                    || found.Isin != item.Isin)
                {
                    found.Name = item.Name;
                    found.Country = item.Country;
                    found.Exchange = item.Exchange;
                    found.Currency = item.Currency;
                    found.Type = item.Type;
                    found.Isin = item.Isin;
                    updateSymbols.Add(found);
                }
            }
        }

        if (addSymbols.Count > 0)
        {
            db.StockSymbols.AddRange(addSymbols);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (updateSymbols.Count > 0)
        {
            db.StockSymbols.UpdateRange(updateSymbols);
            await db.SaveChangesAsync(cancellationToken);
        }

        logger?.LogInformation("Symbols Added: {added}  Updated: {updated}", addSymbols.Count, updateSymbols.Count);
    }
}