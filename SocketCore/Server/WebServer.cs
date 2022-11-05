using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SocketCore.Server;

/// <summary>
/// WebSocket 服务端
/// </summary>
public class WebServer
{
    /// <summary>
    /// 日志接口
    /// </summary>
    private ILogger logger;
    /// <summary>
    /// 日志标签
    /// </summary>
    private EventId event_id = new EventId(0, "WebSocketHelper");
    /// <summary>
    /// websocker集合
    /// </summary>
    /// <returns></returns>
    public ConcurrentDictionary<int, WebSocket> server = new ConcurrentDictionary<int, WebSocket>();
    /// <summary>
    /// 连接标示 自增长
    /// </summary>
    private volatile int connectId;
    /// <summary>
    /// 断开事件
    /// </summary>
    public event Action<int>? OnClose;
    /// <summary>
    /// 错误事件
    /// </summary>
    public event Action<Exception>? OnError;
    /// <summary>
    /// 接收消息事件 string str = System.Text.Encoding.UTF8.GetString(byte, 0, byte.Length);
    /// </summary>
    public event Action<int, byte[]>? OnReceive;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="logger">日志接口</param>
    public WebServer(ILogger? logger = null)
    {
        this.logger = logger ?? NullLogger.Instance;
    }

    /// <summary>
    /// 发送消息  byte[] data = Encoding.UTF8.GetBytes(str);
    /// </summary>
    /// <param name="connectId">客户id</param>
    /// <param name="data">发送数据</param>
    /// <param name="message_type">发送数据类型</param>
    /// <returns>是否已发送returns>
    public async Task<bool> Send(int connectId, byte[] data, WebSocketMessageType message_type = WebSocketMessageType.Text)
    {
        if (server.TryGetValue(connectId, out WebSocket? websocket))
        {
            if (websocket.State == WebSocketState.Open)
            {
                await websocket.SendAsync(new ArraySegment<byte>(data), message_type, true, CancellationToken.None);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 初始化 成功连接一个客户端 HttpContext.WebSockets.AcceptWebSocketAsync() ,注:先判断此http请求为websocket
    /// </summary>
    /// <param name="websocker">websocker</param>
    /// <returns>客户id</returns>
    public int Accept(WebSocket websocker)
    {
        connectId += 1;
        server.TryAdd(connectId, websocker);
        new Thread(async () =>
        {
            try
            {
                List<byte> cache = new List<byte>();
                ArraySegment<byte> seg = new ArraySegment<byte>(new byte[1024 * 10]);
                WebSocketReceiveResult result = await websocker.ReceiveAsync(seg, CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    if (result.EndOfMessage)
                    {
                        cache.AddRange(seg.Take(result.Count).ToArray());
                        this.OnReceive?.Invoke(connectId, cache.ToArray());
                        cache.Clear();
                    }
                    else
                    {
                        cache.AddRange(seg.Take(result.Count).ToArray());
                    }
                    result = await websocker.ReceiveAsync(seg, CancellationToken.None);
                }
                await websocker.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                this.OnClose?.Invoke(connectId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(this.event_id, ex, $"websocket连接失败!");
                this.OnError?.Invoke(ex);
            }
        })
        {
            IsBackground = true,
            Priority = ThreadPriority.Highest,
        }.Start();
        return connectId;
    }

}
