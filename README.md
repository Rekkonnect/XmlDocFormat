# XmlDocFormat

This is a utility tool to format C# XML documentation comments by enforcing a
max line length limit, re-wrapping text and consistent tag formatting.

## Dependencies


- [`Spectre.Console.Cli`](https://www.nuget.org/packages/Spectre.Console.Cli)
  to build the CLI
  - [`D20Tek.Spectre.Console.Extensions`](https://www.nuget.org/packages/D20Tek.Spectre.Console.Extensions)
  for extra features and helpers
- [`TUnit`](https://www.nuget.org/packages/TUnit) for testing
- [`System.IO.Abstractions`](https://www.nuget.org/packages/System.IO.Abstractions)
  for IO mocking capabilities during testing
  - [`System.IO.Abstractions.TestingHelpers`](https://www.nuget.org/packages/System.IO.Abstractions.TestingHelpers)
- [`Microsoft.CodeAnalysis.CSharp`](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp)
  for parsing C#
- [`Garyon`](https://www.nuget.org/packages/Garyon) for common utilities
  - Also used as a [lab rat](https://github.com/Rekkonnect/Garyon/pull/15)
    during the tool's development

## Usage

The tool is offered as a CLI tool that can be installed via `dotnet tool`,
either locally or globally. With the `XmlDocFormat` command, you can quickly
format XML documentation comments in C# source files across an entire
directory.

Find the releases on [NuGet](https://www.nuget.org/packages/XmlDocFormat.Cli)

Install via the .NET CLI:
```ps
dotnet tool install -g XmlDocFormat.Cli
```

### Commands

The available commands are:

- `format <file-path> [-d] [-r] [-o <path>] [-p <number>] [-l <length>]`

For all commands, the order of the options is not strict.

See also:
- the [commands reference](./docs/commands.md) for detailed command usage
  instructions.
- the [parsing behavior reference](./docs/parsing-behavior.md) for tips on
  expected behaviors and command syntax.

## Testing

The project has been tested against example cases, in the `XmlDocFormat.Tests`
project. However, the test suite is not exhaustive. Feel free to report any
bugs in the repo's issues.

### Success Stories

- [Garyon](https://github.com/Rekkonnect/Garyon/pull/15) - reformatted the
  entire project's documentation

## Contributing

For the main formatter's logic in `XmlDocFormatter`, development was heavily
AI-driven, thus code quality might be subpar. Contributions to improve the
codebase in that area are always welcome, but will be susceptible to heavy
adjustments as more features are included. As a result, it's recommended to
focus more on delivering the functionality rather than perfect code style, at
least at the present.

All other parts of the codebase were entirely human-written. Contributions in
these areas must adhere to the existing code style and quality, without
deviating from the main goal of the architecture; a CLI tool that is tested via
mocking to reduce side-effects and risk during testing, that's also split into
a core logic library for the event that it gets released as a separate package.
