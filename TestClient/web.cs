using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SocketCore.Client;
using SocketCore.Server;

namespace TestClient;

public class Web
{
    public void WebClient(string url)
    {
        WebClients udpclient = new WebClients(url);
        udpclient.OnOpen += () =>
        {
            Console.WriteLine("客户端接收长度：");
        };
        udpclient.OnReceive += (byte[] arg2) =>
        {
            Console.WriteLine("客户端接收长度：" + arg2.Length);
            //udpServer.Send( arg2, arg3, arg4);
        };
        udpclient.OnClose += () =>
        {

        };
        udpclient.Open(1).Wait();

    }
}