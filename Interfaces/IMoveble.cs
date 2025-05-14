using Agar.io_Alpfa.Entities;

namespace Agar.io_Alpfa.Interfaces
{
    public interface IMoveble
    {
        Player player {get;set;}
        public void Move(double new_x, double new_y);

    }
}