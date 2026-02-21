using System.IO.Abstractions;

namespace XmlDocFormat.Core.FileExtensions;

public static class IFileSystemInfoExtensions
{
    extension(IFileSystemInfo info)
    {
        public void Create()
        {
            switch (info)
            {
                case IFileInfo file:
                    file.Create();
                    break;

                case IDirectoryInfo directory:
                    directory.Create();
                    break;

                default:
                    throw new InvalidOperationException(
                        "The given info does not provide a Create method.");
            }
        }

        public void CreateSafe()
        {
            switch (info)
            {
                case IFileInfo file:
                    file.Create();
                    break;

                case IDirectoryInfo directory:
                    directory.CreateSafe();
                    break;

                default:
                    throw new InvalidOperationException(
                        "The given info does not provide a Create method.");
            }
        }
    }
}
