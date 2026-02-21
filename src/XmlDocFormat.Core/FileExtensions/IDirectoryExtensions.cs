using System.Collections.Immutable;
using System.IO.Abstractions;

namespace XmlDocFormat.Core.FileExtensions;

public static class IDirectoryExtensions
{
    extension(IDirectory directory)
    {
        public ImmutableArray<string> GetFiles(
            string directoryPath,
            IReadOnlyList<string> searchPatterns,
            SearchOption searchOption)
        {
            var builder = ImmutableArray.CreateBuilder<string>();
            foreach (var pattern in searchPatterns)
            {
                var files = directory.GetFiles(directoryPath, pattern, searchOption);
                builder.AddRange(files);
            }

            return builder.ToImmutable();
        }
    }
}
