using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace test.client;

public class Program
{
    public static ILogger logger = null!;

    static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory =
           LoggerFactory.Create(builder =>
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                }));

        logger = loggerFactory.CreateLogger("");








    }
}




