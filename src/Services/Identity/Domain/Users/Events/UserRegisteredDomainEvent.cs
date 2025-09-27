using MicroShop.Services.Identity.Domain.Events;

namespace MicroShop.Services.Identity.Domain.Users.Events;

/// <summary>
/// Raised when a new user is registered in the system.
/// </summary>
public sealed class UserRegisteredDomainEvent : IDomainEvent
{
  public UserRegisteredDomainEvent(Guid userId, string username, string email, DateTimeOffset occurredOnUtc)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("User identifier cannot be empty.", nameof(userId));
    }

    this.UserId = userId;
    this.Username = !string.IsNullOrWhiteSpace(username) ? username : throw new ArgumentException("Username cannot be empty.", nameof(username));
    this.Email = !string.IsNullOrWhiteSpace(email) ? email : throw new ArgumentException("Email cannot be empty.", nameof(email));
    this.OccurredOnUtc = occurredOnUtc;
  }

  public Guid UserId { get; }

  public string Username { get; }

  public string Email { get; }

  public DateTimeOffset OccurredOnUtc { get; }
}
