namespace TestConsoleApp;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using z018.StockDb;
using z018.StockDb.Updater;

internal class Testing2
{
    public static async Task Test(string apiToken, StockDbContext db, ILoggerFactory loggerFactory)
    {
        ////await db.Database.EnsureDeletedAsync();
        ////await db.Database.EnsureCreatedAsync();

        var updater = new StockDbUpdater(db, apiToken, loggerFactory);
        await updater.UpdateStockSymbolAsync();

        var symbols = await db.StockSymbols.Where(s => s.Type == "Common Stock" && (s.Exchange == "NYSE" || s.Exchange == "NASDAQ")).Select(s => s.Symbol).ToListAsync();
        if (symbols == null)
        {
            Console.WriteLine("No symbols");
            return;
        }

        // Get 100 samples
        int step = symbols.Count / 100;
        int counter = 0;
        DateOnly fromDate = DateOnly.FromDateTime(DateTime.Now).AddYears(-5);
        foreach (var symbol in symbols)
        {
            if (counter % step == 0)
            {
                await updater.UpdateStockPriceAsync(symbol, fromDate: fromDate);
            }

            counter++;
        }
    }
}