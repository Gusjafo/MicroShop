namespace MicroShop.Services.Identity.Domain.Entities;

public sealed class Role
{
    private Role()
    {
    }

    public Role(string name)
    {
        Name = name;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public bool IsDefault { get; private set; }

    public static Role Create(string name, string? description = null, bool isDefault = false)
    {
        return new Role(name)
        {
            Description = description,
            IsDefault = isDefault
        };
    }
}
