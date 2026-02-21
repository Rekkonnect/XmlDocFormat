using System.IO.Abstractions;
using XmlDocFormat.InternalGenerators.Core;

namespace XmlDocFormat.Core;

[GaryonUtility]
[ConstructFromImplicitCast]
public readonly partial record struct DirectoryPath(string Path)
{
    public DirectoryPath(DirectoryInfo directoryInfo)
        : this(directoryInfo.FullName) { }

    public DirectoryPath(IDirectoryInfo directoryInfo)
        : this(directoryInfo.FullName) { }
}
