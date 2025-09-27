namespace MicroShop.Services.Identity.Domain.Users;

/// <summary>
/// Represents an application role that can be assigned to a user.
/// </summary>
public sealed class Role
{
  private Role()
  {
  }

  private Role(Guid id, string name)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("Role identifier cannot be empty.", nameof(id));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Role name cannot be empty.", nameof(name));
    }

    this.Id = id;
    this.Name = name.Trim();
  }

  /// <summary>
  /// Gets the unique identifier for the role.
  /// </summary>
  public Guid Id { get; private set; }

  /// <summary>
  /// Gets the human friendly name for the role.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Creates a new role instance.
  /// </summary>
  public static Role Create(string name)
  {
    return new Role(Guid.NewGuid(), name);
  }

  /// <summary>
  /// Creates a role instance with the provided identifier.
  /// </summary>
  public static Role Create(Guid id, string name)
  {
    return new Role(id, name);
  }
}
