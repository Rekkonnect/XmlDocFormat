namespace XmlDocFormat.Core.FileExtensions;

public static class DirectoryInfoExtensions
{
    extension(DirectoryInfo info)
    {
        public void CreateSafe()
        {
            DelegateHelpers.Try(info.Create);
        }
    }
}
