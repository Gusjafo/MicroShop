namespace MicroShop.BuildingBlocks.Abstractions.Messaging
{
  /// <summary>
  /// Provides serialization services for messaging payloads.
  /// </summary>
  public interface IMessageSerializer
  {
    /// <summary>
    /// Gets the content type produced by the serializer.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Serializes the provided value to a string representation.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The serialized payload.</returns>
    string Serialize<T>(T value);

    /// <summary>
    /// Deserializes the provided payload to the specified type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="payload">The serialized payload.</param>
    /// <returns>The deserialized value.</returns>
    T Deserialize<T>(string payload);
  }
}
