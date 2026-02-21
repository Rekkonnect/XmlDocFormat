using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using UtfUnknown;
using UtfUnknown.Core.Probers;
using UtfUnknown.Core.Probers.MultiByte;
using XmlDocFormat.Core;

namespace XmlDocFormat.Cli;

public static class CharsetDetectionHelpers
{
    public static DetectionResult DetectFromBytesEx(byte[] bytes)
    {
        var result = CharsetDetector.DetectFromBytes(bytes);
        var hasConvincingEncoding = result.Details
            .Any(IsConvincinglySatisfyingEncoding);

        if (!hasConvincingEncoding)
        {
            // Re-attempt all UTF encodings and ASCII, since they are
            // sometimes wrongly assessed in this library in v2.6
            IReadOnlyList<DetectionDetail> detectionDetails =
            [
                GetDetectionDetail(new UTF8Prober(), bytes),
                GetDetectionDetail(Encoding.Unicode, bytes),
                GetDetectionDetail(Encoding.BigEndianUnicode, bytes),
                GetDetectionDetail(Encoding.UTF32, bytes),
                GetDetectionDetail(Encoding.ASCII, bytes),
            ];

            const float confidenceThreshold = 0.85f;
            var missedConfidences = detectionDetails
                .Where(static s => s.Confidence > confidenceThreshold)
                .OrderByDescending(static s => s.Confidence)
                .ToList();

            if (missedConfidences is not [])
            {
                return new(missedConfidences);
            }
        }

        return result;

        // This method determines whether the detection is convincing,
        // otherwise being forced to re-attempt all UTF encodings,
        // since UTF8, UTF16 and UTF32 are sometimes undetected in Utf.Unknown v2.6
        static bool IsConvincinglySatisfyingEncoding(DetectionDetail detail)
        {
            var encoding = detail.Encoding;
            return ConvincingResults.IsConvincingEncoding(encoding);
        }
    }

    private static DetectionDetail GetDetectionDetail(
        Encoding encoding,
        byte[] bytes)
    {
        var confidence = GetEncodingConfidence(bytes, encoding);
        var detail = new DetectionDetail(encoding.WebName, confidence);
        return detail;
    }

    private static DetectionDetail GetDetectionDetail(CharsetProber prober, byte[] bytes)
    {
        prober.HandleData(bytes, 0, bytes.Length);
        return new DetectionDetail(prober);
    }

    private static float GetEncodingConfidence(byte[] bytes, Encoding encoding)
    {
        return DelegateHelpers.Try(
            () => GetEncodingConfidenceCore(bytes, encoding));
    }

    private static float GetEncodingConfidenceCore(byte[] bytes, Encoding encoding)
    {
        if (bytes is [])
        {
            return 0;
        }

        var text = encoding.GetString(bytes);
        if (text is null or "")
        {
            return 0;
        }

        int nulls = 0;
        int controls = 0;
        int legibles = 0;
        int commonCodeCharacters = 0;
        foreach (var ch in text)
        {
            var category = char.GetUnicodeCategory(ch);
            ref int counter = ref legibles;

            if (ch is '\0')
            {
                counter = ref nulls;
            }
            else if (category
                is UnicodeCategory.Control
                or UnicodeCategory.Format
                or UnicodeCategory.Surrogate
                or UnicodeCategory.PrivateUse
                or UnicodeCategory.OtherSymbol)
            {
                counter = ref controls;
            }
            else if (category
                is UnicodeCategory.LowercaseLetter
                or UnicodeCategory.UppercaseLetter
                or UnicodeCategory.OpenPunctuation
                or UnicodeCategory.ClosePunctuation
                or UnicodeCategory.OtherPunctuation)
            {
                counter = ref commonCodeCharacters;
            }

            counter++;
        }

        const double nullWeight = 5;
        const double controlWeight = 2.5;
        const double legibleWeight = 1;
        const double commonCodeCharacterWeight = 3;

        var positiveWeight = legibles * legibleWeight
            + commonCodeCharacters * commonCodeCharacterWeight;
        var negativeWeight = nulls * nullWeight
            + controls * controlWeight;
        var totalWeight = positiveWeight + negativeWeight;

        return (float)(positiveWeight / totalWeight);
    }

    private static class ConvincingResults
    {
        private static readonly FrozenSet<string> _convincingEncodingNames
            = GetConvincingEncodingNames();

        public static bool IsConvincingEncoding(Encoding encoding)
        {
            return IsConvincingEncoding(encoding.WebName);
        }

        public static bool IsConvincingEncoding(string webName)
        {
            return _convincingEncodingNames.Contains(webName);
        }

        private static FrozenSet<string> GetConvincingEncodingNames()
        {
            // Ignoring this warning because of UTF7
#pragma warning disable SYSLIB0001 // Type or member is obsolete
            IReadOnlyList<Encoding> convincingEncodings = [
                Encoding.UTF7,
                Encoding.UTF8,
                Encoding.Unicode,
                Encoding.BigEndianUnicode,
                Encoding.UTF32,
                Encoding.Windows1252,
            ];
#pragma warning restore SYSLIB0001 // Type or member is obsolete

            return [
                .. convincingEncodings.Select(static s => s.WebName),
                "utf-32be",
            ];
        }
    }
}
