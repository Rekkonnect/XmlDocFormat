using System.Reflection;

namespace XmlDocFormat.Core.Tests;

public partial class XmlDocFormattingTests
{
    [Test]
    [MethodDataSource(nameof(FormatTestCaseSource))]
    [ArgumentDisplayFormatter<FieldInfoArgumentDisplayFormatter>]
    [DisplayName("$testCaseField")]
    public async Task TestFormatCase(FieldInfo testCaseField)
    {
        var testCase = testCaseField.GetValue(null) as FormatTestCase;
        await Assert.That(testCase).IsNotNull();
        await AssertFormat(testCase);
    }

    public static IEnumerable<FieldInfo> FormatTestCaseSource()
    {
        return typeof(FormattingTestCases)
            .GetFields()
            .Where(s => s.IsStatic && s.FieldType == typeof(FormatTestCase));
    }

    private static async Task AssertFormat(FormatTestCase testCase)
    {
        var formatted = XmlDocFormatter.Format(testCase.Source, testCase.FormatOptions);
        await Assert.That(formatted).IsEqualTo(testCase.Formatted);
    }
}
