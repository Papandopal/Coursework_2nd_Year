using Agar.io_Alpfa.RulesNameSpace;
using Agar.io_Alpfa.Services;

namespace Agar.io_Alpfa.Entities
{
    public record Food
    {
        public Food() 
        {
            x = EntitiesService.GetRandCoord();
            y = EntitiesService.GetRandCoord();
            size = Rules.FoodSize;
            color = EntitiesService.GetRandColor();
            is_eated = false;
        }
        public double x { get; set; }
        public double y { get; set; }
        public double size { get; set; }
        public string color { get; set; }
        public bool is_eated { get; set; }
    }

}
