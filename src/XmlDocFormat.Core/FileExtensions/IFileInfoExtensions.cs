using System.IO.Abstractions;

namespace XmlDocFormat.Core.FileExtensions;

public static class IFileInfoExtensions
{
    extension(IFileInfo file)
    {
        public void CopyTo(IFileInfo target, bool overwrite = false)
        {
            file.CopyTo(target.FullName, overwrite);
        }
    }
}
