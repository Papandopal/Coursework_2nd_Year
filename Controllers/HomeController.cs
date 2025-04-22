using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Models;
using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.Constants;

namespace Agar.io_Alpfa.Controllers;
public class HomeController : Controller
{
    static ServerModel model = new ServerModel();
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
            model.WebSockets.Add(webSocket);    
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Echo(WebSocket webSocket)
    {
        Player player = new Player(userGen++);
        model.Players.Add(player);

        var time_call_back = new TimerCallback(GoToMouseCoord);
        var timer = new Timer(time_call_back, (player, webSocket), 0, Const.TimerPeriod);

        var task_1 = model.LoadMap();
        var task_2 = model.UpdateAll();
        var task_3 = model.UpdateCurrentPlayer(player.user_id);

        Task.WaitAll(task_1, task_2, task_3);

        try
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                String ss = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);

                var data = ss.Split();

                if (data.First() == "move")
                {
                    string data_index = data.ElementAt(2);
                    string data_x = data.ElementAt(4);
                    string data_y = data.ElementAt(6);

                    var index = int.Parse(data_index);
                    double new_x, new_y;
                    data_x = data_x.Replace('.', ',');
                    data_y = data_y.Replace(".", ",");

                    try
                    { 
                        new_x = double.Parse(data_x);
                        new_y = double.Parse(data_y);
                    }
                    catch
                    {
                        data_x = data_x.Replace(',', '.');
                        data_y = data_y.Replace(",", ".");

                        new_x = double.Parse(data_x);
                        new_y = double.Parse(data_y);
                    }

                    model.Move(index, new_x, new_y);

                }
                else if (data.First() == "new_size")
                {
                    int index = int.Parse(data.ElementAt(1));

                    model.NewSize(index);

                }
                else if (data.First() == "eat_food")
                {
                    int index = int.Parse(data.ElementAt(1));

                    model.EatFood(index);
                }
                else if(data.First() == "disconnection")
                {
                    timer.Dispose();
                    model.WebSockets.Remove(webSocket);
                    //await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
                    model.Players.Remove(player);

                    for(int i = player.user_id; i < model.Players.Count; i++)
                    {
                        --model.Players[i].user_id;
                    }

                    return;
                }

                await model.UpdateAll();

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

            }
        }
        catch(Exception e)
        {
            model.WebSockets.Remove(webSocket);
            Console.WriteLine(e);
            timer.Dispose();
            return;
        }

    }
    async void GoToMouseCoord(object obj)
    {
        (Player, WebSocket) data = ((Player, WebSocket))obj;
        var pos = data.Item1;
        var ws = data.Item2;

        if (ws.State != WebSocketState.Open) return;

        var pos_index = model.Players.IndexOf(pos);
        var player = model.Players[pos_index];

        var mouse_x = player.mouse_x;
        var mouse_y = player.mouse_y;

        if (mouse_x!=player.x && mouse_y!=player.y)
        {
            double len_vector = Math.Sqrt(Math.Pow(player.x - mouse_x, 2) + Math.Pow(player.y - mouse_y, 2));
            var x_norm = Math.Abs(player.x - mouse_x) / len_vector;
            var y_norm = Math.Abs(player.y - mouse_y) / len_vector;

            if (mouse_x > player.x)
            {
                player.x += x_norm * player.speed;
                player.mouse_x += x_norm * player.speed;
            }
            else
            {
                player.x -= x_norm * player.speed;
                player.mouse_x -= x_norm * player.speed;
            }

            player.x = Math.Min(Math.Max(player.x, 0), Const.MapWidth);
            player.mouse_x = Math.Min(Math.Max(player.mouse_x, 0), Const.MapWidth);

            if (mouse_y > player.y)
            {
                player.y += y_norm * player.speed;
                player.mouse_y += y_norm * player.speed;
            }
            else
            {
                player.y -= y_norm * player.speed;
                player.mouse_y -= y_norm * player.speed;
            }

            player.y = Math.Min(Math.Max(player.y, 0), Const.MapHeight);
            player.mouse_y = Math.Min(Math.Max(player.mouse_y, 0), Const.MapHeight);

            model.Players[pos_index] = player;
            Console.WriteLine("move");
            var task_1 = model.UpdateCurrentPlayer(pos_index);
            var task_2 = model.UpdateAll();
            Task.WaitAll(task_1, task_2);
            Console.WriteLine("move-end");
        }
    }
}
