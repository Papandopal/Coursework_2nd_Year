using System.Diagnostics;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

record PosUpdate
{
    public double x { get; set; }
    public double y { get; set; }
    public int user { get; set; }
    public double size { get; set; }
    public double speed { get; set; } 
    public String type { get; set; } = "pos";
};

public class HomeController : Controller
{
    static List<PosUpdate> posUpdates = new List<PosUpdate>();
    static List<WebSocket> ws = new List<WebSocket>();

    static int userGen = 0;

    private readonly ILogger<HomeController> _logger;

    private double mouse_x = -1, mouse_y = -1;

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
        var rand = new Random();
        PosUpdate curPos = new PosUpdate();
        curPos.x = rand.Next(0, 1200);
        curPos.size = 20;
        curPos.y = rand.Next(0, 600);
        curPos.user = userGen++;
        curPos.speed = 3;

        await UpdateAll();

        var time_call_back = new TimerCallback(GoToMouseCoord);
        var timer = new Timer(time_call_back, curPos, 1000, 20);

        posUpdates.Add(curPos);

        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            String ss = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);
            Console.WriteLine("<<< " + ss);

            var data = ss.Split();

            if (data.First() == "move")
            { 
                mouse_x = double.Parse(data.ElementAt(1).Remove(0, 2));
                mouse_y = double.Parse(data.ElementAt(2).Remove(0, 2));
                
            }

            /*
            if (ss == "moveW") curPos.y -= 5;
            if (ss == "moveS") curPos.y += 5;
            if (ss == "moveA") curPos.x -= 5;
            if (ss == "moveD") curPos.x += 5;
            if (ss == "move+") curPos.size -= 5;
            if (ss == "move-") curPos.size += 5;
            */
            await UpdateAll();

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

        }
        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    async void GoToMouseCoord(object obj)
    {
        PosUpdate curPos = (PosUpdate)obj;

        if (mouse_x!=-1 && mouse_y!=-1 && Math.Pow(curPos.x - mouse_x, 2) + Math.Pow(curPos.y - mouse_y, 2) > (curPos.size*curPos.size))
        {
            
            double len_vector = Math.Sqrt(Math.Pow(curPos.x - mouse_x, 2) + Math.Pow(curPos.y - mouse_y, 2));
            var x_norm = Math.Abs(curPos.x - mouse_x) / len_vector;
            var y_norm = Math.Abs(curPos.y - mouse_y) / len_vector;

            if (mouse_x >= curPos.x) curPos.x += x_norm * curPos.speed;
            else curPos.x -= x_norm * curPos.speed;
            if (mouse_y >= curPos.y) curPos.y += y_norm * curPos.speed;
            else curPos.y -= y_norm * curPos.speed;
            
            //curPos.x += (mouse_x - curPos.x) * curPos.size;
            //curPos.y += (mouse_y - curPos.y) * curPos.size;
            await UpdateAll();
        }
    }
}
