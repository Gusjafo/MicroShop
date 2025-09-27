using System;

namespace MicroShop.Services.Identity.Domain.Events;

public sealed record UserRoleAssignedDomainEvent(string UserId, string RoleName)
    : UserDomainEvent(DateTimeOffset.UtcNow);
