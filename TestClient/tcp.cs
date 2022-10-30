using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketCore.Client;


namespace TestClient;

public class Tcp
{

    /// <summary>
    /// 设置基本配置
    /// </summary>   
    /// <param name="numConnections">同时处理的最大连接数</param>
    /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
    /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
    /// <param name="port">端口</param>
    /// <param name="headerFlag">包头</param>
    public void Pack(int receiveBufferSize, string ip, int port, uint headerFlag)
    {
        TcpPackClient client = new TcpPackClient(receiveBufferSize, headerFlag);
        client.OnConnect += (bool obj) =>
        {
            Console.WriteLine($"pack连接{obj}");
            Console.WriteLine($"Pack已连接{obj}");

            Task.Run(() =>
            {
                string senddata = "aaaaaaaaaaaaa";
                byte[] data = Encoding.UTF8.GetBytes(senddata);
                client.Send(data, 0, data.Length);
            });
        };
        client.OnReceive += (byte[] obj) =>
        {
            Console.WriteLine($"pack接收byte[{obj.Length}]");
        };
        client.OnSend += (int obj) =>
        {
            Console.WriteLine($"pack已发送长度{obj}");
        };
        client.OnClose += () =>
        {
            Console.WriteLine($"pack断开");
        };
        client.Connect(ip, port);
    }




    /// <summary>
    /// 设置基本配置
    /// </summary>   
    /// <param name="numConnections">同时处理的最大连接数</param>
    /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
    /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
    ///<param name = "port" > 端口 </ param >
    public void Pull(int receiveBufferSize, string ip, int port)
    {
        TcpPullClient client = new TcpPullClient(receiveBufferSize);
        client.OnConnect += (bool obj) =>
        {
            Console.WriteLine($"pull连接{obj}");
        };
        client.OnReceive += (int obj) =>
        {
            byte[] data = client.Fetch(obj);
            Console.WriteLine($"pull接收byte[{data.Length}]");
        };
        client.OnSend += (int obj) =>
        {
            Console.WriteLine($"Push已发送长度{obj}");
        };
        client.OnClose += () =>
        {
            Console.WriteLine($"pull断开");
        };
        client.Connect(ip, port);
    }

    /// <summary>
    /// 设置基本配置
    /// </summary>   
    /// <param name="numConnections">同时处理的最大连接数</param>
    /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
    /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
    /// <param name="port">端口</param>
    public void Push(int receiveBufferSize, string ip, int port)
    {
        TcpPushClient client = new TcpPushClient(receiveBufferSize);
        client.OnConnect += (bool obj) =>
        {
            Console.WriteLine($"Push连接{obj}");
        };
        client.OnReceive += (byte[] obj) =>
        {
            Console.WriteLine($"Push接收长度[{obj.Length}]");
        };
        client.OnSend += (int obj) =>
        {
            Console.WriteLine($"Push已发送长度{obj}   ");
        };
        client.OnClose += () =>
        {
            Console.WriteLine($"Push断开");
        };
        client.Connect(ip, port);
    }




}