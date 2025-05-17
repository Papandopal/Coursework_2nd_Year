using Agar.io_Alpfa.Interfaces;
using Agar.io_Alpfa.Models;

namespace Agar.io_Alpfa.Entities
{
    public class MouseMoveble : IMoveble
    {
        public Player player { get; set; }
        TimerCallback time_call_back;
        Timer timer;

        public MouseMoveble()
        {
            time_call_back = new TimerCallback(GoToMouse);
            timer = new Timer(time_call_back, null, 0, Rules.TimerPeriod);
        }

        public void Move(double new_x, double new_y)
        {
            player.mouse_x = new_x;
            player.mouse_y = new_y;
        }

        void GoToMouse(object obj)
        {

            //var ws = data.connection;

            //if (ws.State != WebSocketState.Open) return;

            //var player = game_model.Players.Where(item => item.user_id == data.user_id).FirstOrDefault();
            if (player == null) return;
            //var pos_index = game_model.Players.IndexOf(player);

            var mouse_x = player.mouse_x;
            var mouse_y = player.mouse_y;


            if (mouse_x != player.x && mouse_y != player.y)
            {
                double len_vector = Math.Sqrt(Math.Pow(player.x - mouse_x, 2) + Math.Pow(player.y - mouse_y, 2));
                var x_norm = Math.Abs(player.x - mouse_x) / len_vector;
                var y_norm = Math.Abs(player.y - mouse_y) / len_vector;

                if (mouse_x > player.x)
                {
                    player.x += x_norm * player.speed;
                    player.mouse_x += x_norm * player.speed;
                }
                else
                {
                    player.x -= x_norm * player.speed;
                    player.mouse_x -= x_norm * player.speed;
                }

                player.x = Math.Min(Math.Max(player.x, 0), Rules.MapSize);
                player.mouse_x = Math.Min(Math.Max(player.mouse_x, 0), Rules.MapSize);

                if (mouse_y > player.y)
                {
                    player.y += y_norm * player.speed;
                    player.mouse_y += y_norm * player.speed;
                }
                else
                {
                    player.y -= y_norm * player.speed;
                    player.mouse_y -= y_norm * player.speed;
                }

                player.y = Math.Min(Math.Max(player.y, 0), Rules.MapSize);
                player.mouse_y = Math.Min(Math.Max(player.mouse_y, 0), Rules.MapSize);

                //game_model.Players[pos_index] = player;
                //var task_2 = game_model.UpdateAllPlayers();
                //Task.WaitAll(task_2);
            }
        }
    }
}