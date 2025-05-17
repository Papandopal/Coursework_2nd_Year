using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.Interfaces;
using Agar.io_Alpfa.Models;

namespace Agar.io_Alpfa.Services
{
    class MyDeserializator : IDeserializator
    {
        public IModel? model { get; set; }

        public Status Deserialize(string info)
        {
            var data = info.Split();
            if (data.First() == "move")
            {
                string data_index = data.ElementAt(2);
                string data_x = data.ElementAt(4);
                string data_y = data.ElementAt(6);

                var index = int.Parse(data_index);
                double new_x, new_y;

                new_x = double.Parse(data_x);
                new_y = double.Parse(data_y);

                model.Move(index, new_x, new_y);

            }
            else if (data.First() == "new_size")
            {
                int id = int.Parse(data.ElementAt(1));

                model.NewSize(id);

            }
            else if (data.First() == "eat_food")
            {
                int index = int.Parse(data.ElementAt(2));

                model.EatFood(index);
            }
            else if (data.First() == "disconnect")
            {
                int id = int.Parse(data.ElementAt(2));
                var player = model.Players.Where(player=>player.user_id == id).FirstOrDefault();
                model.Players.Remove(player);
                return Status.Disconnect;
            }
            else if (data.First() == "kill")
            {
                int victim_id = int.Parse(data.ElementAt(2));
                int killer_id = int.Parse(data.ElementAt(4));
                model.NewSize(victim_id, killer_id);
                model.ResetPlayer(victim_id);
            }
            return Status.Ok;
        }

        public void SetModel(IModel model)
        {
            this.model = model;
        }
    }
}