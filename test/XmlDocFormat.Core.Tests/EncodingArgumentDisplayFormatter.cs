using System.Text;

namespace XmlDocFormat.Core.Tests;

public class EncodingArgumentDisplayFormatter : BaseTypeArgumentDisplayFormatter<Encoding>
{
    public override string FormatValue(Encoding value)
    {
        if (value.Preamble is not [])
        {
            return $"{value.WebName} (with BOM)";
        }

        return value.WebName;
    }
}
