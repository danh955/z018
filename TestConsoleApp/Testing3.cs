namespace TestConsoleApp;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using z018.StockDb;
using z018.StockDb.Updater;

internal static class Testing3
{
    public static async Task Test(string apiToken, StockDbContext db, ILoggerFactory loggerFactory)
    {
        ////await db.Database.EnsureDeletedAsync();
        ////await db.Database.EnsureCreatedAsync();

        var updater = new StockDbUpdater(db, apiToken, loggerFactory);
        await updater.UpdateLastStockPrices(daysToBackfill: 2);
    }
}