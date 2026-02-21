using System.IO.Abstractions;

namespace XmlDocFormat.Core.FileExtensions;

public static class IDirectoryInfoExtensions
{
    extension(IDirectoryInfo info)
    {
        public void CreateSafe()
        {
            DelegateHelpers.Try(info.Create);
        }
    }
}
