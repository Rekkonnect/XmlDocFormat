namespace XmlDocFormat.Core;

public readonly record struct RepeatedString(string String, int Count)
{
    public int Length => String.Length * Count;

    public override string ToString()
    {
        if (Count is 0)
        {
            return string.Empty;
        }

        var strings = Enumerable.Repeat(String, Count);
        return string.Concat(strings);
    }
}
