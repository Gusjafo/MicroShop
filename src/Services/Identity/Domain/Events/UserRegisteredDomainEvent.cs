using System;

namespace MicroShop.Services.Identity.Domain.Events;

public sealed record UserRegisteredDomainEvent(
    string UserId,
    string Username,
    string Email,
    DateTimeOffset RegisteredAt) : UserDomainEvent(RegisteredAt);
