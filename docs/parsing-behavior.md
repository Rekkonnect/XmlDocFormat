# Command Parsing Behavior

The tool uses `Spectre.Console.Cli` to build the CLI, and thus command parsing 
is based on that library's rules. Refer to the official documentation of the
library for further information besides this page.

Examples of behaviors and extra features:
- https://spectreconsole.net/cli/reference/built-in-command-behaviors

## Parsing Correct Commands

Correctly-typed commands will parse based on the following rules:
- The command name is present
- Required arguments are all present
- Required options are all present
- Values passed to the options are correctly formatted (e.g. integers, strings
  with whitespace and other characters, etc.)

### Short Options

The library allows merging multiple flag short options together into a single
short option token, as long as the options are only flags. For example, instead
of passing `format -d -r`, it's allowed to shorten into `format -dr`, which will
toggle both options `-d` and `-r` to true. Non-flag short options that require a
value are not supported in this feature. For example, `format -l 10 -p 10`
**cannot** be shortened to `format -lp 10` or any variations.

## Unknown Commands

Unknown command names will be gracefully handled and the tool will exit with -3.

## Invalid Arguments

Invalid arguments are not well-supported by the library yet, and as such more
unfriendly exceptions will be thrown and shown to the user. It may require some
effort to track down the violating argument or option.

## Unknown Options

Unknown option names behave differently. Long options (those starting with `--`)
will result in an "invalid long option name" error. Short options that are not
recognized, either in the merged string syntax or in general, will not cause any
errors and will be handled gracefully. Assume a command `cm` that does not have
a `-z` short option. It is possible to invoke it as `cm -z` and there will be no
errors. This option might be picked up unofficially due to the workarounds.

However, it is heavily discouraged to pass unsupported and undocumented options
to the commands, since these behaviors are not formally defined and future
versions of the library might introduce breaking changes affecting this area.
