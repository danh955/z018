namespace z018.StockDb.Updater;

using Microsoft.Extensions.Logging;
using z018.EodHistoricalData;

public partial class StockDbUpdater
{
    private const string DefaultExchangeCode = "US";

    private readonly StockDbContext db;
    private readonly ApiClient client;
    private readonly ILogger<StockDbUpdater>? logger;

    public StockDbUpdater(StockDbContext db, string apiToken, ILoggerFactory? loggerFactory = null)
    {
        this.db = db;
        client = new ApiClient(apiToken, loggerFactory?.CreateLogger<ApiClient>());
        logger = loggerFactory?.CreateLogger<StockDbUpdater>();
        logger?.LogInformation("Constructor");
    }

    /// <summary>
    /// Only weekdays.  Saturday and Sunday will be the date of Friday.
    /// </summary>
    /// <returns>Todays date.  Friday includes Saturday and Sunday.</returns>
    private static DateOnly TodaysWeekDate()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return today.DayOfWeek switch
        {
            DayOfWeek.Saturday => today.AddDays(-1),
            DayOfWeek.Sunday => today.AddDays(-2),
            _ => today,
        };
    }
}