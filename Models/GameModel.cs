using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.RulesNameSpace;
using System.Net.WebSockets;
using System.Text;
using Agar.io_Alpfa.Controllers;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Services;

namespace Agar.io_Alpfa.Models
{
    public class GameModel
    {
        public ConcurrentList<Player> Players = new ConcurrentList<Player>();
        public ConcurrentList<Food> Foods = EntitiesService.GetRandFoods(Rules.CirclesCount);

        public async Task UpdateAll()
        {
            if (Players.Count == 0) return;

            IEnumerable<PlayerDTO> players_dto = Players.Select(x => x.GetDTO());

            foreach (var player in Players)
            {
                string s = System.Text.Json.JsonSerializer.Serialize(players_dto);

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

            if (player == null) return;

            string s = "UpdateCurrentPlayer " + System.Text.Json.JsonSerializer.Serialize(player.GetDTO());

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
                string s = "UpdateMap " + System.Text.Json.JsonSerializer.Serialize(c);
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

                string s = "DeleteFood " + System.Text.Json.JsonSerializer.Serialize(index);
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
                string s = "LoadMap " + System.Text.Json.JsonSerializer.Serialize(Foods);
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

            Foods.Add(new_food);
            var task_2 = UpdateMap(new_food);

            Task.WaitAll(task_1, task_2);
            //await task_1;
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

        }
        public async void NewSize(int id)
        {
            if (Players.Count == 0) return;

            var player = Players.Where(item => item.user_id == id).FirstOrDefault();

            if (player == null) return;

            var index = Players.IndexOf(player);

            Players.ElementAt(index).size += Rules.FoodIncrease;

            var task_1 = UpdateCurrentPlayer(id);
            Task.WaitAll(task_1);

        }
        public async Task NewSize(int victim_id, int killer_id)
        {
            if (Players.Count == 0) return;

            var victim_player = Players.Where(item => item.user_id == victim_id).FirstOrDefault();
            var killer_player = Players.Where(item => item.user_id == killer_id).FirstOrDefault();

            if (victim_player == null || killer_player == null)
            {
                throw new Exception("Че блять, как нахуй при съедении кто-то удалился");
            }

            var killer_index = Players.IndexOf(killer_player);

            Players.ElementAt(killer_index).size += victim_player.size;

            var task_1 = UpdateCurrentPlayer(killer_index);
            Task.WaitAll(task_1);
        }
        public async void ResetPlayer(int id)
        {
            var player = Players.Where(item=>item.user_id == id).FirstOrDefault();
            
            if(player == null)
            {
                throw new Exception("ResetPlayer не нашел человека");
            }

            Players.Remove(player);
            
            string s = "Reset";

            await player.connection.SendAsync(
                Encoding.ASCII.GetBytes(s).ToArray(),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
