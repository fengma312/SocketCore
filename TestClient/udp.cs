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

public class Udp
{
    public void UdpClient(string ip, int port)
    {
        UdpClients udpclient = new UdpClients(1024);
        udpclient.OnReceive += (byte[] arg2, int arg3, int arg4) =>
        {
            Console.WriteLine("客户端接收长度：" + arg4);
            //udpServer.Send( arg2, arg3, arg4);
        };
        udpclient.OnSend += (int arg2) =>
        {
            Console.WriteLine("客户端发送长度：" + arg2);
        };
        udpclient.Start(ip, port);

    }
}