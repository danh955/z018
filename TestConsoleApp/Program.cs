using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestConsoleApp;
using z018.StockDb;

Console.WriteLine("Start");

var config = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

string connectionString = config.GetConnectionString("SqlDatabase") ?? throw new NullReferenceException("ConnectionString SqlDatabase");
var apiToken = config.GetValue<string>("EODHistoricalDataSettings:ApiToken") ?? throw new NullReferenceException("EODHD");

using ILoggerFactory loggerFactory = LoggerFactory.Create(logging =>
    {
        logging.AddFilter("Microsoft", LogLevel.Warning);
        logging.AddFilter("System", LogLevel.Warning);
        logging.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
    });

var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>()
                    .UseSqlServer(connectionString)
                    .UseLoggerFactory(loggerFactory);
using StockDbContext db = new(optionsBuilder.Options);

//// await Testing2.Test(apiToken, db, loggerFactory);
await Testing3.Test(apiToken, db, loggerFactory);

Console.WriteLine("End");