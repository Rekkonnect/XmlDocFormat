using Garyon.Extensions;
using XmlDocFormat.Core;

namespace XmlDocFormat.Cli;

[GaryonUtility]
public static class CancellationTokenExtensions
{
    extension(CancellationToken cancellationToken)
    {
        public CancellationToken WithTimeout(TimeSpan timeout)
        {
            return cancellationToken.WithTimeout((int)timeout.TotalMilliseconds);
        }

        public CancellationToken WithTimeout(int milliseconds)
        {
            var timeoutSource = new CancellationTokenSource(milliseconds);
            var linkedSource = cancellationToken.CreateLinked(
                cancellationToken, timeoutSource.Token);
            return linkedSource.Token;
        }
    }
}
