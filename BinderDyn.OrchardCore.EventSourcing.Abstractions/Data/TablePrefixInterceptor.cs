using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrchardCore.Environment.Shell;

namespace BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;

public class TablePrefixInterceptor : DbCommandInterceptor
{
    private readonly string _tablePrefix;

    public TablePrefixInterceptor(ShellSettings shellSettings)
    {
        _tablePrefix = shellSettings["TablePrefix"];
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        if (_tablePrefix == string.Empty)
            return base.NonQueryExecuting(command, eventData, result);

        command.CommandText = AddMigrationsHistoryPrefix(command);
        command.CommandText = AddEventTablePrefix(command);

        return result;
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        if (_tablePrefix == string.Empty)
            return base.ReaderExecuting(command, eventData, result);

        command.CommandText = AddMigrationsHistoryPrefix(command);
        command.CommandText = AddEventTablePrefix(command);

        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = new CancellationToken())
    {
        if (_tablePrefix == string.Empty)
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);

        command.CommandText = AddMigrationsHistoryPrefix(command);
        command.CommandText = AddEventTablePrefix(command);

        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    public override InterceptionResult<object> ScalarExecuting(DbCommand command,
        CommandEventData eventData, InterceptionResult<object> result)
    {
        if (_tablePrefix == string.Empty) return base.ScalarExecuting(command, eventData, result);

        command.CommandText = AddMigrationsHistoryPrefix(command);
        command.CommandText = AddEventTablePrefix(command);

        return result;
    }

    private string AddMigrationsHistoryPrefix(DbCommand command)
    {
        return command.CommandText.Contains("[__EFMigrationsHistory]")
            ? command.CommandText.Replace("__EFMigrationsHistory", _tablePrefix + "__EFMigrationsHistory")
            : command.CommandText;
    }

    private string AddEventTablePrefix(DbCommand command)
    {
        return command.CommandText.Contains("Events")
            ? command.CommandText.Replace("Events", $"{_tablePrefix}_Events")
            : command.CommandText;
    }
}