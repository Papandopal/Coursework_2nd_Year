namespace Agar.io_Alpfa.Interfaces
{
    public interface IDeserializator
    {
        IModel? model { get; set; }
        void SetModel(IModel model);
        Status Deserialize(string data);
    }

    public enum Status
    {
        Ok,
        Disconnect
    }

}
