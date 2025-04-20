using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Models;
using Agar.io_Alpfa.Entities;

namespace Agar.io_Alpfa.Controllers;
public class HomeController : Controller
{
    static List<Player> Players = new List<Player>();
    static List<WebSocket> ws = new List<WebSocket>();

    static Constants.Constants constants = new Constants.Constants();

    static int userGen = 0;
    static List<Food> Foods = EntitiesService.GetRandFoods(constants.CirclesCount);

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

    private async Task Echo(WebSocket webSocket)
    {
        Player player = new Player(userGen++);
        Players.Add(player);

        var time_call_back = new TimerCallback(GoToMouseCoord);
        var timer = new Timer(time_call_back, (player, webSocket), 0, constants.TimerPeriod);

        var task_1 = LoadMap();
        var task_2 = UpdateAll();
        var task_3 = UpdateCurrentPlayer(player, webSocket);

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
                    string data_x = data.ElementAt(1).Remove(0, 2);
                    string data_y = data.ElementAt(2).Remove(0, 2);

                    data_x = data_x.Replace('.', ',');
                    data_y = data_y.Replace(".", ",");

                    var new_x = double.Parse(data_x);
                    var new_y = double.Parse(data_y);

                    Move(player, new_x, new_y);

                }
                else if (data.First() == "new_size")
                {
                    int index = int.Parse(data.ElementAt(1));

                    NewSize(index, player, webSocket);

                }
                else if (data.First() == "eat_food")
                {
                    int index = int.Parse(data.ElementAt(1));

                    EatFood(index);
                }
                else
                {
                    ws.Remove(webSocket);
                    timer.Dispose();
                    await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
                    return;
                }

                await UpdateAll();

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

            }
        }
        catch
        {
            ws.Remove(webSocket);
            Console.WriteLine("резкий отрубон");
            timer.Dispose();
            return;
        }

    }
    private async Task UpdateAll()
    {
        foreach (var webSocket in ws)
        {
            String s = System.Text.Json.JsonSerializer.Serialize(Players);

            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

    }
    async Task UpdateCurrentPlayer(Player player, WebSocket ws)
    {
        String s = "UpdateCurrentPlayer " + System.Text.Json.JsonSerializer.Serialize(player);

        await ws.SendAsync(
            Encoding.ASCII.GetBytes(s).ToArray(),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
    async Task UpdateMap(Food c)
    {
        foreach (var webSocket in ws)
        {
            String s = "UpdateMap " + System.Text.Json.JsonSerializer.Serialize(c);
            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
    async Task DeleteFood(int index)
    {
        foreach (var webSocket in ws)
        {
            String s = "DeleteFood " + System.Text.Json.JsonSerializer.Serialize(index);
            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
    async Task LoadMap()
    {
        foreach (var webSocket in ws)
        {
            String s = "LoadMap " + System.Text.Json.JsonSerializer.Serialize(Foods);
            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }

    async void EatFood(int index)
    {
        Foods.RemoveAt(index);
        var task_1 = DeleteFood(index);

        var new_food = new Food();

        Foods.Add(new_food);
        var task_2 = UpdateMap(new_food);

        Task.WaitAll(task_1, task_2);
    }

    async void NewSize(int index, Player player, WebSocket webSocket)
    {
        Players.ElementAt(index).size += constants.FoodIncrease;
        var task_1 = UpdateCurrentPlayer(player, webSocket);
        Task.WaitAll(task_1);
    }
    async void Move(Player player, double new_x, double new_y)
    {
        player.mouse_x = new_x;
        player.mouse_y = new_y;
    }
    async void GoToMouseCoord(object obj)
    {
        (Player, WebSocket) data = ((Player, WebSocket))obj;
        var pos = data.Item1;
        var ws = data.Item2;

        if (ws.State != WebSocketState.Open) return;

        var pos_index = Players.IndexOf(pos);
        var player = Players[pos_index];

        var mouse_x = player.mouse_x;
        var mouse_y = player.mouse_y;

        if (mouse_x != 0 && mouse_y != 0 && Math.Pow(player.x - mouse_x, 2) + Math.Pow(player.y - mouse_y, 2) > Math.Pow(constants.PlayerMinLenToMouseForMove, 2))
        {

            double len_vector = Math.Sqrt(Math.Pow(player.x - mouse_x, 2) + Math.Pow(player.y - mouse_y, 2));
            var x_norm = Math.Abs(player.x - mouse_x) / len_vector;
            var y_norm = Math.Abs(player.y - mouse_y) / len_vector;

            if (mouse_x >= player.x)
            {
                player.x += x_norm * player.speed;
                player.mouse_x += x_norm * player.speed;
            }
            else
            {
                player.x -= x_norm * player.speed;
                player.mouse_x -= x_norm * player.speed;
            }

            if (mouse_y >= player.y)
            {
                player.y += y_norm * player.speed;
                player.mouse_y += y_norm * player.speed;
            }
            else
            {
                player.y -= y_norm * player.speed;
                player.mouse_y -= y_norm * player.speed;
            }

            Players[pos_index].x = player.x;
            Players[pos_index].y = player.y;

            var task_1 = UpdateCurrentPlayer(player, ws);
            var task_2 = UpdateAll();
            Task.WaitAll(task_1, task_2);
        }
    }
}
