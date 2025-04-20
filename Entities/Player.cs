using Agar.io_Alpfa.Constants;

namespace Agar.io_Alpfa.Entities
{
    public record Player
    {
        public Player(int user_id) 
        {
            x = EntitiesService.GetRandCoord();
            y = EntitiesService.GetRandCoord();
            size = EntitiesService.constants.PlayerBasicSize;
            this.user_id = user_id;
            speed = EntitiesService.constants.PlayerBasicSpeed;
            mouse_x = 0;
            mouse_y = 0;
        }
        public double x { get; set; }
        public double y { get; set; }
        public int user_id { get; set; }
        public double size { get; set; }
        public double speed { get; set; }
        public string type { get; set; } = "pos";
        public double mouse_x { get; set; }
        public double mouse_y { get; set; }
    };
}
