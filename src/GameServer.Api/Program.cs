using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GameServer.Application;
using GameServer.Application.Users.Connect;
using GameServer.Infrastructure;
using GameServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("GameServer")
    ?? "Data Source=gameserver.db";

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.UseWebSockets();

// Apply migrations on startup so schema stays aligned with EF model changes.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameServerDbContext>();
    await db.Database.MigrateAsync();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    using var socket = await context.WebSockets.AcceptWebSocketAsync();
    using var scope = context.RequestServices.CreateScope();
    var connectHandler = scope.ServiceProvider.GetRequiredService<ConnectUserHandler>();

    var request = await ReceiveConnectRequestAsync(socket, context.RequestAborted);
    if (request is null)
    {
        await SendErrorAsync(socket, "INVALID_MESSAGE", "Invalid connect payload.", context.RequestAborted);
        await socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid connect payload.", context.RequestAborted);
        return;
    }

    var result = await connectHandler.HandleAsync(request, context.RequestAborted);
    if (!result.IsSuccess)
    {
        await SendErrorAsync(socket, result.ErrorCode ?? "INTERNAL_ERROR", result.ErrorMessage ?? "Unknown error", context.RequestAborted);
        await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, result.ErrorCode, context.RequestAborted);
        return;
    }

    var payload = JsonSerializer.Serialize(new
    {
        type = "connected",
        userId = result.UserId,
        name = result.Name,
        isNewUser = result.IsNewUser
    });

    await socket.SendAsync(Encoding.UTF8.GetBytes(payload), WebSocketMessageType.Text, true, context.RequestAborted);
    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connected", context.RequestAborted);
});

app.Run();

static async Task<ConnectUserRequest?> ReceiveConnectRequestAsync(WebSocket socket, CancellationToken cancellationToken)
{
    var buffer = new byte[4096];
    var segment = new ArraySegment<byte>(buffer);
    WebSocketReceiveResult result;
    using var stream = new MemoryStream();

    do
    {
        result = await socket.ReceiveAsync(segment, cancellationToken);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            return null;
        }

        await stream.WriteAsync(buffer.AsMemory(0, result.Count), cancellationToken);
    }
    while (!result.EndOfMessage);

    var json = Encoding.UTF8.GetString(stream.ToArray());

    try
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("type", out var typeElement) || typeElement.GetString() != "connect")
        {
            return null;
        }

        var userId = doc.RootElement.TryGetProperty("userId", out var userIdElement)
            ? userIdElement.GetString()
            : null;

        return new ConnectUserRequest(userId);
    }
    catch (JsonException)
    {
        return null;
    }
}

static Task SendErrorAsync(WebSocket socket, string code, string message, CancellationToken cancellationToken)
{
    var payload = JsonSerializer.Serialize(new { type = "error", code, message });
    return socket.SendAsync(Encoding.UTF8.GetBytes(payload), WebSocketMessageType.Text, true, cancellationToken);
}
