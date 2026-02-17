namespace XmlDocFormat.Core;

public static class SolutionHelpers
{
    public static DirectoryInfo GetSolutionRoot()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        var root = currentDirectory.FullName;
        while (currentDirectory != null)
        {
            var solutionFile = currentDirectory.EnumerateFiles("*.sln*").FirstOrDefault();
            if (solutionFile != null)
                return currentDirectory;
            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Could not find the .sln* file in any of the directories");
    }

}
