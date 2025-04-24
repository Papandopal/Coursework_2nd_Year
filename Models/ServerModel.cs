using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.Constants;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;

namespace Agar.io_Alpfa.Models
{
    public class ServerModel
    {
        public ConcurrentList<Player> Players = new ConcurrentList<Player>();
        //public List<WebSocket> WebSockets = new List<WebSocket>();
        public ConcurrentList<Food> Foods = EntitiesService.GetRandFoods(Const.CirclesCount);

        public async Task UpdateAll()
        {
            if(Players.Count == 0) return;

            IEnumerable<PlayerDTO> players_dto = Players.Select(x => x.GetDTO());

            foreach (var player in Players)
            {
                String s = System.Text.Json.JsonSerializer.Serialize(players_dto);
                
                await player.connection.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }

        }
        public async Task UpdateCurrentPlayer(int id)
        {
            if (Players.Count == 0) return;

            Player player = Players.Where((item) => item.user_id == id).FirstOrDefault();

            if(player==null) return;

            String s = "UpdateCurrentPlayer " + System.Text.Json.JsonSerializer.Serialize(player.GetDTO());

            await player.connection.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);


        }
        public async Task UpdateMap(Food c)
        {
            if (Players.Count == 0) return;

            foreach (var player in Players)
            {
                String s = "UpdateMap " + System.Text.Json.JsonSerializer.Serialize(c);
                await player.connection.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
        public async Task DeleteFood(int index)
        {
            if (Players.Count == 0) return;

            foreach (var player in Players)
            {
                String s = "DeleteFood " + System.Text.Json.JsonSerializer.Serialize(index);
                await player.connection.SendAsync(
                    Encoding.ASCII.GetBytes(s).ToArray(),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
        public async Task LoadMap()
        {
            if (Players.Count == 0) return;

            foreach (var player in Players)
            {
                String s = "LoadMap " + System.Text.Json.JsonSerializer.Serialize(Foods);
                await player.connection.SendAsync(
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
            /*
            Foods.Add(new_food);
            var task_2 = UpdateMap(new_food);
            */
            //Task.WaitAll(task_1, task_2);
            await task_1;
        }
        public async void Move(int index, double new_x, double new_y)
        {
            if (Players.Count == 0) return;

            foreach (var player in Players)
            {
                if (player.user_id == index)
                {
                    player.mouse_x = new_x;
                    player.mouse_y = new_y;
                    break;
                }
            }
            //Players.ElementAt(index).mouse_x = new_x;
            //Players.ElementAt(index).mouse_y = new_y;


        }
        public async void NewSize(int id)
        {
            if (Players.Count == 0) return;

            var player = Players.Where(item=>item.user_id==id).FirstOrDefault();
            var index = Players.IndexOf(player);

            Players.ElementAt(index).size += Const.FoodIncrease;
            
            var task_1 = UpdateCurrentPlayer(id);
            Task.WaitAll(task_1);

        }
    }
}
