using OrchardCore.Environment.Shell;

namespace BinderDyn.OrchardCore.EventSourcing.Data;

public interface IDbConnectionProvider
{
    string GetConnectionString();
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
}