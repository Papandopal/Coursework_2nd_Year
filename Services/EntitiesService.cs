using System.Collections.Concurrent;
using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.RulesNameSpace;

namespace Agar.io_Alpfa.Services
{
    public static class EntitiesService
    {
        public static int GetRandCoord()
        {
            var rand = new Random();
            return rand.Next(10, Rules.MapSize-10);
        }
        public static string GetRandColor()
        {
            var rand = new Random();
            return Rules.Colors[rand.Next(0, Rules.Colors.Count)];
        }
        public static ConcurrentList<Food> GetRandFoods(int count)
        {
            var foods = new ConcurrentList<Food>();
            for (int i = 0; i < count; i++)
            {
                Food c = new Food();
                foods.Add(c);
            }
            return foods;
        }
    }
}
