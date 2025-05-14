namespace Agar.io_Alpfa.Models
{
    public static class Rules
    {
        public static int PlayerBasicSize = 20;
        public static int PlayerBasicSpeed;
        public static Dictionary<string, int> PlayerSpeeds = new Dictionary<string, int>() { { "slow", 1 }, { "normal", 3 }, { "fast", 5 } };
        //public static int PlayerMinLenToMouseForMove = 20;

        public static int CirclesCount = 300;

        public static int FoodSize = 10;
        public static int FoodIncrease = 1;

        public static int TimerPeriod = 20;

        public static int MapSize;
        //public static int MapWidth = 5000;
        public static Dictionary<string, int> MapSizes = new Dictionary<string, int>() { { "small", 3000 }, { "medium", 5000 }, { "large", 10000 } };

        public static List<string> Colors = ["#ffff00", "#00ff00", "#00ffff"];

        public static List<string> NamesOfVoters = new ();
        public static int HowManyPeoplesVote = 0;
    }
}
