using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocFormat.Core;

public class XmlDocFormatOptions
{
    public const int MinimumLineLength = 40;
    public const int DefaultLineLength = 80;

    /// <summary>
    /// The max length of each line, including the indentation.
    /// Must be at least <see cref="MinimumLineLength"/>.
    /// Defaults to <see cref="DefaultLineLength"/>.
    /// </summary>
    public int MaxLineLength
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(
                value,
                MinimumLineLength,
                nameof(MaxLineLength));
            field = value;
        }
    } = 80;

    /// <summary>
    /// An optional tag comparer, that compares <see cref="XmlDocTag"/>
    /// instances, representing the actual documentation tags.
    /// Such tags include summary, remarks, param, typeparam, returns, etc.,
    /// but not content tags like see or paramref.
    /// </summary>
    public IComparer<XmlDocTag>? TagComparer { get; set; }

    /// <summary>
    /// Determines whether content lines may be merged together,
    /// for the purposes of re-aligning and re-wrapping text to
    /// meet the <see cref="MaxLineLength"/> requirement.
    /// </summary>
    public bool MergeContentLines { get; set; } = true;

    // TODO: Add options to allow keeping a single line for some tags
}
