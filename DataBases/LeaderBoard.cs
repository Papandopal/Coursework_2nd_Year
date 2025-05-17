using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Firebase.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Database.Query;

namespace Agar.io_Alpfa.DataBases
{
    public class LeaderBoard
    {
        private readonly FirebaseClient _client;
        public LeaderBoard()
        { 
            // Создание клиента Realtime Database
            _client = new FirebaseClient("https://agario-14b61-default-rtdb.europe-west1.firebasedatabase.app");
        }

        public void TrySavePlayerAsync(string playerId, double size)
        {
            if (GetPlayerAsync(playerId) == null)
            {
                var task = SavePlayerAsync(playerId, size);
                Task.WaitAll(task);
            }
            else
            {
                var task = UpdatePlayerAsync(playerId, new Dictionary<string, object> { { "Size", size } });
                Task.WaitAll(task);
            }

        }
        public async Task UpdatePlayerAsync(string playerId, Dictionary<string, object> updates)
        {
            await _client
            .Child("players")
            .Child(playerId)
            .PatchAsync(updates);
        }

        // Сохранение игровых данных
        public async Task SavePlayerAsync(string playerId, double size)
        {
            await _client
            .Child("players")
            .Child(playerId)
            .PutAsync(new { Size = size, LastUpdated = System.DateTime.UtcNow.ToString("o") });
        }

        // Получение игровых данных
        public async Task<Dictionary<string, object>> GetPlayerAsync(string playerId)
        {
            var response = await _client
            .Child("players")
            .Child(playerId)
            .OnceSingleAsync<Dictionary<string, object>>();
            return response;
        }

        // Получение лидерборда
        public async Task<List<Dictionary<string, object>>> GetLeaderboardAsync()
        {
            var response = await _client
            .Child("players")
            .OrderBy("Score")
            .LimitToLast(10)
            .OnceAsync<Dictionary<string, object>>();

            var leaderboard = new List<Dictionary<string, object>>();
            foreach (var item in response)
            {
                leaderboard.Add(item.Object);
            }
            return leaderboard;
        }
    }


}

