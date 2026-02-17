using System.Reflection;
using XmlDocFormat.Tests.Shared;

namespace XmlDocFormat.Core.Tests;

public partial class CSharpXmlDocFormatTests
    : BaseXmlDocFormatTests<CSharpFormatTestCase>
{
    [Test]
    [MethodDataSource(nameof(FormatTestCaseSource))]
    [ArgumentDisplayFormatter<FieldInfoArgumentDisplayFormatter>]
    [DisplayName("$testCaseField")]
    public async Task TestFormatCase(FieldInfo testCaseField)
    {
        await TestFormatCaseField(testCaseField);
    }

    public static IEnumerable<FieldInfo> FormatTestCaseSource()
    {
        return GetTestCasesFromType(typeof(CSharpFormatTestCases));
    }

    protected override async Task AssertFormat(CSharpFormatTestCase testCase)
    {
        var formatted = CSharpXmlDocFormatter.Format(testCase.Source, testCase.FormatOptions);
        await Assert.That(formatted).IsEqualTo(testCase.Formatted);
    }
}
