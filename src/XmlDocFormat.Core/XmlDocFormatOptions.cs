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
    /// Determines whether content lines may be merged together,
    /// for the purposes of re-aligning and re-wrapping text to
    /// meet the <see cref="MaxLineLength"/> requirement.
    /// </summary>
    public bool MergeContentLines { get; set; } = true;

    // TODO: Add options to allow keeping a single line for some tags
}
