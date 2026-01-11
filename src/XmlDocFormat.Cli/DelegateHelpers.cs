namespace XmlDocFormat.Cli;

public static class DelegateHelpers
{
    public static void Try(Action action)
    {
        try
        {
            action();
        }
        catch
        {
        }
    }
}
