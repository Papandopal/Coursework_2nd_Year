
using Agar.io_Alpfa.Entities;

namespace Agar.io_Alpfa.Interfaces
{
    public interface IModel
    {
        public ConcurrentList<Player> Players {get;set;}
        public ConcurrentList<Food> Foods {get;set;}

        public Task UpdateAllPlayers();
        public Task UpdateCurrentPlayer(int id);
        public Task UpdateMap(Food c);
        public Task DeleteFood(int index);
        public Task LoadMap();
        public void EatFood(int index);
        public void Move(int index, double new_x, double new_y);
        public void NewSize(int id);
        public void NewSize(int victim_id, int killer_id);
        public void ResetPlayer(int id);
    }
}