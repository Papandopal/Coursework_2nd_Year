using System.Net.WebSockets;
using Agar.io_Alpfa.RulesNameSpace;
using Agar.io_Alpfa.Services;

namespace Agar.io_Alpfa.Entities
{
    public record Player
    {
        private PlayerDTO _dto = new PlayerDTO();
        public Player(int user_id, WebSocket connection)
        {
            x = EntitiesService.GetRandCoord();
            y = EntitiesService.GetRandCoord();
            size = Rules.PlayerBasicSize;
            this.user_id = user_id;
            speed = Rules.PlayerBasicSpeed;
            mouse_x = 0;
            mouse_y = 0;
            this.connection = connection;
        }
        public double x { get; set; }
        public double y { get; set; }
        public int user_id { get; set; }
        public double size { get; set; }
        public double speed { get; set; }
        public string name { get; set; } = "name";
        public string type { get; set; } = "pos";
        public double mouse_x { get; set; }
        public double mouse_y { get; set; }
        public WebSocket connection { get; set; }

        public PlayerDTO GetDTO()
        {
            _dto.x = x;
            _dto.y = y;
            _dto.size = size;
            _dto.speed = speed;
            _dto.user_id = user_id;
            _dto.name = name;
            return _dto;
        }

    };

}
