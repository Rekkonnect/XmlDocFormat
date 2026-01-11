using Spectre.Console.Cli;
using System.ComponentModel;

namespace XmlDocFormat.Cli;

public abstract class BaseCliCommandSettings
    : CommandSettings
{
    [CommandOption("--plain")]
    [Description("Print everything plainly without styling such as colors, ASCII art, etc.")]
    public bool PrintPlain { get; init; }
}
