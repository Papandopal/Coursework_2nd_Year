using System.Collections.Concurrent;
using Agar.io_Alpfa.Constants;

namespace Agar.io_Alpfa.Entities
{
    public static class EntitiesService
    {
        public static int GetRandCoord()
        {
            var rand = new Random();
            return rand.Next(10, 4980);
        }
        public static string GetRandColor()
        {
            var rand = new Random();
            return Const.colors[rand.Next(0, Const.colors.Count)];
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
