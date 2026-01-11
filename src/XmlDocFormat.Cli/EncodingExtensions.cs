using System.Text;

namespace XmlDocFormat.Cli;

public static class EncodingExtensions
{
    extension(Encoding)
    {
        public static Encoding Utf8WithoutBom
            => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static Encoding Windows1252
            => CodePagesEncodingProvider.Instance.GetEncoding("windows-1252")!;
    }
}
