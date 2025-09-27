namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  public interface IMessageSerializer
  {
    string ContentType { get; }
    string Serialize<T>(T value);
    T Deserialize<T>(string payload);
  }
}
