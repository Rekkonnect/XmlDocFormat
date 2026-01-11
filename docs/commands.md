# Commands Reference

Below are listed all the commands supported by this CLI tool.

For every command, the options can be provided in any order. Only the arguments
must be in the order they are described.

All commands have the following options available, besides the ones specific to
them, which are the below:
- `--plain`:
    Print everything plainly without styling such as colors, ASCII art, etc.

Exit codes have the following project-wide convention that all commands adhere
to:
- Negative exit codes represent a failure
  - -1 reflects a generic failure
  - -2 reflects operation cancellation
  - -3 reflects command parsing failed
  - All other negative values may further specify the type of issue
- 0 represents successful execution as intended
- Positive exit codes represent a non-failure result with some possible
  warning or extra information about the result

During any command's execution, `Ctrl+C` or `Ctrl+Break` will trigger a
cancellation event and the command will gracefully terminate with the -2 exit
code.

## Format

Command name: `format`

Aliases: `fmt`, `f`

Formats XML documentation comments in files.

### Exit Codes

| Code | Result |
|------|--------|
| 0 | Success |
| 1 | No files affected |
| -1 | Generic failure |
| -2 | Operation cancelled |
| -10 | Source path does not exist |
| -11 | Output path not writable |

### Arguments

- `<file-path>`:
    The path of the file to format XML documentation comments in.
    When the -d or -r flag is specified, this path is treated as a
    directory path, which formats XML documentation comments in
    all files in the directory, and optionally its subdirectories.

### Options

- `-o` or `--output`:
    The path of the output. When not specified, the tool overwrites the files.
- `-l` or `--line-length`:
    The max length of every line, which determines at which point to break
    XML documentation comment lines. This ONLY affects XML documentation;
    regular code will not be affected, regardless of its line length.
- `-d` or `--directory`:
    When specified, treats the file path as a directory, which formats all
    files contained in the specified directory, but not recursively.
- `-r` or `--directory-recursive`:
    When specified, treats the file path as a directory, which formats all
    files contained in the specified directory and all its subdirectories.
- `-p` or `--parallelism`:
    The maximum parallelism to use across formatting multiple files.
    Defaults to 0, which means the tool will automatically determine the
    optimal degree of parallelism based on processor count.

### Examples

```ps
# Formats XML docs in a specific file with a maximum line length of 80
xmldocformat format "E:/repos/my-project/Utilities.cs"

# Formats XML docs in a specific file with a maximum line length of 60
xmldocformat format "E:/repos/my-project/Utilities.cs" -l 60

# Formats XML docs in a directory recursively with a maximum line length of 60
xmldocformat format "E:/repos/my-project/" -r -l 60

# Formats XML docs in a directory recursively with a maximum line length of 60
# with parallelism of 2, meaning no more than 2 files will be processed at the
# same time
xmldocformat format "E:/repos/my-project/" -r -l 60 -p 2

# Using the aliases
xmldocformat fmt "E:/repos/my-project/" -r
xmldocformat f "E:/repos/my-project/" -r
```

### Remarks

Only files with the .cs extension will be formatted, and all others will be
ignored. VB support is coming soon.

Only `///` XML doc comments has been tested thoroughly so far. All XML doc
comments are always normalized to `///`, without the ability to customize the
comment style. It is the recommended style, and the formatter does not guarantee
support for `/**` beyond parsing and re-formatting it into `///`.

Files with syntax errors will be formatted as long as the Roslyn parser parses
the XML documentation comments, despite the syntax errors. Avoid invoking the
formatter when syntax errors are present to guarantee correctness.

The command attempts to respect the original encoding, detecting it via the
[Utf.Unknown library](https://github.com/CharsetDetector/UTF-unknown) (with
some workarounds for common issues with ASCII, UTF-8/16/32) and stores the
formatted code back in the detected encoding. For wrong encoding detections,
please feel free to open issues. Keep in mind however, that UTF-8 is the
de-facto and most preferred encoding to use.
