namespace Agar.io_Alpfa.Entities
{
    public static class EntitiesService
    {
        public static Constants.Constants constants = new();

        public static int GetRandCoord()
        {
            var rand = new Random();
            return rand.Next(1000, 4000);
        }
        public static string GetRandColor()
        {
            var rand = new Random();
            return constants.colors[rand.Next(0, constants.colors.Count)];
        }
        public static List<Food> GetRandFoods(int count)
        {
            var foods = new List<Food>();
            for (int i = 0; i < count; i++)
            {
                Food c = new Food();
                foods.Add(c);
            }
            return foods;
        }
    }
}
