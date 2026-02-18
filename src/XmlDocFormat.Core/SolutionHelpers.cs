using XmlDocFormat.Core.FileExtensions;

namespace XmlDocFormat.Core;

public static class SolutionHelpers
{
    public static DirectoryInfo GetSolutionRoot()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        return currentDirectory.GetAncestorOrSelfDirectoryWithFiles("*.sln*")
            ?? throw new InvalidOperationException(
                "Could not find the .sln* file in any of the directories");
    }
}
