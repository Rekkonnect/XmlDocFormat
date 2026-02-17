namespace XmlDocFormat.Core;

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

    public static T? Try<T>(Func<T> func, T? defaultValue = default)
    {
        try
        {
            return func();
        }
        catch
        {
            return defaultValue;
        }
    }
}
