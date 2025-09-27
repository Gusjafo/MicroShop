using System.Collections.Generic;
using MicroShop.BuildingBlocks.Abstractions.IdGeneration;
using MicroShop.BuildingBlocks.Abstractions.Time;
using MicroShop.Services.Identity.Domain.Events;

namespace MicroShop.Services.Identity.Domain.Entities;

public sealed class User
{
    private readonly HashSet<Role> _roles = [];
    private readonly List<UserDomainEvent> _domainEvents = [];

    public User(string id, string username, string email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public string Id { get; }

    public string Username { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public IReadOnlyCollection<Role> Roles => _roles;

    public IReadOnlyCollection<UserDomainEvent> DomainEvents => _domainEvents;

    public static User Create(
        IIdGenerator idGenerator,
        IClock clock,
        string username,
        string email,
        string passwordHash,
        IEnumerable<Role> roles)
    {
        ArgumentNullException.ThrowIfNull(idGenerator);
        ArgumentNullException.ThrowIfNull(clock);

        var id = idGenerator.NewId();
        var createdAt = clock.UtcNow;
        var user = new User(id, username, email, passwordHash, createdAt);

        foreach (var role in roles)
        {
            user.AssignRole(role);
        }

        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Username, user.Email, createdAt));

        return user;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void AssignRole(Role role)
    {
        if (_roles.Add(role))
        {
            _domainEvents.Add(new UserRoleAssignedDomainEvent(Id, role.Name));
        }
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void AddDomainEvent(UserDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }
}
