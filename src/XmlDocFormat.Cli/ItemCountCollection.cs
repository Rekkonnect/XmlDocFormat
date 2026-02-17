using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using XmlDocFormat.Core;

namespace XmlDocFormat.Cli;

[CollectionBuilder(typeof(ItemCountCollection), nameof(Create))]
public sealed record class ItemCountCollection(
    IReadOnlyList<ItemCount> Items)
    : IReadOnlyList<ItemCount>
{
    public int Count => Items.Count;

    public string ToDisplayString()
    {
        var displayStrings = Items
            .Where(static s => s.Count is > 0)
            .Select(static s => s.ToDisplayString())
            .ToImmutableArray();

        if (displayStrings is [])
        {
            return string.Empty;
        }

        if (displayStrings is [var single])
        {
            return single;
        }

        var left = string.Join(", ", displayStrings.SkipLast(1));
        var right = displayStrings.Last();
        return $"{left} and {right}";
    }

    public static ItemCountCollection Create(ReadOnlySpan<ItemCount> items)
    {
        return new(items.AsReadOnlyList());
    }

    public IEnumerator<ItemCount> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ItemCount this[int index] => Items[index];
}
