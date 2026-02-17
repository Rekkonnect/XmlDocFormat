# XmlDocFormat

This is a utility tool to format C# XML documentation comments by enforcing a
max line length limit, re-wrapping text and consistent tag formatting.

<div style="text-align: center">

![Line Coverage](.github/images/badge_linecoverage.svg)&nbsp;&nbsp;
![Method Coverage](.github/images/badge_methodcoverage.svg)

</div>

## Dependencies

- [`Spectre.Console.Cli`](https://www.nuget.org/packages/Spectre.Console.Cli)
  to build the CLI
  - [`D20Tek.Spectre.Console.Extensions`](https://www.nuget.org/packages/D20Tek.Spectre.Console.Extensions)
  for extra features and helpers
- [`TUnit`](https://www.nuget.org/packages/TUnit) for testing
- [`Meziantou.Analyzer`](https://www.nuget.org/packages/Meziantou.Analyzer)
  for code analysis involving good practices
- [`UTF.Unknown`](https://www.nuget.org/packages/UTF.Unknown)
  for encoding detection (augmented with custom detection logic)
- [`System.IO.Abstractions`](https://www.nuget.org/packages/System.IO.Abstractions)
  for IO mocking capabilities during testing
  - [`System.IO.Abstractions.TestingHelpers`](https://www.nuget.org/packages/System.IO.Abstractions.TestingHelpers)
- [`Microsoft.CodeAnalysis`](https://www.nuget.org/packages/Microsoft.CodeAnalysis)
  and relevant packages:
  - [`Microsoft.CodeAnalysis.CSharp`](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp)
    for parsing C#
  - [`Microsoft.CodeAnalysis.VisualBasic`](https://www.nuget.org/packages/Microsoft.CodeAnalysis.VisualBasic)
    for parsing Visual Basic
- [`Vogen`](https://www.nuget.org/packages/Vogen) for domain struct generation
- [`RoseLynn`](https://www.nuget.org/packages/RoseLynn) and derivative packages
  for the development of internal source generators
- [`Dentextist`](https://www.nuget.org/packages/Dentextist) for building the
  generated sources with friendlier APIs
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

For the main formatter's logic in `XmlDocFormatter`, development was somewhat
AI-driven but well-human-reviewed, aiming to retain code quality levels.

All other parts of the codebase were entirely human-written. Contributions in
these areas must adhere to the existing code style and quality, without
deviating from the main goal of the architecture; a CLI tool that is tested via
mocking to reduce side-effects and risk during testing, that's also split into
a core logic library for the event that it gets released as a separate package.

It's also important to sustain a high code coverage, as it's visible on the
README's header. This process has been semi-automated by updating the coverage
badges locally via a C# script. The script allows the user to also navigate to
a local HTML page for more detailed information about the coverage.

Run the following command on the repository root to generate the coverage
reports and badges:
```sh
dotnet run ./tools/scripts/run-coverage.cs
```

To also automatically open the HTML index page for analysis, also add the `-i`
flag in the command:
```sh
dotnet run ./tools/scripts/run-coverage.cs -i
```

To run the above command, the following tools must be installed on the system:
- `dotnet-coverage`
- `reportgenerator`

Whenever code is being pushed, ensure that the coverage reports have been
re-generated appropriately.
