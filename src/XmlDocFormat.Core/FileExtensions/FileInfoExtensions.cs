namespace XmlDocFormat.Core.FileExtensions;

public static class FileInfoExtensions
{
    extension(FileInfo file)
    {
        public void CopyTo(FileInfo target, bool overwrite = false)
        {
            file.CopyTo(target.FullName, overwrite);
        }
    }
}
