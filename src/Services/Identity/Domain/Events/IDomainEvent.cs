namespace MicroShop.Services.Identity.Domain.Events;

/// <summary>
/// Represents a domain event that occured within the identity domain.
/// </summary>
public interface IDomainEvent
{
  /// <summary>
  /// Gets the timestamp for when the event occurred.
  /// </summary>
  DateTimeOffset OccurredOnUtc { get; }
}
