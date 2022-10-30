using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace test.client;

public class Program
{
    public static ILogger logger = null!;
    int port = 5555;
    int numConnections = 20000;
    int receiveBufferSize = 1024;
    int overtime = 20;



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
        Program program = new Program();
        program.Test();

        logger.LogInformation("结束");
        Console.Read();




    }

    public void Test()
    {
        Task.Run(() =>
        {
            Tcp tcp = new Tcp();
            tcp.Pack(numConnections, receiveBufferSize, overtime, port, 0xff);
        });
    }



}





