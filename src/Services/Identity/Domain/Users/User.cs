namespace MicroShop.Services.Identity.Domain.Users;

/// <summary>
/// Represents an identity user within the MicroShop platform.
/// </summary>
public sealed class User
{
  private readonly List<Role> roles = new();
  private readonly List<Events.IDomainEvent> domainEvents = new();

  private User()
  {
  }

  private User(Guid id, string username, string email, string passwordHash, DateTimeOffset createdAt)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("User identifier cannot be empty.", nameof(id));
    }

    this.SetUsername(username);
    this.SetEmail(email);
    this.SetPasswordHash(passwordHash);

    this.Id = id;
    this.CreatedAt = createdAt == default ? DateTimeOffset.UtcNow : createdAt;
  }

  /// <summary>
  /// Gets the unique identifier for the user.
  /// </summary>
  public Guid Id { get; private set; }

  /// <summary>
  /// Gets the username for the user.
  /// </summary>
  public string Username { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the email for the user.
  /// </summary>
  public string Email { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the hashed password for the user.
  /// </summary>
  public string PasswordHash { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the moment when the user was created.
  /// </summary>
  public DateTimeOffset CreatedAt { get; private set; }

  /// <summary>
  /// Gets the roles assigned to the user.
  /// </summary>
  public IReadOnlyCollection<Role> Roles => this.roles.AsReadOnly();

  /// <summary>
  /// Gets the domain events raised by this user.
  /// </summary>
  public IReadOnlyCollection<Events.IDomainEvent> DomainEvents => this.domainEvents.AsReadOnly();

  /// <summary>
  /// Clears the currently tracked domain events.
  /// </summary>
  public void ClearDomainEvents() => this.domainEvents.Clear();

  /// <summary>
  /// Creates a new user instance.
  /// </summary>
  public static User Create(Guid id, string username, string email, string passwordHash, DateTimeOffset createdAt)
  {
    var user = new User(id, username, email, passwordHash, createdAt);
    user.domainEvents.Add(new Events.UserRegisteredDomainEvent(user.Id, user.Username, user.Email, user.CreatedAt));
    return user;
  }

  /// <summary>
  /// Assigns the provided role to the user.
  /// </summary>
  public void AssignRole(Role role)
  {
    if (role is null)
    {
      throw new ArgumentNullException(nameof(role));
    }

    if (this.roles.Any(existing => existing.Id == role.Id || existing.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase)))
    {
      return;
    }

    this.roles.Add(role);
  }

  /// <summary>
  /// Removes the provided role from the user.
  /// </summary>
  public void RemoveRole(Role role)
  {
    if (role is null)
    {
      throw new ArgumentNullException(nameof(role));
    }

    this.roles.RemoveAll(existing => existing.Id == role.Id || existing.Name.Equals(role.Name, StringComparison.OrdinalIgnoreCase));
  }

  /// <summary>
  /// Updates the password hash for the user.
  /// </summary>
  public void SetPasswordHash(string passwordHash)
  {
    if (string.IsNullOrWhiteSpace(passwordHash))
    {
      throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
    }

    this.PasswordHash = passwordHash;
  }

  /// <summary>
  /// Updates the username for the user.
  /// </summary>
  public void SetUsername(string username)
  {
    if (string.IsNullOrWhiteSpace(username))
    {
      throw new ArgumentException("Username cannot be empty.", nameof(username));
    }

    if (username.Length is < 3 or > 32)
    {
      throw new ArgumentException("Username length must be between 3 and 32 characters.", nameof(username));
    }

    this.Username = username.Trim();
  }

  /// <summary>
  /// Updates the email for the user.
  /// </summary>
  public void SetEmail(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      throw new ArgumentException("Email cannot be empty.", nameof(email));
    }

    this.Email = email.Trim();
  }
}
