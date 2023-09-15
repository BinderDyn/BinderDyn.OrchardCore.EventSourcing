using System.Data.Common;
using BinderDyn.OrchardCore.EventSourcing.Abstractions.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using OrchardCore.Environment.Shell;

namespace BinderDyn.Test.OrchardCore.EventSourcing.Abstractions.Data;

public class TablePrefixInterceptorTests
{
    private TablePrefixInterceptor _sut;

    private readonly ShellSettings _shellSettings = new ShellSettings()
    {
        ["TablePrefix"] = "prefix"
    };

    public TablePrefixInterceptorTests()
    {
        _sut = new TablePrefixInterceptor(_shellSettings);
    }

    public class NonQueryExecuting : TablePrefixInterceptorTests
    {
        [Fact]
        public void AddsPrefixForMigrationHistoryTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("CREATE TABLE [__EFMigrationsHistory] AND SOMETHING ELSE");
            _sut.NonQueryExecuting(dbCommand, null, new InterceptionResult<int>());

            dbCommand.CommandText.Should().Be("CREATE TABLE [prefix__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public void AddsPrefixForEventsTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("CREATE TABLE [Events] AND SOMETHING ELSE");
            _sut.NonQueryExecuting(dbCommand, null, new InterceptionResult<int>());

            dbCommand.CommandText.Should().Be("CREATE TABLE [prefix_Events] AND SOMETHING ELSE");
        }
    }

    public class ReaderExecuting : TablePrefixInterceptorTests
    {
        [Fact]
        public void AddsPrefixForMigrationHistoryTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
            _sut.ReaderExecuting(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [prefix__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public void AddsPrefixForEventsTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("CREATE TABLE [Events] AND SOMETHING ELSE");
            _sut.ReaderExecuting(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("CREATE TABLE [prefix_Events] AND SOMETHING ELSE");
        }
    }

    public class ReaderExecutingAsync : TablePrefixInterceptorTests
    {
        [Fact]
        public async Task AddsPrefixForMigrationHistoryTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
            await _sut.ReaderExecutingAsync(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [prefix__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public async Task AddsPrefixForEventsTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("CREATE TABLE [Events] AND SOMETHING ELSE");
            await _sut.ReaderExecutingAsync(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("CREATE TABLE [prefix_Events] AND SOMETHING ELSE");
        }
    }

    public class ScalarExecuting : TablePrefixInterceptorTests
    {
        [Fact]
        public void AddsPrefixForMigrationHistoryTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
            _sut.ScalarExecuting(dbCommand, null, new InterceptionResult<object>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [prefix__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public void AddsPrefixForEventsTable()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
            _sut.ScalarExecuting(dbCommand, null, new InterceptionResult<object>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [prefix_Events] AND SOMETHING ELSE");
        }
    }

    public class NoPrefix : TablePrefixInterceptorTests
    {
        private readonly ShellSettings _shellSettingsWithoutPrefix = new ShellSettings()
        {
            ["TablePrefix"] = ""
        };

        public NoPrefix()
        {
            _sut = new TablePrefixInterceptor(_shellSettingsWithoutPrefix);
        }

        [Fact]
        public async Task DoesntChangePrefixForReaderExecutingAsyncMigrations()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
            await _sut.ReaderExecutingAsync(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public async Task DoesntChangePrefixForReaderExecutingAsyncEvents()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
            await _sut.ReaderExecutingAsync(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
        }

        [Fact]
        public void DoesntChangePrefixForReaderExecutingMigrations()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
            _sut.ReaderExecuting(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public void DoesntChangePrefixForReaderExecutingEvents()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
            _sut.ReaderExecuting(dbCommand, null, new InterceptionResult<DbDataReader>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
        }

        [Fact]
        public void DoesntChangePrefixForScalarExecutingMigrations()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
            _sut.ScalarExecuting(dbCommand, null, new InterceptionResult<object>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public void DoesntChangePrefixForScalarExecutingEvents()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
            _sut.ScalarExecuting(dbCommand, null, new InterceptionResult<object>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
        }

        [Fact]
        public void DoesntChangePrefixForNonQueryMigrations()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
            _sut.NonQueryExecuting(dbCommand, null, new InterceptionResult<int>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [__EFMigrationsHistory] AND SOMETHING ELSE");
        }

        [Fact]
        public void DoesntChangePrefixForNonQueryEvents()
        {
            var dbCommand = Substitute.For<DbCommand>();
            dbCommand.CommandText.Returns("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
            _sut.NonQueryExecuting(dbCommand, null, new InterceptionResult<int>());

            dbCommand.CommandText.Should().Be("SELECT COUNT(*) FROM [Events] AND SOMETHING ELSE");
        }
    }
}