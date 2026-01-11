using D20Tek.Spectre.Console.Extensions;
using XmlDocFormat.Cli;

var app = new CommandAppBuilder()
    .WithDIContainer()
    .WithStartup<Startup>()
    .Build();
return await app.RunAsync(args);
