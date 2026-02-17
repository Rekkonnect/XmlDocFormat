using System.Reflection;
using XmlDocFormat.Tests.Shared;

namespace XmlDocFormat.Core.Tests;

public partial class VisualBasicXmlDocFormatTests
    : BaseXmlDocFormatTests<VisualBasicFormatTestCase>
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
        return GetTestCasesFromType(typeof(VisualBasicFormatTestCases));
    }

    protected override async Task AssertFormat(VisualBasicFormatTestCase testCase)
    {
        var formatted = VisualBasicXmlDocFormatter.Format(testCase.Source, testCase.FormatOptions);
        await Assert.That(formatted).IsEqualTo(testCase.Formatted);
    }
}
