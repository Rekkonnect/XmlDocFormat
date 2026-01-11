using D20Tek.Spectre.Console.Extensions;
using D20Tek.Spectre.Console.Extensions.Injection;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace XmlDocFormat.Cli;

internal sealed class Startup : StartupBase
{
    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar
            .WithLifetimes()
            .RegisterSingleton<IFileSystem, FileSystem>()
            ;
    }

    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        XmlDocCliConfiguration.Configure(config);
        return config;
    }
}
