namespace MicroShop.Services.Identity.Infrastructure.Options;

public sealed class SqlServerSettings
{
    public const string SectionName = "SqlServer";

    public string ConnectionString { get; set; } = string.Empty;
}
