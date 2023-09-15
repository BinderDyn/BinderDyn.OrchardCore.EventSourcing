using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrchardCore.Environment.Shell;

namespace BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;

public class TablePrefixInterceptor : DbCommandInterceptor
{
    private readonly ShellSettings _shellSettings;

    public TablePrefixInterceptor(ShellSettings shellSettings)
    {
        _shellSettings = shellSettings;
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData,
        InterceptionResult<int> result)
    {
        var tablePrefix = _shellSettings["TablePrefix"];
        if (tablePrefix == string.Empty) return base.NonQueryExecuting(command, eventData, result);

        if (command.CommandText.Contains("[__EFMigrationsHistory]"))
            command.CommandText =
                command.CommandText.Replace("__EFMigrationsHistory", tablePrefix + "__EFMigrationsHistory");

        if (command.CommandText.Contains("Events"))
            command.CommandText = command.CommandText.Replace("Events", $"{tablePrefix}_Events");

        return base.NonQueryExecuting(command, eventData, result);
    }
}