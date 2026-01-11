namespace XmlDocFormat.Core.Tests;

public partial class XmlDocFormatOptionsTests
{
    [Test]
    public async Task TestInvalidOptions()
    {
        await Assert.That(ConstructOptions).ThrowsException();

        static XmlDocFormatOptions ConstructOptions()
        {
            return new()
            {
                MaxLineLength = XmlDocFormatOptions.MinimumLineLength - 1,
            };
        }
    }
}
