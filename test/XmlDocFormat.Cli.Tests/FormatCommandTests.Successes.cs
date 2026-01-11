using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using XmlDocFormat.Core.Tests;

namespace XmlDocFormat.Cli.Tests;

public partial class FormatCommandTests : BaseCliTests
{
    [Test]
    public async Task SingleFileFormat()
    {
        const string filePath = @"Q:\cs\file1.cs";

        var asset = FormattingTestCases.BasicSummary;
        var source = asset.Source;
        var target = asset.Formatted;
        var options = asset.FormatOptions;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [filePath] = new(source),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", filePath,
            "-l", options.MaxLineLength.ToString(),
            "-p", "1"
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var newText = await fileSystem.File.ReadAllTextAsync(filePath);
        await Assert.That(newText).IsEqualTo(target);
    }

    [Test]
    public async Task MultipleFilesOuterFormat()
    {
        const string file1Path = @"Q:\cs\file1.cs";
        const string file2Path = @"Q:\cs\file2.cs";
        const string file3Path = @"Q:\cs\nested\file3.cs";

        var case1 = FormattingTestCases.BasicSummary;
        var case2 = FormattingTestCases.BreakElementPreservation;
        var case3 = FormattingTestCases.ParagraphElementPreservation;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [file1Path] = new(case1.Source),
                [file2Path] = new(case2.Source),
                [file3Path] = new(case3.Source),
            });
        var context = CreateTestContext(fileSystem);

        var targetDirectory = fileSystem.FileInfo.New(file1Path).Directory!;

        var result = await context.RunAsync(
        [
            "format", targetDirectory.FullName,
            "-d",
            "-l", case1.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var newText1 = await fileSystem.File.ReadAllTextAsync(file1Path);
        await Assert.That(newText1).IsEqualTo(case1.Formatted);

        var newText2 = await fileSystem.File.ReadAllTextAsync(file2Path);
        await Assert.That(newText2).IsEqualTo(case2.Formatted);

        var newText3 = await fileSystem.File.ReadAllTextAsync(file3Path);
        await Assert.That(newText3).IsEqualTo(case3.Source);
    }

    [Test]
    public async Task MultipleFilesInnerFormat()
    {
        const string file1Path = @"Q:\cs\file1.cs";
        const string file2Path = @"Q:\cs\file2.cs";
        const string file3Path = @"Q:\cs\nested\file3.cs";

        var case1 = FormattingTestCases.BasicSummary;
        var case2 = FormattingTestCases.BreakElementPreservation;
        var case3 = FormattingTestCases.ParagraphElementPreservation;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [file1Path] = new(case1.Source),
                [file2Path] = new(case2.Source),
                [file3Path] = new(case3.Source),
            });
        var context = CreateTestContext(fileSystem);

        var targetDirectory = fileSystem.FileInfo.New(file3Path).Directory!;

        var result = await context.RunAsync(
        [
            "format", targetDirectory.FullName,
            "-d",
            "-l", case1.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var newText1 = await fileSystem.File.ReadAllTextAsync(file1Path);
        await Assert.That(newText1).IsEqualTo(case1.Source);

        var newText2 = await fileSystem.File.ReadAllTextAsync(file2Path);
        await Assert.That(newText2).IsEqualTo(case2.Source);

        var newText3 = await fileSystem.File.ReadAllTextAsync(file3Path);
        await Assert.That(newText3).IsEqualTo(case3.Formatted);
    }

    [Test]
    public async Task MultipleFilesRecursiveOuterFormat()
    {
        const string file1Path = @"Q:\cs\file1.cs";
        const string file2Path = @"Q:\cs\file2.cs";
        const string file3Path = @"Q:\cs\nested\file3.cs";

        var case1 = FormattingTestCases.BasicSummary;
        var case2 = FormattingTestCases.BreakElementPreservation;
        var case3 = FormattingTestCases.ParagraphElementPreservation;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [file1Path] = new(case1.Source),
                [file2Path] = new(case2.Source),
                [file3Path] = new(case3.Source),
            });
        var context = CreateTestContext(fileSystem);

        var targetDirectory = fileSystem.FileInfo.New(file1Path).Directory!;

        var result = await context.RunAsync(
        [
            "format", targetDirectory.FullName,
            "-r",
            "-l", case1.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var newText1 = await fileSystem.File.ReadAllTextAsync(file1Path);
        await Assert.That(newText1).IsEqualTo(case1.Formatted);

        var newText2 = await fileSystem.File.ReadAllTextAsync(file2Path);
        await Assert.That(newText2).IsEqualTo(case2.Formatted);

        var newText3 = await fileSystem.File.ReadAllTextAsync(file3Path);
        await Assert.That(newText3).IsEqualTo(case3.Formatted);
    }

    [Test]
    public async Task MultipleFilesRecursiveInnerFormat()
    {
        const string file1Path = @"Q:\cs\file1.cs";
        const string file2Path = @"Q:\cs\file2.cs";
        const string file3Path = @"Q:\cs\nested\file3.cs";

        var case1 = FormattingTestCases.BasicSummary;
        var case2 = FormattingTestCases.BreakElementPreservation;
        var case3 = FormattingTestCases.ParagraphElementPreservation;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [file1Path] = new(case1.Source),
                [file2Path] = new(case2.Source),
                [file3Path] = new(case3.Source),
            });
        var context = CreateTestContext(fileSystem);

        var targetDirectory = fileSystem.FileInfo.New(file3Path).Directory!;

        var result = await context.RunAsync(
        [
            "format", targetDirectory.FullName,
            "-r",
            "-l", case1.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var newText1 = await fileSystem.File.ReadAllTextAsync(file1Path);
        await Assert.That(newText1).IsEqualTo(case1.Source);

        var newText2 = await fileSystem.File.ReadAllTextAsync(file2Path);
        await Assert.That(newText2).IsEqualTo(case2.Source);

        var newText3 = await fileSystem.File.ReadAllTextAsync(file3Path);
        await Assert.That(newText3).IsEqualTo(case3.Formatted);
    }

    [Test]
    public async Task SingleFileFormatToOutputInSameDirectory()
    {
        const string sourceFilePath = @"Q:\cs\file1.cs";
        const string targetFilePath = @"Q:\cs\file2.cs";

        var formattingCase = FormattingTestCases.BasicSummary;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [sourceFilePath] = new(formattingCase.Source),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", sourceFilePath,
            "-o", targetFilePath,
            "-l", formattingCase.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        await AssertSourceTargetRelationship(
            fileSystem, sourceFilePath, targetFilePath, formattingCase);
    }

    [Test]
    public async Task SingleFileFormatToOutputInDifferentDirectory()
    {
        const string sourceFilePath = @"Q:\cs\file1.cs";
        const string targetFilePath = @"Q:\cs\inner\file2.cs";
        const string otherExistingFilePath = @"Q:\cs\inner\existing.cs";

        var formattingCase = FormattingTestCases.BasicSummary;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [sourceFilePath] = new(formattingCase.Source),
                [otherExistingFilePath] = new(string.Empty),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", sourceFilePath,
            "-o", targetFilePath,
            "-l", formattingCase.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        await AssertSourceTargetRelationship(
            fileSystem, sourceFilePath, targetFilePath, formattingCase);
    }

    [Test]
    public async Task SingleFileFormatToOutputInNewNestedDirectory()
    {
        const string sourceFilePath = @"Q:\cs\file1.cs";
        const string targetFilePath = @"Q:\cs\inner\file2.cs";

        var formattingCase = FormattingTestCases.BasicSummary;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [sourceFilePath] = new(formattingCase.Source),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", sourceFilePath,
            "-o", targetFilePath,
            "-l", formattingCase.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        await AssertSourceTargetRelationship(
            fileSystem, sourceFilePath, targetFilePath, formattingCase);
    }

    [Test]
    public async Task SingleFileFormatToOutputInNewIrrelevantDirectory()
    {
        const string sourceFilePath = @"Q:\cs\file1.cs";
        const string targetFilePath = @"Q:\cs2\file2.cs";

        var formattingCase = FormattingTestCases.BasicSummary;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [sourceFilePath] = new(formattingCase.Source),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", sourceFilePath,
            "-o", targetFilePath,
            "-l", formattingCase.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        await AssertSourceTargetRelationship(
            fileSystem, sourceFilePath, targetFilePath, formattingCase);
    }

    [Test]
    public async Task MultipleFileFormatToOutputInNewIrrelevantDirectory()
    {
        const string sourceFile1Path = @"Q:\cs\file1.cs";
        const string sourceFile2Path = @"Q:\cs\file2.cs";
        const string sourceFile3Path = @"Q:\cs\nested\file3.cs";

        const string outputFile1Path = @"Q:\cs2\file1.cs";
        const string outputFile2Path = @"Q:\cs2\file2.cs";
        const string outputFile3Path = @"Q:\cs2\nested\file3.cs";

        var case1 = FormattingTestCases.BasicSummary;
        var case2 = FormattingTestCases.BreakElementPreservation;
        var case3 = FormattingTestCases.ParagraphElementPreservation;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [sourceFile1Path] = new(case1.Source),
                [sourceFile2Path] = new(case2.Source),
                [sourceFile3Path] = new(case3.Source),
            });
        var context = CreateTestContext(fileSystem);

        var targetDirectory = fileSystem.FileInfo.New(sourceFile1Path).Directory!;
        var outputTargetDirectory = fileSystem.FileInfo.New(outputFile1Path).Directory!;

        var result = await context.RunAsync(
        [
            "format", targetDirectory.FullName,
            "-o", outputTargetDirectory.FullName,
            "-r",
            "-l", case1.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        await AssertSourceTargetRelationship(
            fileSystem, sourceFile1Path, outputFile1Path, case1);

        await AssertSourceTargetRelationship(
            fileSystem, sourceFile2Path, outputFile2Path, case2);

        await AssertSourceTargetRelationship(
            fileSystem, sourceFile3Path, outputFile3Path, case3);
    }

    [Test]
    public async Task SingleNotCsFileFormat()
    {
        const string filePath = @"Q:\cs\file1.notcs";

        var asset = FormattingTestCases.BasicSummary;
        var source = asset.Source;
        var target = asset.Formatted;
        var options = asset.FormatOptions;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [filePath] = new(source),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", filePath,
            "-l", options.MaxLineLength.ToString(),
            "-p", "1"
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.NoFilesAffected);

        var newText = await fileSystem.File.ReadAllTextAsync(filePath);
        await Assert.That(newText).IsEqualTo(source);
    }

    [Test]
    public async Task MultipleFilesMixedRealAndNotCsFormat()
    {
        const string file1Path = @"Q:\cs\file1.cs";
        const string file2Path = @"Q:\cs\file2.notcs";
        const string file3Path = @"Q:\cs\nested\file3.cs";

        var case1 = FormattingTestCases.BasicSummary;
        var case2 = FormattingTestCases.BreakElementPreservation;
        var case3 = FormattingTestCases.ParagraphElementPreservation;

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [file1Path] = new(case1.Source),
                [file2Path] = new(case2.Source),
                [file3Path] = new(case3.Source),
            });
        var context = CreateTestContext(fileSystem);

        var targetDirectory = fileSystem.FileInfo.New(file1Path).Directory!;

        var result = await context.RunAsync(
        [
            "format", targetDirectory.FullName,
            "-r",
            "-l", case1.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var newText1 = await fileSystem.File.ReadAllTextAsync(file1Path);
        await Assert.That(newText1).IsEqualTo(case1.Formatted);

        var newText2 = await fileSystem.File.ReadAllTextAsync(file2Path);
        await Assert.That(newText2).IsEqualTo(case2.Source);

        var newText3 = await fileSystem.File.ReadAllTextAsync(file3Path);
        await Assert.That(newText3).IsEqualTo(case3.Formatted);
    }

    [Test]
    [MethodDataSource(nameof(GetTestJapaneseEncodings))]
    [ArgumentDisplayFormatter<EncodingArgumentDisplayFormatter>]
    [DisplayName("$encoding")]
    public async Task SingleFileJapaneseFormat(Encoding encoding)
    {
        await AssertSingleFileWithEncoding(encoding, FormattingTestCases.WithJapaneseText);
    }

    [Test]
    [MethodDataSource(nameof(GetTestEncodings))]
    [ArgumentDisplayFormatter<EncodingArgumentDisplayFormatter>]
    [DisplayName("$encoding")]
    public async Task SingleFileLatinFormat(Encoding encoding)
    {
        await AssertSingleFileWithEncoding(encoding, FormattingTestCases.BasicSummary);
    }

    [Test]
    public async Task MultipleFilesJapaneseFormat()
    {
        const string file1Path = @"Q:\cs\file1.cs";
        const string file2Path = @"Q:\cs\file2.cs";

        var encoding1 = Encoding.BigEndianUnicode;
        var encoding2 = Encoding.UTF32;

        var testCase = FormattingTestCases.WithJapaneseText;

        var source = testCase.Source.WithTrailingNewLine();
        var source1Bytes = encoding1.GetBytes(source);
        var source2Bytes = encoding2.GetBytes(source);

        await Assert.That(source1Bytes).IsNotEquivalentTo(source2Bytes);

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [file1Path] = new(source1Bytes),
                [file2Path] = new(source2Bytes),
            });
        var context = CreateTestContext(fileSystem);

        var sourceDirectory = fileSystem.FileInfo.New(file1Path).Directory!;

        var result = await context.RunAsync(
        [
            "format", sourceDirectory.FullName,
            "-r",
            "-l", testCase.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var expected = testCase.Formatted.WithTrailingNewLine();

        var newBytes1 = await fileSystem.File.ReadAllBytesAsync(file1Path);
        var newText1 = encoding1.GetString(newBytes1);
        await Assert.That(newText1).IsEqualTo(expected);

        var newBytes2 = await fileSystem.File.ReadAllBytesAsync(file2Path);
        var newText2 = encoding2.GetString(newBytes2);
        await Assert.That(newText2).IsEqualTo(expected);
    }

    public static IReadOnlyList<Encoding> GetTestEncodings()
    {
        return
        [
            Encoding.Utf8WithoutBom,
            Encoding.UTF8,
            Encoding.Unicode,
            Encoding.BigEndianUnicode,
            Encoding.UTF32,
            Encoding.ASCII,
            Encoding.Latin1,
            Encoding.Windows1252,
        ];
    }

    public static IReadOnlyList<Encoding> GetTestJapaneseEncodings()
    {
        return
        [
            Encoding.Utf8WithoutBom,
            Encoding.UTF8,
            Encoding.Unicode,
            Encoding.BigEndianUnicode,
            Encoding.UTF32,
        ];
    }

    private static async Task AssertSingleFileWithEncoding(
        Encoding? encoding,
        FormatTestCase testCase)
    {
        await Assert.That(encoding).IsNotNull();

        const string filePath = @"Q:\cs\file1.cs";

        var source = testCase.Source.WithTrailingNewLine();
        var sourceBytes = encoding.GetBytes(source);
        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [filePath] = new(sourceBytes),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", filePath,
            "-l", testCase.FormatOptions.MaxLineLength.ToString(),
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.Success);

        var newBytes = await fileSystem.File.ReadAllBytesAsync(filePath);
        var newText = encoding.GetString(newBytes);
        var expected = testCase.Formatted.WithTrailingNewLine();
        await Assert.That(newText).IsEqualTo(expected);
    }

    private static async Task AssertSourceTargetRelationship(
        IFileSystem fileSystem,
        string sourceFilePath,
        string targetFilePath,
        FormatTestCase formatTestCase)
    {
        var newSourceText = await fileSystem.File.ReadAllTextAsync(sourceFilePath);
        await Assert.That(newSourceText).IsEqualTo(formatTestCase.Source);

        var newTargetText = await fileSystem.File.ReadAllTextAsync(targetFilePath);
        await Assert.That(newTargetText).IsEqualTo(formatTestCase.Formatted);
    }
}
