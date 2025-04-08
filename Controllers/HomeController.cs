using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

record PosUpdate
{
    public float x { get; set; }
    public float y { get; set; }
    public int user { get; set; }
    public float size { get; set; }
    public String type { get; set; } = "pos";
};

public class HomeController : Controller
{
    static List<PosUpdate> posUpdates = new List<PosUpdate>();
    static List<WebSocket> ws = new List<WebSocket>();

    static int userGen = 0;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            ws.Add(webSocket);
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task UpdateAll()
    {
        foreach (var webSocket in ws)
        {

            String s = System.Text.Json.JsonSerializer.Serialize(posUpdates);
            Console.WriteLine(">>> " + s);
            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

    }

    private async Task Echo(WebSocket webSocket)
    {
        PosUpdate curPos = new PosUpdate();
        curPos.x = 10;
        curPos.size = 20;
        curPos.y = 10;
        curPos.user = userGen++;

        posUpdates.Add(curPos);


        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            String ss = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);
            Console.WriteLine("<<< " + ss);

            if (ss == "moveW") curPos.y -= 5;
            if (ss == "moveS") curPos.y += 5;
            if (ss == "moveA") curPos.x -= 5;
            if (ss == "moveD") curPos.x += 5;
            if (ss == "move+") curPos.size -= 5;
            if (ss == "move-") curPos.size += 5;

            await UpdateAll();

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
