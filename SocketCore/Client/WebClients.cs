using System.Text;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SocketCore.Client;

/// <summary>
/// websocker客户连接端 帮助类
/// </summary>
public class WebClients
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
    /// websocket地址
    /// </summary>
    private string url;
    /// <summary>
    /// 重连次数,每次休眠间隔休眠auto_connect*5秒
    /// </summary>
    private volatile int auto_connect;
    /// <summary>
    /// 连接成功事件
    /// </summary>
    public event Action? OnOpen;
    /// <summary>
    /// 断开事件
    /// </summary>
    public event Action? OnClose;
    /// <summary>
    /// 错误事件
    /// </summary>
    public event Action<Exception>? OnError;
    /// <summary>
    /// 接收消息事件 string str = System.Text.Encoding.UTF8.GetString(byte, 0, byte.Length);
    /// </summary>
    public event Action<byte[]>? OnReceive;
    /// <summary>
    /// 客户端连接对象
    /// </summary>
    /// <returns></returns>
    public ClientWebSocket websocket = null!;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="url">websocket服务地址</param>
    /// <param name="logger">日志接口</param>
    public WebClients(string url, ILogger? logger = null)
    {
        this.url = url;
        this.logger = logger ?? NullLogger.Instance;
    }

    /// <summary>
    /// 打开连接
    /// </summary>
    /// <param name="auto_connect">重连次数,每次重连休眠间隔为auto_connect*5秒</param>
    public async Task Open(int auto_connect = 3)
    {
        this.auto_connect = auto_connect;
        await this.Handle();
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public async Task Close()
    {
        this.auto_connect = 0;
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
        }
        websocket.Dispose();
    }

    /// <summary>
    /// 发送消息  byte[] data = Encoding.UTF8.GetBytes(str);
    /// </summary>
    /// <param name="data">发送数据</param>
    /// <param name="message_type">发送数据类型</param>
    /// <returns>是否已发送(注:不确保发送成功)</returns>
    public async Task<bool> Send(byte[] data, WebSocketMessageType message_type = WebSocketMessageType.Text)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendAsync(new ArraySegment<byte>(data), message_type, true, CancellationToken.None);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 逻辑处理
    /// </summary>
    /// <returns></returns>
    private async Task Handle()
    {
        new Thread(async () =>
        {
            for (int i = 1; i <= this.auto_connect; i++)
            {
                try
                {
                    websocket = new ClientWebSocket();
                    await websocket.ConnectAsync(new Uri(this.url), CancellationToken.None);
                    if (websocket.State == WebSocketState.Open && this.OnOpen != null)
                    {
                        this.OnOpen.Invoke();
                    }
                    List<byte> cache = new List<byte>();
                    ArraySegment<byte> seg = new ArraySegment<byte>(new byte[1024 * 10]);
                    WebSocketReceiveResult result = await websocket.ReceiveAsync(seg, CancellationToken.None);
                    while (result.CloseStatus.HasValue == false)
                    {
                        if (result.EndOfMessage)
                        {
                            cache.AddRange(seg.Take(result.Count).ToArray());
                            this.OnReceive?.Invoke(cache.ToArray());
                            cache.Clear();
                        }
                        else
                        {
                            cache.AddRange(seg.Take(result.Count).ToArray());
                        }
                        result = await websocket.ReceiveAsync(seg, CancellationToken.None);
                    }
                    this.OnClose?.Invoke();
                }
                catch (Exception ex)
                {
                    this.logger.LogError(this.event_id, ex, $"url:{this.url},websocket连接失败!");
                    this.OnError?.Invoke(ex);
                }
                await Task.Delay(i * 1000 * 5);
            }
        })
        {
            IsBackground = true,
            Priority = ThreadPriority.Highest,
        }.Start();
        await Task.Delay(10);
    }

}
