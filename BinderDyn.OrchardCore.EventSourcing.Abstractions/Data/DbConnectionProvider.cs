using OrchardCore.Environment.Shell;

namespace BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;

public interface IDbConnectionProvider
{
    string GetConnectionString();
    string GetProvider();
}

public class DbConnectionProvider : IDbConnectionProvider
{
    private readonly ShellSettings _shellSettings;

    public DbConnectionProvider(ShellSettings shellSettings)
    {
        _shellSettings = shellSettings;
    }

    public string GetConnectionString()
    {
        return _shellSettings["ConnectionString"] ?? string.Empty;
    }

    public string GetProvider()
    {
        return _shellSettings["DatabaseProvider"] ?? string.Empty;
    }
}