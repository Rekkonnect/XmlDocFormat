namespace XmlDocFormat.Core.FileExtensions;

public static class FileInfoExtensions
{
    extension(FileInfo file)
    {
        [GaryonUtility]
        public void CopyTo(FileInfo target, bool overwrite = false)
        {
            file.CopyTo(target.FullName, overwrite);
        }
    }
}
