﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace tanka.graphql.server.webSockets
{
    public class WebSocketServer
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<WebSocketServer> _logger;

        public WebSocketServer(
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<WebSocketServer>();
        }

        public ConcurrentDictionary<string, MessageServer> Clients { get; } =
            new ConcurrentDictionary<string, MessageServer>();

        public async Task ProcessRequestAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation($"Processing WebSocket: {context.TraceIdentifier}");
                var connection = new WebSocketPipe(_loggerFactory);
                var protocol = context.RequestServices
                    .GetRequiredService<IProtocolHandler>();

                var messageServer = new SubscriptionServer(protocol);

                Clients.TryAdd(context.TraceIdentifier, messageServer);
                var run = messageServer.RunAsync(connection, context.RequestAborted);
                await connection.ProcessRequestAsync(context);
                await run;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Processing websocket failed: {context.TraceIdentifier}");
                throw;
            }
            finally
            {
                if (Clients.TryRemove(context.TraceIdentifier, out var s))
                {
                    s.Complete();
                    await s.Completion;
                }
                _logger.LogInformation($"Processing websocket finished: {context.TraceIdentifier}");
            }
        }
    }
}