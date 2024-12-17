using Microsoft.AspNetCore.WebSockets;
using WebLab1;
using Fleck;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(120);
});

var app = builder.Build();

app.UseStaticFiles();
app.UseWebSockets();

app.MapGet("/", context =>
{
    context.Response.Redirect("/admin"); // Ustawienie domyœlnej strony
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseRouting();
app.MapRazorPages();


var socketServer = new WebSocketServer("ws://127.0.0.1:2137/ws");
List<IWebSocketConnection> connections = new List<IWebSocketConnection>();
WebSocketHandler wsHandler = new WebSocketHandler();
socketServer.Start(connection =>
{
    connections.Add(connection);

    connection.OnOpen = () =>
    {
        Console.WriteLine("Websocket is working");
    };

    connection.OnMessage = message =>
    {
        try
        {
            JObject jsonMessage = JObject.Parse(message);
            string response = wsHandler.HandleMessage(jsonMessage);
            if (!jsonMessage["type"].ToString().Contains("fetch"))
            {
                foreach (var conn in connections)
                {
                    if (conn.IsAvailable)
                    {
                        conn.Send(response);
                    }
                }
            }
            else
            {
                connection.Send(response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while processing message: " + ex.Message);
        }
    };

    connection.OnClose = () =>
    {
        connections.Remove(connection);
        Console.WriteLine("Websocket connection closed");
    };
});

app.Run();