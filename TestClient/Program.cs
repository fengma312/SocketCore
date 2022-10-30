using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace TestClient;

public class Program
{
    public static ILogger logger = null!;
    int port = 5555;
    int receiveBufferSize = 1024;
    string ip = "127.0.0.1";


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
        // program.Pack();
        program.Push();
        // program.Pull();
        // program.Udp();
        // program.Web();
        logger.LogInformation("结束");
        Console.Read();




    }

    public void Pack()
    {
        Task.Run(() =>
        {
            Tcp tcp = new Tcp();
            tcp.Pack(receiveBufferSize, ip, port, 0xff);
        });
    }

    public void Push()
    {
        Task.Run(() =>
        {
            Tcp tcp = new Tcp();
            tcp.Push(receiveBufferSize, ip, port);
        });
    }

    public void Pull()
    {
        Task.Run(() =>
        {
            Tcp tcp = new Tcp();
            tcp.Pull(receiveBufferSize, ip, port);
        });
    }

    public void Udp()
    {
        Task.Run(() =>
        {
            Udp tcp = new Udp();
            tcp.UdpClient(ip, port);
        });
    }

    public void Web()
    {
        Task.Run(() =>
        {
            Web tcp = new Web();
            tcp.WebClient("ws://127.0.0.1:5555");
        });
    }

}





