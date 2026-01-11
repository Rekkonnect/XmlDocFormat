using Garyon.Extensions;
using System.IO.Abstractions;

namespace XmlDocFormat.Cli.Tests;

public sealed class FormatCommandDevTests : BaseCliTests
{
    [Test]
#if true
    [Skip("Dev test")]
#endif
    public async Task DevTest()
    {
        var context = CreateTestContext();
        context.Registrar.RegisterInstance(typeof(IFileSystem), new FileSystem());

        var result = await context.RunAsync(
        [
            "format", @"example-source.cs",
            "-l", "80",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);
    }
}
