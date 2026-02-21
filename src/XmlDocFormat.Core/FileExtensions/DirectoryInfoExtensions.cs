namespace XmlDocFormat.Core.FileExtensions;

public static class DirectoryInfoExtensions
{
    extension(DirectoryInfo directory)
    {
        public void CreateSafe()
        {
            DelegateHelpers.Try(directory.Create);
        }

        [GaryonUtility]
        public DirectoryInfo? GetAncestorOrSelfDirectoryWithFiles(
            string searchPattern)
        {
            var currentDirectory = directory;
            while (currentDirectory != null)
            {
                var hasFiles = currentDirectory.EnumerateFiles(searchPattern).Any();
                if (hasFiles)
                    return currentDirectory;
                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }
    }
}
