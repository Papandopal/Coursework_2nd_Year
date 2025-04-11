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
    public double mouse_x { get; set; }
    public double mouse_y { get; set; }
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
        curPos.x = GetRandCoord();
        curPos.size = 20;
        curPos.y = GetRandCoord();
        curPos.user = userGen++;
        curPos.speed = 3;
        curPos.mouse_x = -1;
        curPos.mouse_y = -1;

        var time_call_back = new TimerCallback(GoToMouseCoord);
        var timer = new Timer(time_call_back, (curPos, webSocket), 1000, 20);

        posUpdates.Add(curPos);
        
        await UpdateAll();

        await UpdateCurrentUserScreen(curPos, webSocket);

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
                string data_x = data.ElementAt(1).Remove(0, 2);
                string data_y = data.ElementAt(2).Remove(0, 2);
                data_x = data_x.Replace('.', ',');
                data_y = data_y.Replace(".", ",");
                curPos.mouse_x = double.Parse(data_x);
                curPos.mouse_y = double.Parse(data_y);
                Console.WriteLine(data_x);
                Console.WriteLine(data_y);
                
            }

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
        (PosUpdate, WebSocket) data = ((PosUpdate, WebSocket))obj;
        var pos = data.Item1;
        var ws = data.Item2;
        
        var pos_index = posUpdates.IndexOf(pos);
        var curPos = posUpdates[pos_index];

        var mouse_x = curPos.mouse_x;
        var mouse_y = curPos.mouse_y;

        Console.WriteLine(curPos.x.ToString() + ' ' + curPos.y.ToString() + ' ' + mouse_x.ToString() + ' ' + mouse_y.ToString() + curPos.size.ToString());
        if (mouse_x!=-1 && mouse_y!=-1 && Math.Pow(curPos.x - mouse_x, 2) + Math.Pow(curPos.y - mouse_y, 2) > (curPos.size*curPos.size))
        {
            
            double len_vector = Math.Sqrt(Math.Pow(curPos.x - mouse_x, 2) + Math.Pow(curPos.y - mouse_y, 2));
            var x_norm = Math.Abs(curPos.x - mouse_x) / len_vector;
            var y_norm = Math.Abs(curPos.y - mouse_y) / len_vector;

            if (mouse_x >= curPos.x) curPos.x += x_norm * curPos.speed;
            else curPos.x -= x_norm * curPos.speed;
            if (mouse_y >= curPos.y) curPos.y += y_norm * curPos.speed;
            else curPos.y -= y_norm * curPos.speed;

            Console.WriteLine("изменение на: "+curPos.x.ToString() + ' ' + curPos.y.ToString());

            posUpdates[pos_index].x = curPos.x;
            posUpdates[pos_index].y = curPos.y;

            Console.WriteLine("изменение на(вторая попытка): " + posUpdates[pos_index].x.ToString() + ' ' + posUpdates[pos_index].y.ToString());

            await UpdateCurrentUserScreen(curPos, ws);

            await UpdateAll();
        }
    }
    async Task UpdateCurrentUserScreen(PosUpdate curPos, WebSocket ws)
    {
        String s = "UpdateCurrentUserScreen "+System.Text.Json.JsonSerializer.Serialize(curPos);
        await ws.SendAsync(
            Encoding.ASCII.GetBytes(s).ToArray(),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
    
    int GetRandCoord()
    {
        var rand = new Random();
        return rand.Next(1000, 4000);
    }
}
