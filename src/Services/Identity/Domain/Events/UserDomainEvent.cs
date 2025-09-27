using System;

namespace MicroShop.Services.Identity.Domain.Events;

public abstract record UserDomainEvent(DateTimeOffset OccurredOn);
