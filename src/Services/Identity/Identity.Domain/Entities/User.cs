namespace MicroShop.Services.Identity.Domain.Entities;

public sealed class User
{
    private readonly List<UserRole> _roles = new();

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Username { get; private set; } = default!;

    public string Email { get; private set; } = default!;

    public string PasswordHash { get; private set; } = default!;

    public string SecurityStamp { get; private set; } = Guid.NewGuid().ToString("N");

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public bool Active { get; private set; } = true;

    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    private User()
    {
    }

    public User(string username, string email)
    {
        Username = username;
        Email = email;
    }

    public void SetPasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        SecurityStamp = Guid.NewGuid().ToString("N");
    }

    public void Activate() => Active = true;

    public void Deactivate() => Active = false;

    public void AssignRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (_roles.Exists(r => r.RoleId == role.Id))
        {
            return;
        }

        _roles.Add(new UserRole(Id, role.Id));
    }

    public void RemoveRole(Guid roleId)
    {
        _roles.RemoveAll(r => r.RoleId == roleId);
    }
}
