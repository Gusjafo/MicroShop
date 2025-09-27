using System.Collections.Generic;

namespace MicroShop.Services.Identity.Domain.Entities;

public static class DefaultRoles
{
    public const string Customer = "Customer";
    public const string Admin = "Admin";

    public static IReadOnlyCollection<string> All { get; } = new[] { Customer, Admin };
}
