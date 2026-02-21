namespace XmlDocFormat.Core;

public static class CollectionToStringExtensions
{
    extension<T>(IEnumerable<T> collection)
    {
        public string ToListString()
        {
            return string.Join(", ", collection);
        }
    }
}
