using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SocketCore.Server;

namespace TestServer;

public class Udp
{
    public void UdpServer()
    {
        UdpServer udpServer = new UdpServer(1024);
        udpServer.Start(6666, true);
        udpServer.OnReceive += (EndPoint arg1, byte[] arg2, int arg3, int arg4) =>
        {
            //Console.WriteLine("服务端接收长度：" + arg4);
            udpServer.Send(arg1, arg2, arg3, arg4);
        };

        udpServer.OnSend += (EndPoint arg1, int arg2) =>
        {
            Console.WriteLine("服务端发送长度：" + arg2);
        };
    }
}