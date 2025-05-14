using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Models;
using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.Services;
using Agar.io_Alpfa.Interfaces;

namespace Agar.io_Alpfa.Controllers;
public class GameController : Controller
{
    static IModel game_model = new GameModel();
    static int userGen = 0;
    static TimerCallback time_call_back;
    static Timer timer;
    private readonly ILogger<GameController> _logger;

    public GameController(ILogger<GameController> logger)
    {
        _logger = logger;
        //game_model = model;
        game_model.Foods = EntitiesService.GetRandFoods(Rules.CirclesCount);

        time_call_back = new TimerCallback(StartUpdateAllEveryTime);
        timer = new Timer(time_call_back, null, 0, Rules.TimerPeriod);
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

    public async Task<string> GetName(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            string[] data = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count).Split();
            if (data.Contains("MyNameIs:"))
            {
                return data.ElementAt(1);
            }
            throw new Exception($"Пришло не имя а{data}");
        }

    private async Task DialogWith(WebSocket webSocket)
    {
        Player player = new Player(userGen++, webSocket, new MouseMoveble());

        player.name = await GetName(webSocket);
        game_model.Players.Add(player);

        var task_1 = game_model.UpdateCurrentPlayer(player.user_id);
        var task_2 = game_model.LoadMap();
        var task_3 = game_model.UpdateAllPlayers();
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

                    new_x = double.Parse(data_x);
                    new_y = double.Parse(data_y);

                    game_model.Move(index, new_x, new_y);

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

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

            }
        }
        catch (Exception e)
        {
            game_model.Players.Remove(player);
            Console.WriteLine(e);
            return;
        }

    }

    async void StartUpdateAllEveryTime(object obj)
    {   
        await game_model.UpdateAllPlayers();
    }
    
}
