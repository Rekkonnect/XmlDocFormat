namespace XmlDocFormat.Cli;

public static class CommandCancellationHelpers
{
    public static void AugmentCancellation(ref CancellationToken cancellationToken)
    {
        var cancelKeyPressSource = new CancellationTokenSource();
        var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancelKeyPressSource.Token, cancellationToken);

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // Prevent immediate process termination
            cancelKeyPressSource.Cancel();
            Console.WriteLine("Cancellation requested...");
        };

        cancellationToken = linkedSource.Token;
    }
}
