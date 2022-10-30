using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketCore.Server;

namespace test.client;

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
    public void Pack(int numConnections, int receiveBufferSize, int overtime, int port, uint headerFlag)
    {
        TcpPackServer server = new TcpPackServer(numConnections, receiveBufferSize, overtime, headerFlag);
        server.OnAccept += (int obj) =>
        {
            server.SetAttached(obj, 555);
            Console.WriteLine($"Pack已连接{obj}");
        };
        server.OnReceive += (int arg1, byte[] arg2) =>
        {
            int aaa = server.GetAttached<int>(arg1);
            Console.WriteLine($"Pack已接收:{arg1} 长度:{arg2.Length}");
            server.Send(arg1, arg2, 0, arg2.Length);
        };
        server.OnSend += (int arg1, int arg2) =>
        {

        };
        server.OnClose += (int obj) =>
        {
            int aaa = server.GetAttached<int>(obj);
            Console.WriteLine($"Pack断开{obj}");
        };
        server.Start(port);
    }



    /// <summary>
    /// 设置基本配置
    /// </summary>   
    /// <param name="numConnections">同时处理的最大连接数</param>
    /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
    /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
    ///<param name = "port" > 端口 </ param >
    public void Pull(int numConnections, int receiveBufferSize, int overtime, int port)
    {
        TcpPullServer server = new TcpPullServer(numConnections, receiveBufferSize, overtime);
        server.OnAccept += (int obj) =>
        {
            server.SetAttached(obj, 555);
            Console.WriteLine($"Pack已连接{obj}");
        };
        server.OnReceive += (int arg1, int arg2) =>
        {
            int aaa = server.GetAttached<int>(arg1);
            Console.WriteLine($"Pull已接收:{arg1} 长度:{arg2}");
            byte[] data = server.Fetch(arg1, server.GetLength(arg1));
            server.Send(arg1, data, 0, data.Length);
        };
        server.OnSend += (int arg1, int arg2) =>
        {

        };
        server.OnClose += (int obj) =>
        {
            int aaa = server.GetAttached<int>(obj);
            Console.WriteLine($"Pack断开{obj}");
        };
        server.Start(port);
    }

    /// <summary>
    /// 设置基本配置
    /// </summary>   
    /// <param name="numConnections">同时处理的最大连接数</param>
    /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
    /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
    /// <param name="port">端口</param>
    public void Push(int numConnections, int receiveBufferSize, int overtime, int port)
    {
        TcpPushServer server = new TcpPushServer(numConnections, receiveBufferSize, overtime);
        server.OnAccept += (int obj) =>
        {
            server.SetAttached(obj, 555);
            Console.WriteLine($"Pack已连接{obj}");
        };
        server.OnReceive += (int arg1, byte[] arg2) =>
        {
            ConcurrentDictionary<int, string> aa = server.ClientList;
            int aaa = server.GetAttached<int>(arg1);
            Console.WriteLine($"Push已接收:{arg1} 长度:{arg2.Length}");
            server.Send(arg1, arg2, 0, arg2.Length);
        };
        server.OnSend += (int arg1, int arg2) =>
        {

        };
        server.OnClose += (int obj) =>
        {
            int aaa = server.GetAttached<int>(obj);
            Console.WriteLine($"Pack断开{obj}");
        };
        server.Start(port);
    }




}