using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.Constants;
using System.Net.WebSockets;
using System.Text;

namespace Agar.io_Alpfa.Models
{
    public class ServerModel
    {
        public List<Player> Players = new List<Player>();
        public List<WebSocket> WebSockets = new List<WebSocket>();
        public List<Food> Foods = EntitiesService.GetRandFoods(Const.CirclesCount);

        public async Task UpdateAll()
        {
            foreach (var webSocket in WebSockets)
            {
                String s = System.Text.Json.JsonSerializer.Serialize(Players);

                await webSocket.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }

        }
        public async Task UpdateCurrentPlayer(int index)
        {


            Player player = Players[index];
            WebSocket ws = WebSockets[index];
            String s = "UpdateCurrentPlayer " + System.Text.Json.JsonSerializer.Serialize(player);

            await ws.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);


        }
        public async Task UpdateMap(Food c)
        {
            foreach (var webSocket in WebSockets)
            {
                String s = "UpdateMap " + System.Text.Json.JsonSerializer.Serialize(c);
                await webSocket.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
        public async Task DeleteFood(int index)
        {
            foreach (var webSocket in WebSockets)
            {
                String s = "DeleteFood " + System.Text.Json.JsonSerializer.Serialize(index);
                await webSocket.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
        public async Task LoadMap()
        {
            foreach (var webSocket in WebSockets)
            {
                String s = "LoadMap " + System.Text.Json.JsonSerializer.Serialize(Foods);
                await webSocket.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
        public async void EatFood(int index)
        {
            Foods.RemoveAt(index);
            var task_1 = DeleteFood(index);

            var new_food = new Food();

            Foods.Add(new_food);
            var task_2 = UpdateMap(new_food);

            Task.WaitAll(task_1, task_2);
        }
        public async void Move(int index, double new_x, double new_y)
        {


            Players[index].mouse_x = new_x;
            Players[index].mouse_y = new_y;



        }
        public async void NewSize(int index)
        {


            Players.ElementAt(index).size += Const.FoodIncrease;
            Console.WriteLine("NewSize");
            var task_1 = UpdateCurrentPlayer(index);
            Task.WaitAll(task_1);
            Console.WriteLine("NewSize-end");


        }
    }
}
