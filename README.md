socket.core
这是一个基于C# .net standard2.0 写的socket框架，可使用于.net Framework/dotnet core程序集，能在window(IOCP)/linux(epoll)运行.使用异步连接，异步发送，异步接收，性能爆表，并且通过压力测试。
安装NuGet: https://www.nuget.org/packages/socket.core/


一：TCP模块介绍

服务端所在socket.core.Server命名空间下，分别为三种模式 push/pull/pack
客户端所在socket.core.Client命名空间下，分别为三种模式 push/pull/pack

主要流程与对应的方法和事件介绍.
注:connectId(int)代表着一个连接对象,data(byte[]),success(bool)

1.初始化TCP实现类(对应的三种模式)
实例化服务端类 TcpPushServer/TcpPullServer/TcpPackServer
实例化客户端类 TcpPushClient/TcpPullClient/TcpPackClient
参数介绍int numConnections同时处理的最大连接数,int receiveBufferSize用于每个套接字I/O操作的缓冲区大小(接收端), int overtime超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时,uint headerFlag包头标记范围0~1023(0x3FF),当包头标识等于0时，不校验包头

2.启动监听/连接服务器
服务端 server.Start(port);
客户端 client.Connect(ip,port);

3.触发连接事件
服务端 server.OnAccept(connectId); 接收到一个连接id,可用他来发送，接收，关闭的标记
客户端 client.OnConnect(success); 接收是否成功连接到服务器

4.发送消息
服务端 server.Send(connectId,data,offset,length);
客户端 client.Send(data,offset,length);

5.触发已发送事件
服务端 server.OnSend(connectId,length);
客户端 client.OnSend(length);

6.触发接收事件
服务端 server.OnReceive(connectId, data);
客户端 client.OnReceive(data);

7.关闭连接
服务端 server.Close(connectId);
客户端 client.Close();

8.触发关闭连接事件
服务端 server.OnClose(connectId);
客户端 client.OnClose();

三种模型简介

一:push
当接收到数据时会触发监听事件OnReceive(connectId,data);把数据立马“推”给应用程序

二:pull
当接收到数据时会触发监听事件OnReceive(connectId,length)，告诉应用程序当前已经接收到了多少数据长度，应用程序可使用GetLength(connectId)方法检查已接收的数据的长度，如果满足则调用组件的Fetch(connectId,length)方法，把需要的数据“拉”出来

三:pack
pack模型组件是push和pull模型的结合体，应用程序不必要处理分包/合包，组件保证每个server.OnReceive(connectId,data)/client.OnReceive(data)事件都向应用程序提供一个完整的数据包
注：pack模型组件会对应用程序发送的每个数据包自动加上4个字节(32bit)的包头，组件接收到数据时，根据包头信息自动分包，每个完整的数据包通过OnReceive(connectId, data)事件发送给应用程序
PACK包头格式(4字节)4*8=32
XXXXXXXXXXYYYYYYYYYYYYYYYYYYYYYY
前10位X为包头标识位，用于数据包校验，有效包头标识取值范围0~1023(0x3FF),当包头标识等于0时，不校验包头，后22位Y为长度位，记录包体长度。有效数据包最大长度不能超过4194303（0x3FFFFF）字节(byte),应用程序可以通过TcpPackServer/TcpPackClient构造函数参数headerFlag设置

服务端其它方法介绍

bool SetAttached(int connectId, object data)
服务端为每个客户端设置附加数据，避免用户自己再建立用户映射表

T GetAttached(int connectId)
获取指定客户端的附加数据

属性:ConcurrentDictionary<int, string> ClientList
获取正在连接的客户端信息<connectId,ip和端口>

二：UDP模块介绍

1.初始化UDP实现类UdpServer/UdpClients

服务端socket.core.Server.UdpServer
客户端socket.core.Client.UdpClients
参数int receiveBufferSize用于每个套接字I/O操作的缓冲区大小(接收端)

2.发送数据

服务端 server.Send(remoteEndPoint,data,offset,length)
客户端 client.Send(data,offset,length)
客户端 client.Send(remoteEndPoint,data,offset,length)

3.触发已发送事件

服务端 server.OnSend(remoteEndPoint,length)
客户端 client.OnSend(length)

3.触发接收事件

服务端 server.OnReceive(remoteEndPoint,data,offset,length)
客户端 client.OnReceive(data,offset,length)

2017/12/27

技术在于分享,大家共同进步