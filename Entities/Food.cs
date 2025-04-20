namespace Agar.io_Alpfa.Entities
{
    public record Food
    {
        public Food() 
        {
            x = EntitiesService.GetRandCoord();
            y = EntitiesService.GetRandCoord();
            size = EntitiesService.constants.FoodSize;
            color = EntitiesService.GetRandColor();
        }
        public double x { get; set; }
        public double y { get; set; }
        public double size { get; set; }
        public string color { get; set; }
    }

}
