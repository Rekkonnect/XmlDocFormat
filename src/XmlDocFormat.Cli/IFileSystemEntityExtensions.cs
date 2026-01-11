using System.IO.Abstractions;

namespace XmlDocFormat.Cli;

public static class IFileSystemEntityExtensions
{
    extension(IFileSystemEntity entity)
    {
        public bool EntityExists(string? path)
        {
            return entity switch
            {
                IFile file => file.Exists(path),
                IDirectory directory => directory.Exists(path),
                _ => throw new InvalidOperationException(
                    "The given entity does not provide an Exists method."),
            };
        }

        /// <summary>
        /// Creates the file or directory at the given path.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the entity does not support creating a path.
        /// </exception>
        public void Create(string path)
        {
            switch (entity)
            {
                case IFile file:
                    file.Create(path);
                    break;

                case IDirectory directory:
                    directory.CreateDirectory(path);
                    break;

                default:
                    throw new InvalidOperationException(
                        "The given entity does not provide a Create method.");
            }
        }

        /// <summary>
        /// Creates the file or directory at the given path, ensuring that
        /// the containing directory also exists to avoid throwing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the entity does not support creating a path.
        /// </exception>
        public void CreateEnsuringDirectory(string path)
        {
            switch (entity)
            {
                case IFile file:
                    var directory = file.FileSystem.FileInfo.New(path).Directory!;
                    directory.Create();
                    break;

                default:
                    entity.Create(path);
                    break;
            }
        }
    }
}
