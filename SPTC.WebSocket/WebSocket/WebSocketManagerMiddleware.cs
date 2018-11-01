using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPTC.WebSockets
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;

        private WebSocketHandler _webSocketHander { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHander = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            string ipAddress = context.Connection.RemoteIpAddress.ToString();
            await _webSocketHander.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _webSocketHander.ReceiveAsync(socket, result, buffer);
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketHander.OnDisconnected(socket);
                    return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
                handleMessage(result, buffer);
            }
        }
    }
}
