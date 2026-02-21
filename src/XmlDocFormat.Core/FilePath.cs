using System.IO.Abstractions;
using XmlDocFormat.InternalGenerators.Core;

namespace XmlDocFormat.Core;

[GaryonUtility]
[ConstructFromImplicitCast]
public readonly partial record struct FilePath(string Path)
{
    public FilePath(FileInfo fileInfo)
        : this(fileInfo.FullName) { }

    public FilePath(IFileInfo fileInfo)
        : this(fileInfo.FullName) { }
}
