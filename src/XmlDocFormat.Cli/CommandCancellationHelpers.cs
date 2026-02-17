namespace XmlDocFormat.Cli;

public static class CommandCancellationHelpers
{
    public static void AugmentCancellation(ref CancellationToken cancellationToken)
    {
        var cancelKeyPressSource = CreateCancelKeyTokenSource();
        var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancelKeyPressSource.Token, cancellationToken);

        cancellationToken = linkedSource.Token;
    }

    public static CancellationTokenSource CreateCancelKeyTokenSource()
    {
        var cancelKeyPressSource = new CancellationTokenSource();

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // Prevent immediate process termination
            cancelKeyPressSource.Cancel();
            Console.WriteLine("Cancellation requested...");
        };

        return cancelKeyPressSource;
    }
}
