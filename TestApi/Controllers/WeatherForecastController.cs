using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using SocketCore.Server;

namespace TestApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private WebServer server = new WebServer();
    /// <summary>
    /// websocket处理  WeatherForecast/WebSocketUI
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> WebSocketUI()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            server.Accept(webSocket);
            server.OnClose += (int connectId) =>
            {
                Console.WriteLine($"关闭:{connectId}");
            };
            server.OnError += (Exception ex) =>
            {
                Console.WriteLine($"关闭:{ex.Message}");
            };
            server.OnReceive += (int connectId, byte[] b) =>
            {
                Console.WriteLine($"接收:{connectId},内容:{System.Text.Encoding.UTF8.GetString(b, 0, b.Length)}");
            };
        }
        return new EmptyResult();
    }
}
