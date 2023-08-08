namespace z018.EodHistoricalDataTests;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using z018.EodHistoricalDataTests.Helper;

public class ApiClient_GetExchangeSymbolListTest
{
    private readonly ApiClient client;
    private readonly ILogger logger;

    public ApiClient_GetExchangeSymbolListTest(ITestOutputHelper loggerHelper)
    {
        this.logger = loggerHelper.BuildLogger();
        this.client = new ApiClient("Test", this.logger, new HttpClient(new MockHttpResponseMessage(MockData.Messages)));
    }

    [Fact]
    public async Task GetExchangeSymbolListTest()
    {
        var result = await client.GetExchangeSymbolListAsync();
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
    }
}