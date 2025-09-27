using System;

namespace MicroShop.Services.Identity.Domain.Entities;

public sealed class Role : IEquatable<Role>
{
    public Role(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; }

    public string Name { get; }

    public static Role Create(string id, string name) => new(id, name);

    public bool Equals(Role? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is Role role && Equals(role);

    public override int GetHashCode() => Id.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Name;
}
