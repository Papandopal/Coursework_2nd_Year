using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Models;
using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.RulesNameSpace;
using Agar.io_Alpfa.Services;

namespace Agar.io_Alpfa.Controllers;
public class GameController : Controller
{
    static GameModel game_model = new GameModel();
    static int userGen = 0;

    private readonly ILogger<GameController> _logger;

    public GameController(ILogger<GameController> logger)
    {
        _logger = logger;
        game_model.Foods = EntitiesService.GetRandFoods(Rules.CirclesCount);
    }

    public IActionResult Game(string Name)
    {
        ViewBag.Name = Name;
        return View();
    }
    public IActionResult EndGame(string name)
    {
        ViewBag.Name = name;
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
            await DialogWith(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task DialogWith(WebSocket webSocket)
    {
        Player player = new Player(userGen++, webSocket);
        player.name = await game_model.GetName(webSocket);
        game_model.Players.Add(player);

        var task_1 = game_model.UpdateCurrentPlayer(player.user_id);
        var task_2 = game_model.LoadMap();
        var task_3 = game_model.UpdateAllPlayers();
        Task.WaitAll(task_1, task_2, task_3);

        var time_call_back = new TimerCallback(GoToMouseCoord);
        var timer = new Timer(time_call_back, player, 0, Rules.TimerPeriod);

        try
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                String ss = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);

                var data = ss.Split();

                if (data.First() == "mousemove")
                {
                    string data_index = data.ElementAt(2);
                    string data_x = data.ElementAt(4);
                    string data_y = data.ElementAt(6);

                    var index = int.Parse(data_index);
                    double new_x, new_y;

                    new_x = double.Parse(data_x);
                    new_y = double.Parse(data_y);

                    game_model.MouseMove(index, new_x, new_y);

                }
                else if (data.First() == "new_size")
                {
                    int id = int.Parse(data.ElementAt(1));

                    game_model.NewSize(id);

                }
                else if (data.First() == "eat_food")
                {
                    int index = int.Parse(data.ElementAt(2));

                    game_model.EatFood(index);
                }
                else if (data.First() == "disconnect")
                {
                    timer.Dispose();
                    game_model.Players.Remove(player);
                    await game_model.UpdateAllPlayers();
                    return;
                }
                else if (data.First() == "kill")
                {

                    int victim_id = int.Parse(data.ElementAt(2));
                    int killer_id = int.Parse(data.ElementAt(4));
                    game_model.NewSize(victim_id, killer_id);
                    game_model.ResetPlayer(victim_id);
                }

                await game_model.UpdateAllPlayers();

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

            }
        }
        catch (Exception e)
        {
            game_model.Players.Remove(player);
            Console.WriteLine(e);
            timer.Dispose();
            return;
        }

    }
    void GoToMouseCoord(object obj)
    {
        Player data = (Player)obj;

        var ws = data.connection;

        if (ws.State != WebSocketState.Open) return;

        var player = game_model.Players.Where(item => item.user_id == data.user_id).FirstOrDefault();
        if (player == null) return;
        var pos_index = game_model.Players.IndexOf(player);

        var mouse_x = player.mouse_x;
        var mouse_y = player.mouse_y;


        if (mouse_x != player.x && mouse_y != player.y)
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

            player.x = Math.Min(Math.Max(player.x, 0), Rules.MapSize);
            player.mouse_x = Math.Min(Math.Max(player.mouse_x, 0), Rules.MapSize);

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

            player.y = Math.Min(Math.Max(player.y, 0), Rules.MapSize);
            player.mouse_y = Math.Min(Math.Max(player.mouse_y, 0), Rules.MapSize);


            game_model.Players[pos_index] = player;
            var task_2 = game_model.UpdateAllPlayers();
            Task.WaitAll(task_2);
        }
    }
}
