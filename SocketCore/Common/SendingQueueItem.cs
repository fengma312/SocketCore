using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SocketCore.Common;

/// <summary>
/// 发送消息体
/// </summary>
internal class SendingQueue
{
    /// <summary>
    /// 连接标记
    /// </summary>
    internal int connectId { get; set; }
    /// <summary>
    /// 远程IP和端口
    /// </summary>
    internal EndPoint remoteEndPoint { get; set; } = null!;
    /// <summary>
    /// 发送的数据
    /// </summary>
    internal byte[] data { get; set; } = null!;
    /// <summary>
    /// 偏移位
    /// </summary>
    internal int offset { get; set; }
    /// <summary>
    /// 长度
    /// </summary>
    internal int length { get; set; }
}

