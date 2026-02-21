using System.Reflection;
using XmlDocFormat.Tests.Shared;

namespace XmlDocFormat.Core.Tests;

public abstract partial class BaseXmlDocFormatTests<TTestCase>
    where TTestCase : BaseFormatTestCase
{
    protected async Task TestFormatCaseField(FieldInfo testCaseField)
    {
        var testCase = testCaseField.GetValue(null) as TTestCase;
        await Assert.That(testCase).IsNotNull();
        await AssertFormat(testCase!);
    }

    protected static IEnumerable<FieldInfo> GetTestCasesFromType(Type type)
    {
        return type.GetFields()
            .Where(s => s.IsStatic && s.FieldType == typeof(TTestCase));
    }

    protected abstract Task AssertFormat(TTestCase testCase);
}
