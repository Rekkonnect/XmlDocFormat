using D20Tek.Spectre.Console.Extensions.Testing;
using Garyon.Extensions;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace XmlDocFormat.Cli.Tests;

public abstract class BaseCliTests
{
    protected static CommandAppTestContext CreateTestContext()
    {
        var app = new CommandAppTestContext();
        app.Configure(XmlDocCliConfiguration.Configure);
        return app;
    }

    protected static CommandAppTestContext CreateTestContext(
        MockFileSystem fileSystem)
    {
        var context = CreateTestContext();
        context.Registrar.RegisterInstance(typeof(IFileSystem), fileSystem);
        return context;
    }

    protected static CommandAppTestContext CreateTestContext(
        Dictionary<string, string> files)
    {
        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>(
                files.SelectValues(static content => new MockFileData(content))));
        return CreateTestContext(fileSystem);
    }
}
