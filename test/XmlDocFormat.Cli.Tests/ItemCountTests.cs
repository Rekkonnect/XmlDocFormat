using XmlDocFormat.Core;
using XmlDocFormat.Tests.Shared;

namespace XmlDocFormat.Cli.Tests;

public sealed class ItemCountTests
{
    [Test]
    [MethodDataSource(nameof(TestCases))]
    [ArgumentDisplayFormatter<ItemCountTestCaseArgumentDisplayFormatter>]
    public async Task DisplayStringConstruction(ItemCountTestCase testCase)
    {
        await testCase.AssertDisplayString();
    }

    public static IEnumerable<ItemCountTestCase> TestCases()
    {
        yield return new(
            ItemCounts: [],
            ExpectedDisplayString: string.Empty);

        yield return new(
            ItemCounts: [new(0, "a")],
            ExpectedDisplayString: string.Empty);

        yield return new(
            ItemCounts: [new(0, string.Empty)],
            ExpectedDisplayString: string.Empty);

        yield return new(
            ItemCounts: [
                new(0, string.Empty),
                new(0, string.Empty)
            ],
            ExpectedDisplayString: string.Empty);

        yield return new(
            ItemCounts: [new(1, "a")],
            ExpectedDisplayString: "1 a");

        yield return new(
            ItemCounts: [new(2, "a")],
            ExpectedDisplayString: "2 a");

        yield return new(
            ItemCounts: [
                new(3, "*.txt"),
                new(5, "*.pdf"),
            ],
            ExpectedDisplayString: "3 *.txt and 5 *.pdf");

        yield return new(
            ItemCounts: [
                new(3, "*.txt"),
                new(10, "*.docx"),
                new(5, "*.pdf"),
            ],
            ExpectedDisplayString: "3 *.txt, 10 *.docx and 5 *.pdf");
    }

    public readonly record struct ItemCountTestCase(
        ItemCountCollection ItemCounts,
        string ExpectedDisplayString)
    {
        public async Task AssertDisplayString()
        {
            var displayString = ItemCounts.ToDisplayString();
            await Assert.That(displayString).EqualTo(ExpectedDisplayString);
        }
    }

    public sealed class ItemCountTestCaseArgumentDisplayFormatter
        : BaseTypeArgumentDisplayFormatter<ItemCountTestCase>
    {
        public override string FormatValue(ItemCountTestCase value)
        {
            return $"{nameof(value.ItemCounts)}: [{value.ItemCounts.Items.ToListString()}], {nameof(value.ExpectedDisplayString)}: {value.ExpectedDisplayString}";
        }
    }
}
