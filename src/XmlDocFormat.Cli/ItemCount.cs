namespace XmlDocFormat.Cli;

public readonly record struct ItemCount(int Count, string Item)
{
    public string ToDisplayString()
    {
        if (Count is 0)
        {
            return string.Empty;
        }

        return $"{Count} {Item}";
    }
}
